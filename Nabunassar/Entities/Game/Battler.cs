using Nabunassar.Entities.Game.Enums;

namespace Nabunassar.Entities.Game
{
    internal class Battler
    {
        public int BattlerId { get; set; }

        public int MaxHP { get; set; }

        public int NowHP { get; set; }
    }

    public static class BattlerExtensions
    {
        private static NabunassarGame Game => NabunassarGame.Game;

        internal static HPWounds BattlerWounds(this Battler battler)
        {
            var percent = (battler.NowHP / battler.MaxHP) * 100;
            var wounds = Game.DataBase.Get<Dictionary<HPWounds, int>>("Data/HPWounds/WoundPercentage.json");
            var wound= wounds.FirstOrDefault(x => x.Value <= percent);

            return wound.Key;
        }
    }
}