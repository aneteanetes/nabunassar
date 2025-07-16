namespace Nabunassar.Entities.Data.Dices
{
    internal struct RollResult
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
                    Roll = rollMax;

                    ComplexityInner = complexity;
                    RollInner = roll;
                }
                else
                {
                    Complexity = complexity;
                    Roll = roll;
                }
            }
            else
            {
                Complexity = complexity;
                Roll = roll;
            }

            IsSuccess = Roll.Result >= Complexity.Result;
        }

        public DiceResult Complexity { get; private set; }

        public DiceResult Roll { get; private set; }

        public bool IsSuccess { get; private set; }

        private DiceResult ComplexityInner { get; set; }

        private DiceResult RollInner { get; set; }

    }
}