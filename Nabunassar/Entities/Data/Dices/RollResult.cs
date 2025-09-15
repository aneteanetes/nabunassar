namespace Nabunassar.Entities.Data.Dices
{
    internal class RollResult
    {
        public RollResult(DiceResult roll, bool isAutoMax = false)
        {
            Calculate(roll, isAutoMax);
        }

        protected virtual void Calculate(DiceResult roll, bool isAutoMax)
        {
            Result = roll;
            Result.IsMax = isAutoMax;
        }

        public DiceResult Result { get; protected set; }

    }
}