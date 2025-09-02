using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Enums;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Struct;
using Nabunassar.Localization;

namespace Nabunassar.Entities.Data.Effects.Boons
{
    internal class Judgment : BaseBoon
    {
        private Gods? _god;
        private Rank _rank;
        private Dice _dice;

        public Judgment(NabunassarGame game, Gods? god, Rank rank, Dice dice) : base(game)
        {
            _god = god;
            _rank = rank;
            _dice = dice;
        }

        protected override DrawText GetBody(LocalizedStrings strings)
        {
            var element = _god == null ? Elements.Physical : _god.Value.GodElement();
            var color = _god == null ? Globals.BaseColorLight : _god.Value.GodColor();

            var damageType = element == Elements.Physical
                ? Game.Strings["GameTexts"]["SelectedDamageType"]
                : Game.Strings["ElementDamageTexts"][element.ToString()];

            return DrawText.Create(strings["Judgment"]).Color(color).Append(_dice.ToFormula() + " " + damageType.ToLower()).ResetColor().Append(".");
        }

        public override void AdditionalDescription(DescriptionBuilder builder)
        {
            builder.AppendLine(DescriptionPosition.Left, DrawText.Create($"{Game.Strings["GameTexts"]["Application"]}: {Game.Strings["GameTexts"]["OnHit"].ToLower()}.", Color.White));
        }

        protected override int GetRank() => _rank.Value;

        public override string GetName(LocalizedStrings strings) => strings[nameof(Judgment)];

        protected override Color GetNameColor() => Color.Yellow;
    }
}