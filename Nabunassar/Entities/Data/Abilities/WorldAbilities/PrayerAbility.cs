using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Effects.Boons;
using Nabunassar.Entities.Data.Effects.PartyEffects;
using Nabunassar.Entities.Data.Enums;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;
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

        public void CastPrayer(Gods god)
        {
            if (!Game.GameState.PartyEffects.Contains<Worship>())
            {
                Game.GameState.PartyEffects.Add(new Worship(Game));
            }

            var roll = Roll(this.AbilityDice, Creature.PrimaryStats.DialecticsDice);

            Game.GameState.AddMessage(DrawText.Create("").Color(Color.Yellow).Append(Name).ResetColor().Append($": {Game.Strings["GameTexts"]["WorhipPointsTaken"].ToLower()}").Color(Color.Yellow).Append($" {roll.Complexity.ToValue()}").ResetColor().Append("."));

            var ui = Game.Strings["UI"].FineTuning();

            var resultColor = roll.IsSuccess ? Color.Green : Color.Red;
            var resultText = roll.IsSuccess
                ? ui["SUCCESS"].ToString()
                : ui["FAILURE"].ToString();

            var text = DrawText.Create("")
                .Color(Color.Yellow)
                .Append(Name)
                .ResetColor()
                .Append($": {Game.Strings["GameTexts"]["Worship"].ToLower()} {roll.Complexity.ToString(Globals.BaseColor)} , {ui["difficult"]} ").Color(Color.Yellow)
                .Append($"{roll.Result.ToValue()}").ResetColor().Append($" , {ui["result"]}: ")
                .Color(resultColor)
                .Append(resultText).Append(".");

            Game.GameState.AddRollMessage(text, roll);


            if (AbilityRank.Value > 0)
            {
                var addResult = Game.GameState.Prayers.Add(roll.Complexity.ToValue());
                if (addResult == Praying.PrayResult.Equal100AndReseted)
                {
                    Game.GameState.PartyEffects.Remove<Worship>();
                    AddJudgment(god);
                }
                else if (addResult == Praying.PrayResult.Overflowed)
                {
                    AddJudgment(god);
                }
            }

            if (AbilityRank.Value > 2)
            {
                if (roll.IsSuccess)
                {
                    AddJudgment(god);
                }
            }

            // counter will reset at 00:00 or after battle
            Creature.IsPrayerAvailable = false;

            if (AbilityRank.Value > 4)
            {
                AddBlessing(god);
            }
        }

        private void AddJudgment(Gods god)
        {
            var jugj = new Judgment(Game, god, AbilityRank, AbilityDice);
            Creature.Boons.Add(jugj);
            var msg = DrawText.Create("").Color(Color.Yellow).Append(Creature.HeroLink.Name).ResetColor().Append($" {Game.Strings["GameTexts"]["GotHeShe"].ToLower()} {Game.Strings["GameTexts"]["Charge"].ToLower()} ").Color(god.GodColor()).Append($"[{jugj.GetName(Game.Strings["Effects/EffectNames"])}]").ResetColor().Append(" !");
            Game.GameState.AddMessage(msg);
        }

        private void AddBlessing(Gods god)
        {
#warning pray blessing WIP
        }

        public override RollResult GetFormula() => Roll(this.AbilityDice, Dice.d2.Entity(PrimaryStats.GetStatDescription(nameof(PrimaryStats.Dialectics))));

        private RollResultPercent Roll(Dice skillDice, Dice characteristicdDice)
        {
            var complexity = skillDice * 4 + characteristicdDice;

            return new RollResultPercent(complexity);
        }

        public override Result<bool> IsActive(GameObject gameObject)
        {
            if (AbilityRank.Value >= 3)
                return true;

#warning prayer debug freecast
            return true;

            return Creature.IsPrayerAvailable;
        }

        public override bool IsApplicable(GameObject gameObject)
        {
            return true;
        }
    }
}
