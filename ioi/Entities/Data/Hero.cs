using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using ioi.Components;
using ioi.Entities.Data.Enums;
using ioi.Entities.Game;
using ioi.Entities.Game.Enums;

namespace ioi.Entities.Data
{
    internal class Hero
    {
        private GameHost _game;

        public Sex Sex { get; set; }

        public Entity Entity { get; set; }

        public MapObject MapObject { get; set; }

        public Creature Creature { get; set; }

        public Hero(GameHost game, Archetype archetype)
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
        public AnimatedSprite Sprite { get; internal set; }
    }
}