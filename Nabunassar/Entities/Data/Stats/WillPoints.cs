using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Game;
using Nabunassar.Localization;

namespace Nabunassar.Entities.Data.Stats
{
    internal class WillPoints : BaseStat<WillPoints>
    {
        public WillPoints(Creature creature=null) :base(creature)
        {
        }

        public override void Build(DescriptionBuilder builder, LocalizedStrings strings)
        {
            builder.AppendLine(DescriptionPosition.Left, strings["GameTexts"]["Characteristic"], Color.Gray);

            builder.AppendLine(DescriptionPosition.Center, strings["StatDescriptions"]["Will"]);
        }

        public override string GetName(LocalizedStrings strings) => strings["GameTexts"]["Will"];

        public override Color GetNameColor() => "#4ddee3".AsColor();
    }
}
