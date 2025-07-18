using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Dices
{
    internal class DiceRoll(Dice dice, int result, DiceOperation diceOperation)
    {
        public Dice Dice { get; } = dice;

        public int Result { get; } = result;

        public DiceOperation Operation { get; } = diceOperation;
    }
}
