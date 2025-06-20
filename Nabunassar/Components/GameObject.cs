using Geranium.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Components.Abstract;
using Nabunassar.Struct;
using System.Diagnostics;

namespace Nabunassar.Components
{
    [DebuggerDisplay("{Name} {Position}")]
    internal class GameObject : BaseComponent, ICollisionActor
    {
        public string Name { get; set; }

        public GameObject Parent { get; set; }

        public IShapeF Bounds { get; set; }

        public RectangleF BoundsAbsolute => new RectangleF(Bounds.Position - BoundRelativePosition, Bounds.BoundingRectangle.Size);

        public Color BoundsColor { get; set; }

        public Vector2 BoundRelativePosition { get;set; }

        public Entity Entity { get; private set; }

        public ObjectType ObjectType { get; private set; }

        public string LayerName { get; private set; } = null;

        private CollisionEventHandler _onCollistion;

        public Vector2 TargetPosition { get; set; } = Vector2.Zero;

        public Direction MoveDirection { get; set; } = Direction.Idle;

        public float MoveSpeed { get; set; } = 0f;

        public bool IsCustomSpeed { get; set; } = false;

        public Ray2 Ray2 { get; set; }

        public bool IsMoving => MoveDirection != Direction.Idle || Ray2 != default;

        private bool _isMoveable;
        public bool IsMoveable
        {
            get => _isMoveable;
            set
            {
                _isMoveable = value;
                if (Entity != default)
                {
                    if (_isMoveable)
                    {
                        Entity.Attach(new MoveableComponent());
                    }
                    else
                    {
                        Entity.Detach<MoveableComponent>();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="objType"></param>
        /// <param name="entity"></param>
        /// <param name="bounds">POSITION IS RELATIVE</param>
        /// <param name="layer"></param>
        /// <param name="onCollistion"></param>
        /// <param name="parent"></param>
        /// <param name="isMoveable"></param>
        public GameObject(NabunassarGame game, Vector2 position, ObjectType objType, Entity entity, RectangleF bounds=default, string layer = null, CollisionEventHandler onCollistion = null, GameObject parent = null, bool isMoveable=false) : base(game)
        {
            LayerName = layer;
            Parent = parent;
            Entity = entity;
            ObjectType = objType;
            Bounds = bounds;
            BoundRelativePosition = bounds.Position;
            Position = position;
            _onCollistion = onCollistion;
            IsMoveable = isMoveable;
            ResetMoveSpeed();
            
        }

        // position

        private Vector2 _position;

        public virtual Vector2 Position
        {
            get
            {
                if (Parent != null)
                {
                    var pos = _position + Parent.Position;
                    Bounds.Position = pos + BoundRelativePosition;
                    return pos;
                }

                return _position;
            }
            set
            {
                _position = value;
                Bounds.Position = value + BoundRelativePosition;
            }
        }

        public virtual void SetPosition(Vector2 position)
        {
            if (Parent != null)
            {
                position = position - Parent.Position;
            }

            Position = position;
        }

        public virtual void SetAbsolutePosition(Vector2 position)
        {
            _position = position;
        }

        public virtual void SetAbsolutePosition(float x, float y)
            => SetAbsolutePosition(new Vector2(x,y));

        public void SetPosition(float x, float y)
            => SetPosition(new Vector2(x, y));

        public void RecalculatePosition()
        {
            Bounds.Position = Position + BoundRelativePosition;
        }

        public Vector2 Origin
        {
            get
            {
                if (Bounds == default)
                    return Position;

                return new Vector2(Position.X + Bounds.BoundingRectangle.Size.Width / 2, Position.Y + Bounds.BoundingRectangle.Size.Height / 2);
            }
        }

        // bounds


        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (_onCollistion == null)
                return;

            var otherObj = collisionInfo.Other.As<GameObject>();
            if (otherObj.ObjectType == ObjectType.Cursor)
                return;

            var thisObj = Entity.Get<GameObject>();

            _onCollistion.Invoke(collisionInfo, thisObj, otherObj);

            if (this.ObjectType == ObjectType.Cursor)
                return;

            var host = this.Entity;
            var other = otherObj.Entity;

            if (otherObj.ObjectType == ObjectType.Ground)
            {
                var tileComp = other.Get<TileComponent>();
                if (tileComp != null)
                {
                    var groudType = tileComp.Polygon.GetPropopertyValue<GroundType>(nameof(GroundType));
                    MoveSpeed = Game.DataBase.GetGroundTypeSpeed(groudType);
                }
            }
            else
            {
                var hostCollision = host.Get<GameObject>();
                hostCollision.Position -= collisionInfo.PenetrationVector * 2;
            }
        }

        // moving

        public void MoveToDirection(Vector2 from, Vector2 moveSpeedVector)
        {
            TargetPosition = new Vector2(from.X + moveSpeedVector.X, from.Y + moveSpeedVector.Y);
            MoveDirection = Vector2.Zero.DetectDirection(moveSpeedVector);
            Ray2 = default;
        }

        public void MoveToPosition(Vector2 from, Vector2 positionToMoving)
        {
            TargetPosition = positionToMoving;
            MoveDirection = from.DetectDirection(positionToMoving);
            Ray2 = new Ray2(from, positionToMoving);
        }

        public void StopMove()
        {
            Ray2 = default;
            MoveDirection = Direction.Idle;
            TargetPosition = Vector2.Zero;
            OnStopMove?.Invoke();
        }

        public void ResetMoveSpeed()
        {
            if (!IsCustomSpeed)
                MoveSpeed = Game.DataBase.GetGroundTypeSpeed(GroundType.Dirt);
        }

        public Action OnStopMove { get; set; }

        public MovingEventHandler OnMoving { get; set; }
    }

    internal delegate void CollisionEventHandler(CollisionEventArgs collisionInfo, GameObject host, GameObject another);

    public delegate void MovingEventHandler(Vector2 prev, Vector2 next);
}
