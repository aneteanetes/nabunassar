using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data
{
    internal class Character
    {
        public string Tileset { get; set; }

        public string Name { get; set; } = Guid.NewGuid().ToString();

        public Direction ViewDirection { get; set; } = Direction.Right;

        //public Vector2 Position { get; set; }

        public float Speed { get; set; } = DefaultSpeed;

        public const float DefaultSpeed = 0.05f;
        public const float RunSpeed = 0.08f;

        public void OnCollision(CollisionEventArgs collisionInfo, Entity host, Entity other)
        {
            var otherCollision = other.Get<CollisionsComponent>();

            if (otherCollision.ObjectType== ObjectType.Ground)
            {
                Speed = RunSpeed;
            }
            else
            {
                var boundsComp = host.Get<CollisionsComponent>();
                boundsComp.Bounds.Position -= collisionInfo.PenetrationVector;

                var render = host.Get<RenderComponent>();
                render.Position -= collisionInfo.PenetrationVector;
            }
        }
    }
}
