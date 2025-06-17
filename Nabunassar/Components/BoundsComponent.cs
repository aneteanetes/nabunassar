using FontStashSharp;
using Geranium.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Struct;

namespace Nabunassar.Components
{
    internal class BoundsComponent : PositionComponent, ICollisionActor
    {
        public IShapeF Bounds { get; private set; }

        public override Vector2 Position { get => Bounds.Position; set => Bounds.Position = value; }

        public override Vector2 Origin { get => new Vector2(Position.X + Bounds.BoundingRectangle.Size.Width / 2, Position.Y + Bounds.BoundingRectangle.Size.Height / 2); set { } }

        public Entity Entity { get; private set; }
        public ObjectType ObjectType { get; private set; }

        private CollisionEventHandler _onCollistion;

        public string LayerName { get; private set; } = null;

        public BoundsComponent(NabunassarGame game, RectangleF bounds, ObjectType objType, Entity entity,string layer=null, CollisionEventHandler onCollistion=null):base(game)
        {
            LayerName = layer;
            Entity = entity;
            ObjectType = objType;
            Bounds = bounds;
            _onCollistion = onCollistion;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (_onCollistion == null)
                return;

            var otherCollision = collisionInfo.Other.As<BoundsComponent>();
            if (otherCollision.ObjectType == ObjectType.Cursor)
                return;

            var thisDesc = Entity.Get<DescriptorComponent>();
            var otherDesc = otherCollision.Entity.Get<DescriptorComponent>();

            _onCollistion.Invoke(collisionInfo,this.Entity,otherCollision.Entity);

            if (this.ObjectType == ObjectType.Cursor)
                return;

            var host = this.Entity;
            var other = otherCollision.Entity;

            if (otherCollision.ObjectType == ObjectType.Ground)
            {
                var tileComp = other.Get<TileComponent>();
                if (tileComp != null)
                {
                    var groudType = tileComp.Polygon.GetPropopertyValue<GroundType>(nameof(GroundType));
                    var moveComponent = host.Get<MoveComponent>();
                    if (moveComponent != null)
                        moveComponent.MoveSpeed = Game.DataBase.GetGroundTypeSpeed(groudType);
                }
            }
            else
            {
                var hostCollision = host.Get<BoundsComponent>();
                var renderHost = host.Get<RenderComponent>();

                hostCollision.Position -= collisionInfo.PenetrationVector * 2;
                renderHost.Position -= collisionInfo.PenetrationVector * 2;
            }
        }
    }

    public delegate void CollisionEventHandler(CollisionEventArgs collisionInfo, Entity host, Entity another);
}