using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Struct;

namespace Nabunassar.Entities.Data.Dices
{
    internal class DiceResult(int result, DiceModifier[] modifiers, DiceOperation operation, params DiceRoll[] diceThrows)
    {
        public  DiceOperation Operation { get; } = operation;

        public  DiceRoll[] DiceRolls { get; } = diceThrows;

        public  DiceModifier[] Modifiers { get; } = modifiers == default ? [] : modifiers;

        public  int Result { get; } = result;

        public DiceResult Maximum()
        {
            var diceRolls = DiceRolls.Select(r => r.Dice.MaxRoll(r.Operation));
            var diceResult = diceRolls.Sum(dr => dr.Result);
            var modifiers = Modifiers.Sum(x=>x.Result);
            return new DiceResult(diceResult + modifiers, Modifiers, Operation, diceRolls.ToArray());
        }

        public static DiceResult operator +(DiceResult diceResult, Dice dice)
        {
            var diceRoll = dice.Roll(DiceOperation.Add);
            var result = diceResult.Result + diceRoll.Result;
            var dices = RecreateDiceRollArray(diceResult, diceRoll);

            return new DiceResult(result, diceResult.Modifiers, DiceOperation.Add, dices);
        }

        public static DiceResult operator -(DiceResult diceResult, Dice dice)
        {
            var diceRoll = dice.Roll(DiceOperation.Add);
            var result = diceResult.Result - diceRoll.Result;
            var dices = RecreateDiceRollArray(diceResult, diceRoll);

            return new DiceResult(result, diceResult.Modifiers, DiceOperation.Substract, dices);
        }

        public static DiceResult operator *(int dicesCount, DiceResult diceResult)
        {
            var result = diceResult.Result + dicesCount;
            var modifiers = RecreateModifiersArray(diceResult, new DiceModifier(dicesCount, dicesCount, DiceModifierType.Pure, DiceOperation.Multiply));

            return new DiceResult(result, modifiers, DiceOperation.Add, diceResult.DiceRolls);
        }

        public static DiceResult operator *(DiceModifier mod, DiceResult diceResult)
        {
            var result = diceResult.Result + mod.Result;
            var modifiers = RecreateModifiersArray(diceResult, mod);

            return new DiceResult(result, modifiers, DiceOperation.Add, diceResult.DiceRolls);
        }

        public static DiceResult operator +(int value, DiceResult diceResult)
        {
            var result = diceResult.Result + value;
            var modifiers = RecreateModifiersArray(diceResult, new DiceModifier(value, value, DiceModifierType.Pure, DiceOperation.Add));

            return new DiceResult(result, modifiers, DiceOperation.Add, diceResult.DiceRolls);
        }

        public static DiceResult operator -(int value, DiceResult diceResult)
        {
            var result = diceResult.Result - value;
            if (result < 0)
                result = 0;

            var modifiers = RecreateModifiersArray(diceResult, new DiceModifier(value, value, DiceModifierType.Pure, DiceOperation.Substract));

            return new DiceResult(result, modifiers, DiceOperation.Substract, diceResult.DiceRolls);
        }

        public static DiceResult operator +(DiceModifier modifier, DiceResult diceResult)
        {
            var result = diceResult.Result + modifier.Result;
            var modifiers = RecreateModifiersArray(diceResult, modifier);

            return new DiceResult(result, modifiers, DiceOperation.Add, diceResult.DiceRolls);
        }

        public static DiceResult operator -(DiceModifier modifier, DiceResult diceResult)
        {
            var result = diceResult.Result - modifier.Result;
            if (result < 0)
                result = 0;

            var modifiers = RecreateModifiersArray(diceResult, modifier);

            return new DiceResult(result, modifiers, DiceOperation.Substract, diceResult.DiceRolls);
        }

        public static implicit operator Rank(DiceResult diceResult) => new Rank(diceResult.Result);

        private static DiceRoll[] RecreateDiceRollArray(DiceResult diceResult, DiceRoll diceRoll)
        {
            var dices = new DiceRoll[diceResult.DiceRolls.Length + 1];
            Array.Copy(diceResult.DiceRolls, dices, diceResult.DiceRolls.Length);
            dices[^1] = diceRoll;
            return dices;
        }

        private static DiceModifier[] RecreateModifiersArray(DiceResult diceResult, DiceModifier diceModifier)
        {
            var modifiers = new DiceModifier[diceResult.DiceRolls.Length + 1];
            Array.Copy(diceResult.Modifiers, modifiers, diceResult.Modifiers.Length);
            modifiers[^1] = diceModifier;
            return modifiers;
        }

        public override string ToString() => ToDrawText().ToUnformatString();

        public DrawText ToDrawText(Color resetColor = default, bool isFull=false)
        {
            var operationChar = "";
            switch (Operation)
            {
                case DiceOperation.Add:
                    operationChar = " + ";
                    break;
                case DiceOperation.Substract:
                    operationChar = " - ";
                    break;
                case DiceOperation.Multiply:
                    operationChar = " * ";
                    break;
                default:
                    break;
            }

            var diceResult = string.Join(operationChar, DiceRolls.Select(x => x.Result));

            var modifier = "";

            if (isFull)
            {
                foreach (var mod in Modifiers)
                {
                    modifier += Operation.ToOperatorString()+" (" + mod.ToString() + ")";
                }
            }
            else
                modifier = $"+ {Modifiers.Sum(x => x.Result)}";

            var drawtext = DrawText.Create("")
                .Color(Color.Yellow)
                .Append(Result.ToString());

            if (resetColor == default)
                drawtext = drawtext.ResetColor();
            else
                drawtext = drawtext.Color(resetColor);

            drawtext = drawtext.Append($" = ({diceResult}) {modifier}");

            return drawtext;
        }

        public DrawText ToFormula()
        {
            var db = NabunassarGame.Game.DataBase;
            var text = DrawText.Create("");

            var opStr = Operation.ToOperatorString();

            var dices = string.Join("", DiceRolls.Select(diceRoll =>
            {
                return $"{NabunassarGame.Game.Strings["GameTexts"]["Dice"]}({diceRoll.Dice}) {db.GetEntity(diceRoll.Dice.ObjectId).FormulaName} {diceRoll.Operation.ToOperatorString()} ";
            }));

            text.Append(dices + " ");


            foreach (var mod in Modifiers)
            {
                text.Append(mod.ToFormulaString());
            }

            return text;
        }
    }
}