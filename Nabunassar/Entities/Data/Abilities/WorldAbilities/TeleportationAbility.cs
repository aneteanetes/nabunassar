using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Dices.Terms;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct;
using Nabunassar.Widgets.UserEffects;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal class TeleportationAbility : BaseWorldAbility
    {
        public TeleportationAbility(NabunassarGame game, Creature creature, AbilityModel model) : base(game, model, creature)
        {
        }

        protected override void Execute(GameObject gameObject)
        {
            LastRoll = Roll(this.AbilityDice, Creature.PrimaryStats.IntelligenceDice);
            Game.AddDesktopWidget(new TeleportationInterface(Game,this, LastRoll.Result.ToValue()));
        }

        public void DoTeleport()
        {
            SpentEndurance();
        }

        public float AvailableDistance
        {
            get
            {
                if (this.AbilityRank < 4)
                    return Game.GameState.GameMeterMeasure;

                return this.AbilityRank.AsDice().Edges / 2 * Game.GameState.GameMeterMeasure;
            }
        }

        public DrawText AvailableDistanceFormula()
        {
            var r = this.AbilityRank.AsDice() / 2 * new DiceTermUnary(Game.GameState.GameMeterMeasure, $"1{Game.Strings["UI"]["m"]}");
            r.IsMax = true;
            return r.ToFormula();
        }

        public RollResult LastRoll { get; private set; }

        public override RollResult GetFormula() => Roll(this.AbilityDice, Creature.PrimaryStats.IntelligenceDice);

        private static RollResult Roll(Dice skillDice, Dice characteristicdDice)
        {
            var @throw = skillDice.RollMax() + characteristicdDice.RollMax();
            return new RollResult(@throw, true);
        }

        public override bool IsApplicable(GameObject gameObject)
        {
            return true;
        }
    }
}
