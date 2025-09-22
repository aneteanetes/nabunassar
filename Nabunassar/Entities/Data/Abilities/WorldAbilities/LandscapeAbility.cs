using Geranium.Reflection;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;
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

            SpentEndurance();

            var landRank = gameObject.LandscapeComplexity.Rank;
            var landDice = gameObject.LandscapeComplexity.Dice;

            var roll = Roll(landRank, landDice, this.AbilityRank, this.AbilityDice, this.Creature.PrimaryStats.ConstitutionDice);

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
                .Append($"{ui["difficult"]}: {roll.Complexity.ToString(commonColor)}, {ui["throw"]}: {roll.Result.ToString(commonColor)}: ")
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

        public override RollResultComplexity GetFormula()
        {
            return Roll(
                Rank.d2.Entity(GameObject.GetAbilityEntity("Landscape")),
                Dice.d2.Entity(GameObject.GetAbilityEntity("Landscape")),
                this.AbilityRank,
                this.AbilityDice,
                Dice.d2.Entity(PrimaryStats.GetStatDescription(nameof(PrimaryStats.Constitution))));
        }

        private RollResultComplexity Roll(Rank checkRank, Dice checkDice, Rank skillRank, Dice skillDice, Dice characteristicdDice)
        {
            var checkValue = checkRank * 2 + checkDice;
            var skillValue = skillRank + skillDice + characteristicdDice;

            return new RollResultComplexity(checkValue, skillValue, true);
        }

        public override Result<bool> IsActive(GameObject gameObject)
        {
            var value = _distanceMeter.IsObjectNear(gameObject);
            if (value.Message.IsNotEmpty())
                return value;

            var result = new Result<bool>(value);

            if (!value)
                result.Message = Game.Strings["GameTexts"]["TooAway"];

            if (!result)
                return result;

            return base.IsActive(gameObject);
        }

        public override bool IsApplicable(GameObject gameObject)
        {
            if (gameObject == default)
                return true;

            var objType = gameObject.ObjectType;

#warning landscape object type calculation is not flexible

            return
                objType is not ObjectType.NPC
                && objType is not ObjectType.Container
                && objType is not ObjectType.Player
                && objType is not ObjectType.Creature;
        }
    }
}
