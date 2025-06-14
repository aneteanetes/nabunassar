using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data
{
    internal class Character
    {
        private NabunassarGame _game;

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
            var otherCollision = other.Get<CollisionsComponent>();

            if (otherCollision.ObjectType == ObjectType.Cursor)
                return;

            if (otherCollision.ObjectType == ObjectType.Ground)
            {
                var tileComp = other.Get<TileComponent>();
                if (tileComp != null)
                {
                    var groudType = tileComp.Polygon.GetPropopertyValue<GroundType>(nameof(GroundType));
                    Speed = _game.DataBase.GetGroundTypeSpeed(groudType);
                }
            }
            else
            {
                var hostCollision = host.Get<CollisionsComponent>();
                hostCollision.Bounds.Position -= collisionInfo.PenetrationVector *2;

                var renderHost = host.Get<RenderComponent>();
                renderHost.Position -= collisionInfo.PenetrationVector *2;

                //hostCollision.Bounds.Position = hostCollision.PrevBoundPosition;
                //renderHost.Position = renderHost.PrevPosition;
            }
        }
    }
}