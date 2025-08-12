using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Widgets.UserEffects;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal class RevealAbility : BaseWorldAbility
    {
        public RevealAbility(NabunassarGame game, Creature creature, AbilityModel model) : base(game, model, creature)
        {
        }

        protected override void Execute(GameObject gameObject)
        {
            Game.IsMouseVisible = false;

            Game.AddDesktopWidget(new RevealCircle(Game));
            Game.RemoveDesktopWidgets<TitleWidget>();
        }

        public void CastReveal()
        {
            GameObject gameObject = null;

            var roll = Roll(gameObject.LandscapeRank, gameObject.LandscapeDice, this.Rank, this.Dice, this.Creature.PrimaryStats.AgilityDice);

            var resultColor = roll.IsSuccess ? Color.Green : Color.Red;
            var commonColor = Globals.BaseColor;

            var ui = Game.Strings["UI"].FineTuning();

            var resultText = roll.IsSuccess
                ? ui["SUCCESS"].ToString()
                : ui["FAILURE"].ToString();

            var text = DrawText.Create("")
                .Font(Fonts.BitterBold)
                .Size(16)
                .Color(commonColor)
                .Append(ui["SkillCheck"])
                .AppendSpace()
                .Color("#a2a832".AsColor())
                .Append(Name).AppendSpace()
                .Color(commonColor)
                .Append(": ")
                .Append($"{ui["difficult"]}: {roll.Complexity.ToDrawText(commonColor)}, {ui["throw"]}: {roll.Result.ToDrawText(commonColor)}: ")
                .Color(resultColor)
                .Append($" {resultText}")
                .Color(commonColor)
                .Append("!");

            Game.GameState.AddRollMessage(text, roll);
        }

        public override RollResult GetFormula()
        {
            return Roll(
                Rank.d2.Entity(GameObject.GetLandscapeAbility()),
                Dice.d2.Entity(GameObject.GetLandscapeAbility()),
                this.Rank,
                this.Dice,
                Dice.d2.Entity(PrimaryStats.GetStatDescription(nameof(PrimaryStats.Agility))));
        }

        private RollResult Roll(Rank checkRank, Dice checkDice, Rank skillRank, Dice skillDice, Dice characteristicdDice)
        {
            //var @lock = ((int)Rank.Advanced) * 2 + Dice.d12;
            //var user = ((int)Rank.Advanced) + Dice.d12 + Dice.d8;

            var checkValue = checkRank * 2 + checkDice;
            var skillValue = skillRank + skillDice + characteristicdDice;

            return new RollResult(checkValue, skillValue, true);
        }

        public override Result<bool> IsActive(GameObject gameObject)
        {
            return true;
        }

        public override bool IsApplicable(GameObject gameObject)
        {
            return true;
        }
    }
}
