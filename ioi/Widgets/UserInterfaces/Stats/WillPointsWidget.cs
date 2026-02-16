using ioi.Entities.Data.Descriptions;
using ioi.Entities.Data.Stats;
using ioi.Entities.Game;

namespace ioi.Widgets.UserInterfaces.Stats
{
    internal class WillPointsWidget : BaseStatWidget
    {
        private WillStat _stat;

        public WillPointsWidget(GameHost game, Creature creature, int size=16/*20*/) : base(game, creature, "Assets/Tilesets/transparent_packed.png", size, new Rectangle(512, 160, 16, 16))
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