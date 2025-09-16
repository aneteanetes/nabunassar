using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Stats;
using Nabunassar.Entities.Game;

namespace Nabunassar.Widgets.UserInterfaces.Combat
{
    internal class WillPointsWidget : BaseStatWidget
    {
        private WillStat _stat;

        public WillPointsWidget(NabunassarGame game, Creature creature, int size=16/*20*/) : base(game, creature, "Assets/Tilesets/transparent_packed.png", size, new Rectangle(512, 160, 16, 16))
        {
            _stat = new WillStat(creature);
        }

        protected override string GetValue(Creature creature)
        {
            return creature.WillPoints.ToString();
        }

        public override Description GetDescription(Creature creature)
        {
            return _stat.GetDescription(Game);
        }

        protected override Color GetIconColor()
        {
            return WillStat.Instance.GetNameColor();
        }
    }
}