using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Struct;
using Nabunassar.Localization;

namespace Nabunassar.Entities.Data.Effects.Boons
{
    internal abstract class BaseBoon : BaseEffect
    {
        protected BaseBoon(NabunassarGame game) : base(game)
        {
        }

        public abstract string GetName(LocalizedStrings strings);

        protected virtual int GetRank() => 0;

        protected virtual Color GetNameColor() => default;

        public override EffectType Type => EffectType.Boon;

        protected abstract DrawText GetBody(LocalizedStrings strings);

        public override Description GetDescription()
        {
            var descBuilder = Description.Create(GetName(Game.Strings["Effects/EffectNames"]),GetNameColor());

            var rank = GetRank();
            if (rank > 0)
                descBuilder.Append(DescriptionPosition.Right, $"{Game.Strings["GameTexts"]["Rank"]} {rank}", Color.Gray, descBuilder.TextSizeTitle);

            descBuilder.AppendLine(DescriptionPosition.Left, Game.Strings["GameTexts"]["Boon"], Color.Gray);

            AdditionalDescription(descBuilder);

            //var descColor = "#ffc411".AsColor();

            var text = GetBody(Game.Strings["Effects/EffectDescriptions"]);

            descBuilder.Append(DescriptionPosition.Center, text);

            return descBuilder;
        }

        public virtual void AdditionalDescription(DescriptionBuilder builder) { }
    }
}
