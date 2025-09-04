using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Game;
using Nabunassar.Localization;

namespace Nabunassar.Entities.Data.Stats
{
    internal class ArmorClass : BaseStat<ArmorClass>
    {
        public ArmorClass(Creature creature=null) :base(creature)
        {
        }

        public override void Build(DescriptionBuilder builder, LocalizedStrings strings)
        {
            builder.AppendLine(DescriptionPosition.Left, strings["GameTexts"]["Characteristic"], Color.Gray);

            if (Creature != default)
                builder.AppendLine(DescriptionPosition.Left, $"{strings["StatDescriptions"]["ArmorClassBaseValue"]}: {Creature.ArmorClassBase}", Color.Gray);

            builder.AppendLine(DescriptionPosition.Center, strings["StatDescriptions"][nameof(ArmorClass)]);

            builder.AppendLine(DescriptionPosition.Center, strings["StatDescriptions"]["ArmorClassCD"], Color.Yellow);

            if (Creature != default)
                builder.AppendLine(DescriptionPosition.Center, $"{strings["StatDescriptions"]["ArmorClassCDAdded"]}: {Creature.ArmorClassCD}", Color.Orange);
        }

        public override string GetName(LocalizedStrings strings) => strings["GameTexts"][nameof(ArmorClass)];

        public override Color GetNameColor() => "#6285de".AsColor();
    }
}
