using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Formulas;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal class LandscapeAbility : BaseWorldAbility
    {
        public LandscapeAbility(NabunassarGame game, Creature creature) : base(game, game.DataBase.GetAbility("Landscape"), creature)
        {
        }

        protected override void Execute(GameObject gameObject)
        {
            if (gameObject == default)
                return;

            var roll = SkillCheck.Roll(gameObject.LandscapeRank, gameObject.LandscapeDice, this.Rank, this.Dice, this.Creature.PrimaryStats.AgilityDice);

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
                .Append($"{ui["difficult"]}: {roll.Complexity.ToDrawText(commonColor)}, {ui["throw"]}: {roll.Roll.ToDrawText(commonColor)}: ")
                .Color(resultColor)
                .Append($" {resultText}")
                .Color(commonColor)
                .Append("!");

            Game.GameState.AddRollMessage(text, roll);

            if (roll.IsSuccess)
            {
                gameObject.Destroy();
#warning landscape get resources
                // take resources
            }
        }

        public override bool IsApplicable(GameObject gameObject)
        {
            if (gameObject == default)
                return true;

            var objType = gameObject.ObjectType;
            return objType is not ObjectType.NPC;
        }
    }
}
