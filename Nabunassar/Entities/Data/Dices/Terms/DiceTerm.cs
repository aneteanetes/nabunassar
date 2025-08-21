namespace Nabunassar.Entities.Data.Dices.Terms
{
    internal abstract class DiceTerm
    {
        public abstract int GetValue(bool isMax = false);

        public abstract string ToFormula();

        public abstract string ToValue(bool isMax = false);
    }
}