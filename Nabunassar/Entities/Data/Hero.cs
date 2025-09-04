using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Entities.Data.Enums;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Enums;

namespace Nabunassar.Entities.Data
{
    internal class Hero
    {
        private NabunassarGame _game;

        public Sex Sex { get; set; }

        public Entity Entity { get; set; }

        public MapObject GameObject { get; set; }

        public Creature Creature { get; set; }

        public Hero(NabunassarGame game, Archetype archetype)
        {
            _game = game;
            Creature = new Creature(archetype,this);
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