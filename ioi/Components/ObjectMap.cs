using ioi.Components.Effects;
using ioi.Struct;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using System.Text;

namespace ioi.Components
{
    internal class ObjectMap
    {
        float idleTimer;
        float idleSpeed = 10f;      // Скорость парения
        float idleAmplitude = .5f;  // Высота в пикселях
        float timeOffset;          // Индивидуальная задержка
        float stepSleepMS = 0;
        float pathSleepMS = 0;

        public string Id { get; }

        private GameHost Game;

        public bool IsUpdatable { get; set; }

        public bool IsIdle { get; set; }

        public double Speed { get; set; } = 1;

        public bool IsMoveable { get; set; }

        public Rectangle? MoveArea { get; set; }

        private object autoMoveLock = new();

        public Side Side { get; set; }

        public Sprite Sprite { get; set; }

        public Color Color { get; set; }

        public bool IsBounds { get; set; }

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                RecalculateBounds();
            }
        }

        public Queue<Point> MovePath { get; set; }

        public bool MoveStopRequest { get; set; }

        public Vector2? DrawPosition { get; set; }

        [JsonIgnore]
        public Vector2 TargetPosition { get; set; }

        [JsonIgnore]
        public bool IsMoving { get; set; }

        public Point Coords { get; set; }

        public ShaderEffect Effect { get; set; }

        public Vector2 Size { get; set; }

        public Rectangle VisualBounds { get; private set; }

        public BoundingBox BoundingBox { get; private set; }

        public BoundingBox BoundingBoxCamera { get; private set; }

        public GameEntity Entity { get; private set; }

        public ObjectMap(GameHost game, string id)
        {
            Game = game;
            timeOffset = (float)new Random().NextDouble() * 10f;
            Id = id;
        }

        public void BindEntity(GameEntity entity)
        {
            Entity = entity;
            entity.MapObject = this;
            BindMoveable(entity);
        }

        private void BindMoveable(GameEntity entity)
        {
            if (entity.Components.Contains("Templates.Base.Moveable"))
            {
                this.IsUpdatable = true;
                this.IsMoveable = true;
                this.Speed = Entity["speed"].Number;
                this.idleSpeed = ((float)Entity["idleSpeed"].Number);
                this.idleAmplitude = ((float)Entity["idleAmplitude"].Number);
                this.stepSleepMS = ((float)Entity["stepSleepMS"].Number);
                this.pathSleepMS = ((float)Entity["pathSleepMS"].Number);

                var movearea = Entity["movearea"];
                if (!movearea.IsNil())
                {
                    var movetable = movearea.Table;

                    var x = ((int)movetable.Get("x").Number);
                    var y = ((int)movetable.Get("y").Number);
                    var w = ((int)movetable.Get("w").Number);
                    var h = ((int)movetable.Get("h").Number);

                    var location = new Point(((int)Coords.X) + x, ((int)Coords.Y) + y);
                    var size = new Point(w, h);

                    this.MoveArea = new Rectangle(location, size);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Effect != default)
                Effect.Update(gameTime);

            if (IsIdle)
            {
                idleTimer = (float)gameTime.TotalGameTime.TotalSeconds + timeOffset;
                float yOffset = (float)Math.Sin(idleTimer * idleSpeed) * idleAmplitude;
                DrawPosition = new Vector2(Position.X, Position.Y + yOffset);
            }

            if (IsMoveable && MoveArea.HasValue)
            {
                if (IsMoving)
                {
                    UpdateMoving();
                    return;
                }

                if (MovePath != default)
                {
                    var target = MovePath.Dequeue();
                    var moving = Game.GameState.Map.Move(this, target);
                    if (!moving)
                        return;

                    if (MovePath.Count == 0)
                    {
                        MovePath = null;
                    }
                }

                if (this.CanUpdate(gameTime, TimeSpan.FromMilliseconds(pathSleepMS), this.autoMoveLock))
                {
                    var moveArea = MoveArea.Value;
                    var randX = Game.Random.Next(moveArea.X, moveArea.X + moveArea.Width);
                    var randY = Game.Random.Next(moveArea.Y, moveArea.Y + moveArea.Height);

                    this.BindMovePath(new Microsoft.Xna.Framework.Point(randX, randY));
                }
            }
        }

        public Vector2 UpdateMoving()
        {
            var newPos = Vector2.Lerp(Position, TargetPosition, ((float)Speed));
            var diff = newPos - Position;
            Position = newPos;
            if (Vector2.Distance(Position, TargetPosition) < 0.1f)
            {
                Position = TargetPosition;
                IsMoving = false;
            }

            return diff;
        }

        public Vector2 GetPositionFromCoords()
        {
            return new Vector2(Coords.X * GameHost.Game.CellSize.X, Coords.Y * GameHost.Game.CellSize.Y);
        }

        public void RecalculateBounds()
        {
            VisualBounds = new Rectangle(Position.ToPoint(), Size.ToPoint());
            BoundingBox = new BoundingBox((Position + Size).ToVector3(0.5f), (Position + Size + Size).ToVector3(0.5f));
            BoundingBoxCamera = new BoundingBox((Position - Size / 2).ToVector3(0.5f), (Position + Size * 2 + Size).ToVector3(0.5f));
        }

        public void ProcessCollision(List<ObjectMap> collided)
        {
            if (collided.Count == 0)
                return;

            foreach (var collision in collided)
            {
                if (collision == this)
                    continue;

                if(collision.Entity==null)
                    continue;

                var hostCollideFunc = Entity?["collide"];
                if(hostCollideFunc.IsNotNil())
                {
                    Entity.Func("collide", this.Entity, this, collision);
                }

                if (Game.World.CombatSystem.IsInCombat)
                    break;

                var otherCollideFunc = collision?.Entity?["collide"];
                if (otherCollideFunc.IsNotNil())
                {
                    collision.Entity.Func("collide", collision.Entity, collision, this);
                }

                if (Game.World.CombatSystem.IsInCombat)
                    break;
            }
        }

        public Queue<Point> BindMovePath(Point target)
        {
            var path = Game.World.PathfindSystem.FindPath(Coords, target);

            if (path != default)
            {
                MovePath = new Queue<Point>(path);
            }

            return MovePath;
        }

        public void StopMove()
        {
            IsMoving = false;
            MovePath = default;
            TargetPosition = Position;
            MoveStopRequest = true;
        }

        internal void RemoveFromMap()
        {
            Game.GameState.Map.Remove(this);
        }
    }
}