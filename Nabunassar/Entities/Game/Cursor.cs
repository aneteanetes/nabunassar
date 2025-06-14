using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Components;

namespace Nabunassar.Entities.Game
{
    internal class Cursor
    {
        public Entity Entity { get; set; }

        public void OnCollision(CollisionEventArgs collisionInfo, Entity host, Entity another)
        {
            Entity = another;
        }
    }
}
