using ioi.Entities.Data.Dices;
using ioi.Entities.Data.Effects.Boons;
using ioi.Entities.Data.Effects.PartyEffects;
using ioi.Entities.Data.Enums;
using ioi.Entities.Game;
using ioi.Entities.Game.Stats;
using ioi.Entities.Struct;
using ioi.Widgets.UserEffects;
using ioi.Widgets.UserInterfaces;

namespace ioi.Entities.Data.Abilities.WorldAbilities
{
    internal class PrayerAbility : BaseWorldAbility
    {
        public PrayerAbility(GameHost game, Creature creature, AbilityModel model) : base(game, model, creature)
        {
        }

        public override int GetCharges()
        {
            return Creature.IsPrayerAvailable ? 1 : 0;
        }

        protected override void Execute(GameObject gameObject)
        {
            Game.AddDesktopWidget(new PrayerInterface(Game, this));
            Game.RemoveDesktopWidgets<TitleWidget>();
        }

        public void CastPrayer(Gods god)
        {
            SpentEndurance();

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
                var addResult = Game.GameState.Prayers.AddWorshipPoints(roll.Complexity.ToValue());
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
            Creature.Effects.Add(jugj);
            var msg = DrawText.Create("").Color(Color.Yellow).Append(Creature.HeroLink.Name).ResetColor().Append($" {Game.Strings["GameTexts"]["GotHeShe"].ToLower()} {Game.Strings["GameTexts"]["Charge"].ToLower()} ").Color(god.GodColor()).Append($"[{jugj.GetName(Game.Strings["Effects/EffectNames"])}]").ResetColor().Append(" !");
            Game.GameState.AddMessage(msg);
        }

        private void AddBlessing(Gods god)
        {
#warning pray blessing WIP
        }

        public override RollResultComplexity GetFormula() => Roll(this.AbilityDice, Dice.d2.Entity(PrimaryStats.GetStatDescription(nameof(PrimaryStats.Dialectics))));

        private RollResultPercent Roll(Dice skillDice, Dice characteristicdDice)
        {
            var complexity = skillDice * 4 + characteristicdDice;

            return new RollResultPercent(complexity);
        }

        public override Result<bool> IsActive(GameObject gameObject)
        {
            return true;

            if (AbilityRank.Value >= 3)
                return true;

            var isAvailable = Creature.IsPrayerAvailable;
            if (!isAvailable)
                return new Result<bool>(false, Game.Strings["UI"]["Prayer can be cast after battle"]);

            return base.IsActive(gameObject);
        }

        public override bool IsApplicable(GameObject gameObject)
        {
            return true;
        }
    }
}
