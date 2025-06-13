using Geranium.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
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
            var hostCollision = host.Get<CollisionsComponent>();
            var renderHost = host.Get<RenderComponent>();
            var renderOther = other.Get<RenderComponent>();
            var desc = host.Get<DescriptorComponent>();

            var otherCollision = other.Get<CollisionsComponent>();

            if (otherCollision.ObjectType == ObjectType.Ground)
            {
                var tileComp = other.Get<TileComponent>();
                if (tileComp != null)
                {
                    var groudType = tileComp.Polygon.GetPropopertyValue<GroundType>(nameof(GroundType));
                    Speed = _game.DataBase.GetGroundTypeSpeed(groudType);
                }
            }
            else if (renderOther.IsIntersects(hostCollision))
            {
                var normilizedVector = NormalizePenetrationVector(collisionInfo.PenetrationVector);

                //boundsComp.Bounds.Position -= normilizedVector;
                hostCollision.Bounds.Position = hostCollision.PrevBoundPosition - normilizedVector;
                //renderHost.Position -= normilizedVector;
                renderHost.Position = renderHost.PrevPosition - normilizedVector;

            }
        }

        private Vector2 NormalizePenetrationVector(Vector2 vector)
        {
            if(vector.X>0)
                vector.X = .015f;

            if(vector.Y>0)
                vector.Y = .015f;

            if (vector.X < 0)
                vector.X = -.015f;

            if(vector.Y<0)
                vector.Y = -.015f;

            return vector;
        }
    }
}