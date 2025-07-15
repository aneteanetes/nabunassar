using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Dices
{
    internal readonly struct DiceRoll(Dice dice, int result, DiceOperation diceOperation)
    {
        public readonly Dice Dice { get; } = dice;

        public readonly int Result { get; } = result;

        public readonly DiceOperation Operation { get; } = diceOperation;
    }
}
