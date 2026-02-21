using ioi.Components.Effects;
using ioi.Struct;
using MonoGame.Extended.Graphics;
using Newtonsoft.Json;

namespace ioi.Components
{
    internal class ObjectMap
    {
        float idleTimer;
        float idleSpeed = 10f;      // Скорость парения
        float idleAmplitude = .5f;  // Высота в пикселях
        float timeOffset;          // Индивидуальная задержка

        public ObjectMap()
        {
            timeOffset = (float)new Random().NextDouble() * 10f;
        }

        public bool IsUpdatable => IsIdle || Effect != default;

        public bool IsIdle { get; set; }

        public Side Side { get; set; }

        public void Update(GameTime gameTime)
        {
            if (Effect != default)
                Effect.Update(gameTime);

            idleTimer = (float)gameTime.TotalGameTime.TotalSeconds + timeOffset;

            float yOffset = (float)Math.Sin(idleTimer * idleSpeed) * idleAmplitude;

            // Рисуем, добавляя смещение только к визуальной части
            DrawPosition = new Vector2(Position.X, Position.Y + yOffset);
        }

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

        public Queue<Vector2> MovingPath { get; set; }

        public Vector2? DrawPosition { get; set; }

        [JsonIgnore]
        public Vector2 TargetPosition { get; set; }

        [JsonIgnore]
        public bool IsMoving { get; set; }

        public Vector2 Coords { get; set; }

        public ShaderEffect Effect { get; set; }

        public Vector2 Size { get; set; }

        public Rectangle VisualBounds { get; private set; }

        public BoundingBox BoundingBox { get; private set; }

        public BoundingBox BoundingBoxCamera { get; private set; }

        public Vector2 GetPositionFromCoords() => new Vector2(Coords.X * GameHost.Game.CellSize.X, Coords.Y * GameHost.Game.CellSize.Y);

        public Vector2 KeyCoords() => Coords;

        public Action<ObjectMap> OnCollide;

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
                if(OnCollide!=default)
                    OnCollide(collision);

                if (collision.OnCollide != default)
                    collision.OnCollide(this);
            }
        }
    }
}
