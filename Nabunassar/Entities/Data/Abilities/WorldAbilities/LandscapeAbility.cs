using Geranium.Reflection;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal class LandscapeAbility : BaseWorldAbility
    {
        private IDistanceMeter _distanceMeter;

        public LandscapeAbility(NabunassarGame game, IDistanceMeter distanceMeter, Creature creature, AbilityModel model) : base(game, model, creature)
        {
            _distanceMeter = distanceMeter;
        }

        protected override void Execute(GameObject gameObject)
        {
            if (gameObject == default)
                return;

            var landRank = gameObject.LandscapeComplexity.Rank;
            var landDice = gameObject.LandscapeComplexity.Dice;

            var roll = Roll(landRank, landDice, this.Rank, this.Dice, this.Creature.PrimaryStats.AgilityDice);

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

            if (roll.IsSuccess)
            {
                gameObject.Destroy();
#warning landscape get resources
                // take resources
            }
        }

        public override RollResult GetFormula()
        {
            return Roll(
                Rank.d2.Entity(GameObject.GetAbilityEntity("Landscape")),
                Dice.d2.Entity(GameObject.GetAbilityEntity("Landscape")),
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
            var value = _distanceMeter.IsObjectNear(gameObject);
            if (value.Message.IsNotEmpty())
                return value;

            var result = new Result<bool>(value);

            if (!value)
                result.Message = Game.Strings["GameTexts"]["TooAway"];

            return result;
        }

        public override bool IsApplicable(GameObject gameObject)
        {
            if (gameObject == default)
                return true;

            var objType = gameObject.ObjectType;
            return objType is not ObjectType.NPC && objType is not ObjectType.Container && objType is not ObjectType.Player;
        }
    }
}
