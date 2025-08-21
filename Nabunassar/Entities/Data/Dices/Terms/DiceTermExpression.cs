namespace Nabunassar.Entities.Data.Dices.Terms
{
    internal class DiceTermExpression : DiceTerm
    {
        public DiceTermOperation Root { get; set; }

        public void Add(DiceTerm right, DiceOperation operation)
        {
            Root = new DiceTermOperation(operation, right, Root);
        }

        public override int GetValue(bool isMax = false)
        {
            return Root.GetValue(isMax);
        }

        public override string ToFormula()
        {
            return Root.ToFormula();
        }

        public override string ToValue(bool isMax = false)
        {
            return Root.ToValue(isMax);
        }
    }
}
