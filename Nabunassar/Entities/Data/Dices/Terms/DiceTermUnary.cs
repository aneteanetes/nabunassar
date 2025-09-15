namespace Nabunassar.Entities.Data.Dices.Terms
{
    internal class DiceTermUnary : DiceTerm
    {
        private string _string;
        private string _name;
        private int _value;

        public DiceTermUnary(int value, string name=null)
        {
            _name = name;
            _value = value;
            _string = value.ToString();
        }

        public override int GetValue(bool isMax = false) => _value;

        public override string ToFormula() => _name ?? _string;

        public override string ToValue(bool isMax = false) => _string;
    }
}
