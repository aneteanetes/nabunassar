using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Dices
{
    internal readonly struct DiceResult(int result, DiceModifier[] modifiers, DiceOperation operation, params DiceRoll[] diceThrows)
    {
        public readonly DiceOperation Operation { get; } = operation;

        public readonly DiceRoll[] Dices { get; } = diceThrows;

        public readonly DiceModifier[] Modifiers { get; } = modifiers == default ? [] : modifiers;

        public readonly int Result { get; } = result;

        public static DiceResult operator +(DiceResult diceResult, Dice dice)
        {
            var diceRoll = dice.Roll();
            var result = diceResult.Result+ diceRoll.Result;
            var dices = RecreateDiceRollArray(diceResult, diceRoll);

            return new DiceResult(result, diceResult.Modifiers, DiceOperation.Add, dices);
        }

        public static DiceResult operator -(DiceResult diceResult, Dice dice)
        {
            var diceRoll = dice.Roll();
            var result = diceResult.Result - diceRoll.Result;
            var dices = RecreateDiceRollArray(diceResult, diceRoll);

            return new DiceResult(result, diceResult.Modifiers, DiceOperation.Substract, dices);
        }

        public static DiceResult operator *(int dicesCount, DiceResult diceResult)
        {
           throw new NotImplementedException("somnitelno");
        }

        public static DiceResult operator +(int value, DiceResult diceResult)
        {
            var result = diceResult.Result + value;
            var modifiers = RecreateModifiersArray(diceResult,new DiceModifier(value, DiceModifierType.Pure));

            return new DiceResult(result, modifiers, DiceOperation.Add, diceResult.Dices);
        }

        public static DiceResult operator -(int value, DiceResult diceResult)
        {
            var result = diceResult.Result - value;
            if (result < 0)
                result = 0;

            var modifiers = RecreateModifiersArray(diceResult, new DiceModifier(value, DiceModifierType.Pure));

            return new DiceResult(result, modifiers, DiceOperation.Substract, diceResult.Dices);
        }

        public static DiceResult operator +(Rank value, DiceResult diceResult)
        {
            var result = diceResult.Result + (int)value;
            var modifiers = RecreateModifiersArray(diceResult, new DiceModifier(value, value));

            return new DiceResult(result, modifiers, DiceOperation.Add, diceResult.Dices);
        }

        public static DiceResult operator -(Rank value, DiceResult diceResult)
        {
            var result = diceResult.Result - (int)value;
            if (result < 0)
                result = 0;

            var modifiers = RecreateModifiersArray(diceResult, new DiceModifier(value, value));

            return new DiceResult(result, modifiers, DiceOperation.Substract, diceResult.Dices);
        }

        private static DiceRoll[] RecreateDiceRollArray(DiceResult diceResult, DiceRoll diceRoll)
        {
            var dices = new DiceRoll[diceResult.Dices.Length + 1];
            Array.Copy(diceResult.Dices, dices, diceResult.Dices.Length);
            dices[^1] = diceRoll;
            return dices;
        }

        private static DiceModifier[] RecreateModifiersArray(DiceResult diceResult, DiceModifier diceModifier)
        {
            var modifiers = new DiceModifier[diceResult.Dices.Length + 1];
            Array.Copy(diceResult.Modifiers, modifiers, diceResult.Modifiers.Length);
            modifiers[^1] = diceModifier;
            return modifiers;
        }

        public override string ToString()
        {
            var operationChar = Operation == DiceOperation.Add ? " + " : " - ";

            var diceResult = string.Join(operationChar, Dices.Select(x => x.Result));

            var modifier = $"+ {Modifiers.Sum(x => x.Value)}";

            return $"({diceResult}) {modifier}";
        }
    }
}