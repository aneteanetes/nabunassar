namespace Nabunassar.Entities.Data.Dices.Terms
{
    internal class DiceTermOperation : DiceTerm
    {
        public DiceOperation Operation { get; }

        public DiceTerm Left { get; }

        public DiceTerm Right { get; }

        public DiceTermOperation(DiceOperation operation, DiceTerm right, DiceTerm left = null)
        {
            Operation = operation;
            Left = left;
            Right = right;
        }

        public override string ToValue(bool isMax = false) => $"{Left?.ToValue(isMax) ?? ""}{Operation.ToOperatorString()}{Right.ToValue(isMax)}";

        public override string ToFormula() => $"{Left?.ToFormula() ?? ""}{Operation.ToOperatorString()}{Right.ToFormula()}";

        public override int GetValue(bool isMax = false)
        {
            switch (Operation)
            {
                case DiceOperation.Add:
                    {
                        return Left.GetValue(isMax) + Right.GetValue(isMax);
                    }
                case DiceOperation.Substract:
                    {
                        var value = Left.GetValue(isMax) - Right.GetValue(isMax);

                        if (value < 0)
                            return 0;

                        return value;
                    }
                case DiceOperation.Multiply:
                    {
                        return Left.GetValue(isMax) * Right.GetValue(isMax);
                    }
                default:
                    {
                        return Right.GetValue(isMax);
                    }
            }
        }
    }
}