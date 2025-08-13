using Geranium.Reflection;
using Newtonsoft.Json;

namespace Nabunassar.Entities.Data.Dices
{
    internal class Dice(int edges = 1, Guid objectId =default, int luckyRoll = 0)
    {
        public int Edges { get; } = edges;

        public Guid ObjectId { get; set; } = objectId;

        public int LuckyRoll { get; } = luckyRoll;

        private int rollsCounter = 0;

        private int[] _previousRolls = new int[luckyRoll];

        public override string ToString()
        {
            return "d" + Edges.ToString();
        }

#pragma warning disable IDE1006 // Naming Styles

        public static Dice d2 => new(2);

        public static Dice d4 => new(4);

        public static Dice d6 => new(6);

        public static Dice d8 => new(8);

        public static Dice d10 => new(10);

        public static Dice d12 => new(12);

        public static Dice d20 => new(20);

        public static Dice FromEntity(IEntity entity, Dice dice)
        {
            return new Dice(dice.Edges, entity.ObjectId, dice.LuckyRoll);
        }

        public Dice Entity(IEntity entity) => FromEntity(entity, this);

#pragma warning restore IDE1006 // Naming Styles

        public DiceRoll Roll(DiceOperation operation)
        {
            if (LuckyRoll == 0)
                return PureRoll(operation);

            //lucky dice rolling
            if (rollsCounter == LuckyRoll)
            {
                var prevAvg = ((int)Math.Round(_previousRolls.Average()));

                var result = NabunassarGame.Randoms.Next(prevAvg, Edges + 1);
                return new DiceRoll(this, result, operation);
            }
            else
            {
                var value = PureRoll(operation);
                _previousRolls[rollsCounter] = value.Result;
                rollsCounter++;

                return value;
            }
        }

        private DiceRoll PureRoll(DiceOperation operation)
        {
            var result = NabunassarGame.Randoms.Next(Edges + 1);

            return new DiceRoll(this, result, operation);
        }

        public DiceRoll MaxRoll(DiceOperation operation)
        {
            return new DiceRoll(this,Edges, operation);
        }

        public static DiceResult operator +(Dice one, Dice another)
        {
            var oneRoll = one.Roll(DiceOperation.Add);
            var anotherRoll = another.Roll(DiceOperation.Add);

            return new DiceResult(oneRoll.Result + anotherRoll.Result, default, DiceOperation.Add, oneRoll, anotherRoll);
        }

        public static DiceResult operator -(Dice one, Dice another)
        {
            var oneRoll = one.Roll(DiceOperation.Substract);
            var twoRoll = another.Roll(DiceOperation.Substract);

            var result = oneRoll.Result - twoRoll.Result;
            if(result<0)
                result = 0;

            return new DiceResult(result, default, DiceOperation.Substract, oneRoll, twoRoll);
        }

        public static DiceResult operator *(int dicesCount, Dice dice)
        {
            List<DiceRoll> rolls = new();

            var value = 0;
            for (int i = 0; i < dicesCount; i++)
            {
                var roll = dice.Roll(DiceOperation.Add);
                value += roll.Result;
            }

            return new DiceResult(value, default, DiceOperation.Add, [.. rolls]);
        }

        public static DiceResult operator +(int value, Dice dice)
        {
            var modifier = value;
            var roll = dice.Roll( DiceOperation.Add);

            var result = roll.Result + modifier;

            return new DiceResult(result, [new DiceModifier(value,value, DiceModifierType.Pure, DiceOperation.Add)], DiceOperation.Add, roll);
        }

        public static DiceResult operator -(int value, Dice dice)
        {
            var modifier = value;
            var roll = dice.Roll(DiceOperation.Substract);

            var result = roll.Result - modifier;
            if (result < 0)
                result = 0;

            return new DiceResult(result, [new DiceModifier(value,value, DiceModifierType.Pure, DiceOperation.Substract)], DiceOperation.Substract, roll);
        }

        public static DiceResult operator *(DiceModifier mod, Dice dice)
        {
            List<DiceRoll> rolls = new();

            var value = 0;
            for (int i = 0; i < mod.Result; i++)
            {
                var roll = dice.Roll(DiceOperation.Add);
                value += roll.Result;
            }

            return new DiceResult(value, [mod], DiceOperation.Add, [.. rolls]);
        }

        public static DiceResult operator +(DiceModifier mod, Dice dice)
        {
            var modifier = mod.Result;
            var roll = dice.Roll(DiceOperation.Add);

            var result = roll.Result + modifier;

            return new DiceResult(result, [mod], DiceOperation.Add, roll);
        }

        public static DiceResult operator -(DiceModifier mod, Dice dice)
        {
            var modifier = mod.Result;
            var roll = dice.Roll(DiceOperation.Substract);

            var result = roll.Result - modifier;
            if (result < 0)
                result = 0;

            return new DiceResult(result, [mod], DiceOperation.Substract, roll);
        }

        public override bool Equals(object obj)
        {
            if(obj == null) 
                return false;

            if (obj is not Dice dice)
                return false;

            return dice.Edges == this.Edges;
        }

        public override int GetHashCode()
        {
            return this.Edges.GetHashCode() + this.LuckyRoll.GetHashCode() + this.ObjectId.GetHashCode();
        }

        public static Dice Parse(string s)
        {
            if (s.IsEmpty() || !s.Contains("d"))
                throw new JsonSerializationException($"Dice expression is wrong! It must be 'dX', passed value: {s}");

            if (!int.TryParse(s.Replace("d", ""), out var diceNumber))
                throw new JsonSerializationException($"Dice expression is wrong! It must be 'dX', passed value: {s}");

            return new Dice(diceNumber);
        }
    }
}
