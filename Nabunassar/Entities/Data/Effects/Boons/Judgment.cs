using Geranium.Reflection;
using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Enums;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Struct;
using Nabunassar.Localization;
using Nabunassar.Struct.Interfaces;

namespace Nabunassar.Entities.Data.Effects.Boons
{
    internal class Judgment : BaseBoon
    {
        private Gods? _god;
        private Rank _rank;
        private Dice _dice;

        private List<Judgment> _stacks = new();

        public Judgment(NabunassarGame game, Gods? god, Rank rank, Dice dice) : base(game)
        {
            _god = god;
            _rank = rank;
            _dice = dice;
        }

        public override EffectType Type => EffectType.Boon;

        public override string IconPath => "Assets/Images/Icons/Effects/revolt.png";

        public override Color IconColor => _god.Value.GodColor();

        public override void Merge(IStackable other)
        {
            _stacks.Add(other.As<Judgment>());
            Charges++;

            base.Merge(other);
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

            if (Charges > 1)
            {
                var chargesText = DrawText.Create($"{Game.Strings["GameTexts"]["Charges"]}: ", Color.LightYellow);

                void appendCharge(Gods god, Dice dice)
                {
                    chargesText.Color(god.GodColor()).Append($" {dice.ToFormula()} ").ResetColor();
                }
                appendCharge(_god.Value, _dice);
                foreach (var inner in _stacks)
                {
                    chargesText.Append("|");
                    appendCharge(inner._god.Value,inner._dice);
                }

                builder.AppendLine(DescriptionPosition.Left, chargesText);
            }
        }

        protected override int GetRank() => _rank.Value;

        public override string GetName(LocalizedStrings strings) => strings[nameof(Judgment)];

        protected override Color GetNameColor() => Color.Yellow;
    }
}