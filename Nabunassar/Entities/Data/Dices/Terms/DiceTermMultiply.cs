namespace Nabunassar.Entities.Data.Dices.Terms
{
    internal class DiceTermMultiply : DiceTerm
    {
        public DiceTerm Multiply { get; }

        public DiceTerm Dice { get; }

        public DiceTermMultiply(DiceTerm multiply, DiceTerm dice)
        {
            Multiply = multiply;
            Dice = dice;
        }

        public override string ToValue(bool isMax = false)
        {
            return GetValue(isMax).ToString();
        }

        public override string ToFormula() => $"{Multiply.ToFormula()}{Dice.ToFormula()}";

        public override int GetValue(bool isMax = false)
        {
            var result = 0;

            for (int i = 0; i < Multiply.GetValue(isMax); i++)
            {
                result += Dice.GetValue(isMax);
            }

            return result;
        }
    }
}
