using ioi.Entities.Game;

namespace ioi.Widgets.UserInterfaces.Stats
{
    internal class HPLine : StatLine
    {
        public HPLine(GameHost game, Creature creature, int length = 75) : base(game, creature, length)
        {
        }

        public override string TextureAsset => "Assets/Tilesets/transparent_packed.png";

        public override Rectangle TextureRegion => new Rectangle(624, 160, 16, 16);

        public override Color Color => Color.Red;

        public override int GetCurrent()
        {
            return Creature.HPMax;
        }

        public override int GetMax()
        {
            return Creature.HPNow;
        }

        public override string GetTooltip()
        {
            return Game.Strings["GameTexts"]["Health"];
        }
    }
}