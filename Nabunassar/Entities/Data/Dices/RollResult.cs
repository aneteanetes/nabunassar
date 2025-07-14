namespace Nabunassar.Entities.Data.Dices
{
    internal struct RollResult
    {
        public DiceResult Complexity { get; set; }

        public DiceResult Roll { get; set; }

        public bool IsSuccess { get; set; }
    }
}
