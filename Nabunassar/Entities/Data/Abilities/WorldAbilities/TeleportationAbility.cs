using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Struct;
using Nabunassar.Widgets.UserEffects;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal class TeleportationAbility : BaseWorldAbility
    {
        public TeleportationAbility(NabunassarGame game, Creature creature, AbilityModel model) : base(game, model, creature)
        {
        }

        protected override void Execute(GameObject gameObject)
        {
            Game.AddDesktopWidget(new TeleportationInterface(Game));
        }

        public void CastReveal(List<GameObject> objs)
        {
            List<Pair<RollResult, GameObject>> revealedObjs = [];

            foreach (var obj in objs)
            {
                var rankDice = obj.RevealComplexity;
                var roll = Roll(rankDice.Rank, rankDice.Dice, this.AbilityRank, this.AbilityDice, this.Creature.PrimaryStats.IntelligenceDice);

                if (roll.IsSuccess)
                {
                    obj.Reveal();
                    revealedObjs.Add(new Pair<RollResult, GameObject>(roll,obj));
                }
            }

            var ui = Game.Strings["UI"].FineTuning();
            var commonColor = Globals.BaseColor;

            if (revealedObjs.Count > 0)
            {
                foreach (var revealedObj in revealedObjs)
                {
                    var text = DrawText.Create("")
                        .Font(Fonts.BitterBold)
                        .Size(16)
                        .Color("#a2a832".AsColor())
                        .Append(ui["Revealed"])
                        .Color(commonColor)
                        .Append($": [{revealedObj.Second.GetObjectNameTitle()}] !");

                    Game.GameState.AddRollMessage(text, revealedObj.First);
                }
            }
            else
            {
                var text = DrawText.Create("")
                    .Color("#a2a832".AsColor())
                    .Append(Name).AppendSpace()
                    .Color(commonColor)
                    .Append(": ")
                    .Append(ui["nothing is found"]).Append(".");

                Game.GameState.AddMessage(text);
            }
        }

        public override RollResult GetFormula()
        {
            return Roll(
                Rank.d2.Entity(GameObject.GetAbilityEntity("Reveal")),
                Dice.d2.Entity(GameObject.GetAbilityEntity("Reveal")),
                this.AbilityRank,
                this.AbilityDice,
                Dice.d2.Entity(PrimaryStats.GetStatDescription(nameof(PrimaryStats.IntelligenceDice))));
        }

        private RollResult Roll(Rank checkRank, Dice checkDice, Rank skillRank, Dice skillDice, Dice characteristicdDice)
        {
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
