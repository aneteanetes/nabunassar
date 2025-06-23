using MonoGame.Extended.ECS;
using Nabunassar.Components;

namespace Nabunassar.Entities.Data
{
    internal class Hero
    {
        private NabunassarGame _game;
        
        public Entity Entity { get; set; }

        public MapObject GameObject { get; set; }

        public Hero(NabunassarGame game)
        {
            _game = game;
        }

        public string Tileset { get; set; }

        public int Order
        {
            get => Entity.Get<OrderComponent>().Order;
            set => Entity.Get<OrderComponent>().Order = value;
        }

        public string Name { get; set; } = Guid.NewGuid().ToString();
    }
}