using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data
{
    internal class Character
    {
        private NabunassarGame _game;
        
        public Entity Entity { get; set; }

        public Character(NabunassarGame game)
        {
            _game = game;
            SetDirtSpeed();
        }

        public string Tileset { get; set; }

        public string Name { get; set; } = Guid.NewGuid().ToString();

        public Direction ViewDirection { get; set; } = Direction.Right;

        public float Speed { get; set; }

        public void SetDirtSpeed()
        {
            Speed = _game.DataBase.GetGroundTypeSpeed(GroundType.Dirt);
        }

        public void OnCollision(CollisionEventArgs collisionInfo, Entity host, Entity other)
        {
            
        }
    }
}