using Geranium.Reflection;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Struct;

namespace Nabunassar.Components
{
    internal class CollisionsComponent : ICollisionActor
    {
        public IShapeF Bounds { get; private set; }

        public Entity Entity { get; private set; }

        public ObjectType ObjectType { get; private set; }

        private CollisionEventHandler _onCollistion;

        public CollisionsComponent(IShapeF bounds, ObjectType objType, Entity entity, CollisionEventHandler onCollistion=null)
        {
            Entity = entity;
            ObjectType = objType;
            Bounds = bounds;
            _onCollistion = onCollistion;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            var otherCollision = collisionInfo.Other.As<CollisionsComponent>();
            _onCollistion?.Invoke(collisionInfo,this.Entity,otherCollision.Entity);
        }
    }

    public delegate void CollisionEventHandler(CollisionEventArgs collisionInfo, Entity host, Entity another);
}