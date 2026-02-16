using ioi.Entities.Data.Descriptions;
using ioi.Entities.Game;
using ioi.Localization;

namespace ioi.Entities.Data.Stats
{
    internal class WillStat : BaseStat<WillStat>
    {
        public WillStat(Creature creature=null) :base(creature)
        {
        }

        public override void Build(DescriptionBuilder builder, LocalizedStrings strings)
        {
            builder.AppendLine(DescriptionPosition.Left, strings["GameTexts"]["Characteristic"], Color.Gray);

            builder.AppendLine(DescriptionPosition.Center, strings["StatDescriptions"][nameof(WillStat)]);
        }

        public override string GetName(LocalizedStrings strings) => strings["GameTexts"][nameof(WillStat)];

        public override Color GetNameColor() => "#4ddee3".AsColor();
    }
}
