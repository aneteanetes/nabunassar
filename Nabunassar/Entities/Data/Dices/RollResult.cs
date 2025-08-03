namespace Nabunassar.Entities.Data.Dices
{
    internal class RollResult
    {
        public RollResult(DiceResult complexity, DiceResult roll, bool isAutoMax = false)
        {
            if (isAutoMax)
            {
                var complexityMax = complexity.Maximum();
                var rollMax = roll.Maximum();

                if (rollMax.Result >= complexityMax.Result)
                {
                    Complexity = complexityMax;
                    Result = rollMax;

                    ComplexityInner = complexity;
                    RollInner = roll;
                }
                else
                {
                    Complexity = complexity;
                    Result = roll;
                }
            }
            else
            {
                Complexity = complexity;
                Result = roll;
            }

            IsSuccess = Result.Result >= Complexity.Result;
        }

        public DiceResult Complexity { get; private set; }

        public DiceResult Result { get; private set; }

        public bool IsSuccess { get; private set; }

        private DiceResult ComplexityInner { get; set; }

        private DiceResult RollInner { get; set; }

    }
}