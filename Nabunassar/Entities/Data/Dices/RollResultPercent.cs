namespace Nabunassar.Entities.Data.Dices
{
    internal class RollResultPercent : RollResultComplexity
    {
        public RollResultPercent(DiceResult complexity, bool isAutoMax = false) : base(complexity, Dice.d100.Roll(), isAutoMax)
        {
        }

        protected override void Calculate(DiceResult complexity, DiceResult roll, bool isAutoMax)
        {
            if (isAutoMax)
            {
                complexity.IsMax = true;
                roll.IsMax = true;

                var complexityMax = complexity.ToValue();
                var rollMax = roll.ToValue();

                if (rollMax <= complexityMax)
                {
                    IsAutoSuccess = true;
                }
                else
                {
                    complexity.IsMax = false;
                    roll.IsMax = false;
                }
            }
            else
            {
                Complexity = complexity;
                Result = roll;
            }

            IsSuccess = Result.ToValue() <= Complexity.ToValue();
        }

    }
}