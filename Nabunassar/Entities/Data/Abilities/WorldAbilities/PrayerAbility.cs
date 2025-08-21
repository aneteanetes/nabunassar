using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Struct;
using Nabunassar.Widgets.UserEffects;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal class PrayerAbility : BaseWorldAbility
    {
        public PrayerAbility(NabunassarGame game, Creature creature, AbilityModel model) : base(game, model, creature)
        {
        }

        protected override void Execute(GameObject gameObject)
        {
            Game.AddDesktopWidget(new PrayerInterface(Game, this));
            Game.RemoveDesktopWidgets<TitleWidget>();
        }

        public void CastReveal(List<GameObject> objs)
        {
            List<Pair<RollResult, GameObject>> revealedObjs = new();

            foreach (var obj in objs)
            {
                var rankDice = obj.RevealComplexity;
                var roll = Roll(this.AbilityDice, this.Creature.PrimaryStats.IntelligenceDice);

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

        public override RollResult GetFormula() => Roll(this.AbilityDice, Dice.d2.Entity(PrimaryStats.GetStatDescription(nameof(PrimaryStats.DialecticsDice))));

        private RollResult Roll(Dice skillDice, Dice characteristicdDice)
        {
            var complexity = skillDice * 4 + characteristicdDice;
                        
            return new RollResultPercent(complexity);
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
