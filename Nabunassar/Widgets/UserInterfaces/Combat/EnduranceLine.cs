using Nabunassar.Entities.Game;

namespace Nabunassar.Widgets.UserInterfaces.Combat
{
    internal class EnduranceLine : StatLine
    {
        public EnduranceLine(NabunassarGame game, Creature creature, int length = 75) : base(game, creature, length)
        {
        }

        public override string TextureAsset => "Assets/Tilesets/transparent_packed.png";

        public override Rectangle TextureRegion => new Rectangle(432, 336, 16, 16);

        public override Color Color => "#DCF014".AsColor();

        public override int GetCurrent()
        {
            return Creature.EnduranceNow;
        }

        public override int GetMax()
        {
            return Creature.EnduranceMax;
        }

        public override string GetTooltip()
        {
            return Game.Strings["GameTexts"]["Endurance"];
        }
    }
}