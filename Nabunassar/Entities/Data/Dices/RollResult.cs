namespace Nabunassar.Entities.Data.Dices
{
    internal class RollResult
    {
        public RollResult(DiceResult complexity, DiceResult roll, bool isAutoMax = false)
        {
            Calculate(complexity, roll, isAutoMax);
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

        public bool IsAutoSuccess { get; internal set; }

        public DiceResult Complexity { get; protected set; }

        public DiceResult Result { get; protected set; }

        public bool IsSuccess { get; protected set; }

        protected DiceResult ComplexityInner { get; set; }

        protected DiceResult RollInner { get; set; }

    }
}