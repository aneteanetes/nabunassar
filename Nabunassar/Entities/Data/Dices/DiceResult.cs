using Geranium.Reflection;
using info.lundin.math;
using Nabunassar.Entities.Data.Dices.Terms;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using Nabunassar.Systems;
using SharpDX;

namespace Nabunassar.Entities.Data.Dices
{
    internal class DiceResult
    {
        public bool IsMax { get; set; }

        public DiceTermExpression Terms { get; set; }

        public DiceResult(DiceTerm root=default)
        {
            Terms = new DiceTermExpression();
            if (root != default)
                Terms.Root = new DiceTermOperation(DiceOperation.Dices, root);
        }

        public DiceResult(DiceTerm term, DiceOperation operation)
        {
            Terms.Add(term, operation);
        }

        public DiceResult(DiceTerm left, DiceOperation operation, DiceTerm right)
        {
            Terms = new DiceTermExpression()
            {
                Root = new DiceTermOperation(operation, right, left)
            };
        }

        public int ToValue() => Terms.GetValue(IsMax);

        public DrawText ToString(Color resetColor = default)
        {
            var drawtext = DrawText.Create("")
                .Color(Color.Yellow)
                .Append(ToValue().ToString());

            if (resetColor != default)
                drawtext = drawtext.Color(resetColor);
            else
                drawtext = drawtext.ResetColor();

            drawtext = drawtext.Append(" = ").Append(Terms.ToValue(IsMax));

            return drawtext;
        }

        public DrawText ToFormula()
        {
            return DrawText.Create("").Append(Terms.ToFormula());
        }

        public static DiceResult operator +(Rank rank, DiceResult diceResult)
        {
            diceResult.Terms.Add(rank, DiceOperation.Add);
            return diceResult;
        }

        public static DiceResult operator -(DiceResult diceResult, Rank rank)
        {
            diceResult.Terms.Add(rank, DiceOperation.Substract);
            return diceResult;
        }

        public static DiceResult operator *(DiceResult diceResult, Rank rank)
        {
            diceResult.Terms.Add(rank, DiceOperation.Multiply);
            return diceResult;
        }

        public static DiceResult operator +(DiceResult diceResult, Dice dice)
        {
            diceResult.Terms.Add(dice.Roll(), DiceOperation.Add);
            return diceResult;
        }

        public static DiceResult operator -(DiceResult diceResult, Dice dice)
        {
            diceResult.Terms.Add(dice.Roll(), DiceOperation.Substract);
            return diceResult;
        }

        public static DiceResult operator *(int dicesCount, DiceResult diceResult)
        {
            var multipleDiceTerm = new DiceTermMultiply(new DiceTermUnary(dicesCount), diceResult.Terms.Root);
            diceResult.Terms.Add(multipleDiceTerm, DiceOperation.Add);
            return diceResult;
        }

        public static DiceResult operator *(DiceTerm mod, DiceResult diceResult)
        {
            diceResult.Terms.Add(mod, DiceOperation.Multiply);
            return diceResult;
        }

        public static DiceResult operator +(int value, DiceResult diceResult)
        {
            diceResult.Terms.Add(new DiceTermUnary(value), DiceOperation.Add);
            return diceResult;
        }

        public static DiceResult operator -(int value, DiceResult diceResult)
        {
            diceResult.Terms.Add(new DiceTermUnary(value), DiceOperation.Substract);
            return diceResult;
        }

        public static DiceResult operator +(DiceTerm modifier, DiceResult diceResult)
        {
            diceResult.Terms.Add(modifier, DiceOperation.Add);
            return diceResult;
        }

        public static DiceResult operator -(DiceTerm modifier, DiceResult diceResult)
        {
            diceResult.Terms.Add(modifier, DiceOperation.Substract);
            return diceResult;
        }

        public static DiceResult operator +(DiceResult value, DiceResult diceResult)
        {
            diceResult.Terms.Add(value, DiceOperation.Add);
            return diceResult;
        }

        public static DiceResult operator *(DiceResult diceResult, int value)
        {
            diceResult.Terms.Add(new DiceTermUnary(value), DiceOperation.Multiply);
            return diceResult;
        }

        public static DiceResult operator *(DiceResult diceResult, DiceTermUnary unary)
        {
            diceResult.Terms.Add(unary, DiceOperation.Multiply);
            return diceResult;
        }

        public static implicit operator Rank(DiceResult diceResult) => new Rank(diceResult.ToValue());

        public override string ToString()
        {
            return this.ToString(Color.White).ToUnformatString();
        }
    }
}