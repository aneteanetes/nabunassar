namespace Nabunassar.Entities.Data.Dices
{
    internal class RollResultComplexity : RollResult
    {

        public RollResultComplexity(DiceResult complexity, DiceResult roll, bool isAutoMax = false) : base(roll, isAutoMax)
        {
        }

        protected virtual void Calculate(DiceResult complexity, DiceResult roll, bool isAutoMax)
        {
            Complexity = complexity;
            Result = roll;

            if (isAutoMax)
            {
                complexity.IsMax = true;
                roll.IsMax = true;

                var complexityMax = complexity.ToValue();
                var rollMax = roll.ToValue();

                if (rollMax >= complexityMax)
                {
                    IsAutoSuccess = true;
                }
                else
                {
                    complexity.IsMax = false;
                    roll.IsMax = false;
                }
            }

            IsSuccess = Result.ToValue() >= Complexity.ToValue();
        }

        public DiceResult Complexity { get; protected set; }

        public bool IsAutoSuccess { get; internal set; }

        public bool IsSuccess { get; protected set; }

    }
}