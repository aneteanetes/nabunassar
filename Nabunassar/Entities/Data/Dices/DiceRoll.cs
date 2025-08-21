using Nabunassar.Entities.Data.Dices.Terms;
using Nabunassar.Resources;
using System.Diagnostics;

namespace Nabunassar.Entities.Data.Dices
{
    [DebuggerDisplay("{Result}({Dice})")]
    internal class DiceRoll(Dice dice, int result) : DiceTerm
    {
        public Dice Dice { get; } = dice;

        public int Result { get; private set; } = result;

        public override int GetValue(bool isMax = false)
        {
            if (isMax)
                return Dice.RollMax().Result;

            return Result;
        }

        public override string ToFormula()
        {
            var entity = DataBase.GetEntity(this.Dice.ObjectId);

            var value = $"{this.Dice}";

            if (entity != default)
                value += $"({entity?.FormulaName})";

            return value;
        }

        public override string ToValue(bool isMax = false)
        {
            if (isMax)
                return Dice.RollMax().Result.ToString();
            else
                return Result.ToString();
        }

        public static implicit operator DiceResult(DiceRoll roll)
        {
            return new DiceResult(roll);
        }
    }
}
