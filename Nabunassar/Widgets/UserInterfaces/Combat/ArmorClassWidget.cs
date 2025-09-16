using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Stats;
using Nabunassar.Entities.Game;

namespace Nabunassar.Widgets.UserInterfaces.Combat
{
    internal class ArmorClassWidget : BaseStatWidget
    {
        public ArmorClassWidget(NabunassarGame game, Creature creature, int size = 16/*20*/) : base(game, creature, "Assets/Tilesets/transparent_packed.png",size, new Rectangle(624, 48, 16, 16))
        {
        }

        protected override string GetValue(Creature creature)
        {
            return creature.ArmorClass.GetComplexValue().ToString();
        }

        public override Description GetDescription(Creature creature)
        {
            return new ArmorClassStat(creature).GetDescription(Game);
        }

        protected override Color GetIconColor()
        {
            return ArmorClassStat.Instance.GetNameColor();
        }
    }
}