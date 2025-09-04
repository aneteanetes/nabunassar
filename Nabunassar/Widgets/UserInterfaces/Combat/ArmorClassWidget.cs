using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Stats;
using Nabunassar.Entities.Game;

namespace Nabunassar.Widgets.UserInterfaces.Combat
{
    internal class ArmorClassWidget : BaseStatWidget
    {
        public ArmorClassWidget(NabunassarGame game, Creature creature) : base(game, creature, "Assets/Tilesets/transparent_packed.png",20, new Rectangle(624, 48, 16, 16))
        {
        }

        protected override string GetValue(Creature creature)
        {
            return creature.ArmorClass.GetComplexValue().ToString();
        }

        public override Description GetDescription(Creature creature)
        {
            return new ArmorClass(creature).GetDescription(Game);
        }

        protected override Color GetIconColor()
        {
            return ArmorClass.Instance.GetNameColor();
        }
    }
}