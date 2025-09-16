using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Game;
using Nabunassar.Localization;

namespace Nabunassar.Entities.Data.Stats
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
