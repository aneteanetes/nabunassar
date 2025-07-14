using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Dices
{
    internal struct Dice(int edges = 1, Description description=null, int lucky = 0)
    {
        public int Edges { get; } = edges;

        public Description Described { get; set; } = description;

        public int LuckyRoll { get; } = lucky;

        private int rollsCounter = 0;

        private int[] _previousRolls = new int[lucky];

        public override string ToString()
        {
            return "d" + Edges.ToString();
        }

#pragma warning disable IDE1006 // Naming Styles
        public static Dice d4 => new(4);

        public static Dice d6 => new(6);

        public static Dice d8 => new(8);

        public static Dice d10 => new(10);

        public static Dice d12 => new(12);

        public static Dice d20 => new(20);

        public Dice Description(Description description)
        {
            this.Described = description;
            return this;
        }

#pragma warning restore IDE1006 // Naming Styles

        public DiceRoll Roll()
        {
            if (LuckyRoll == 0)
                return PureRoll();

            //lucky dice rolling
            if (rollsCounter == LuckyRoll)
            {
                var prevAvg = ((int)Math.Round(_previousRolls.Average()));

                var result = NabunassarGame.Random.Next(prevAvg, Edges + 1);
                return new DiceRoll(this, result);
            }
            else
            {
                var value = PureRoll();
                _previousRolls[rollsCounter] = value.Result;
                rollsCounter++;

                return value;
            }
        }

        private DiceRoll PureRoll()
        {
            var result = NabunassarGame.Random.Next(Edges + 1);

            return new DiceRoll(this, result);
        }

        public static DiceResult operator +(Dice one, Dice another)
        {
            var oneRoll = one.Roll();
            var anotherRoll = another.Roll();

            return new DiceResult(oneRoll.Result + anotherRoll.Result, default, DiceOperation.Add, oneRoll, anotherRoll);
        }

        public static DiceResult operator -(Dice one, Dice another)
        {
            var oneRoll = one.Roll();
            var twoRoll = another.Roll();

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
                var roll = dice.Roll();
                value += roll.Result;
            }

            return new DiceResult(value, default, DiceOperation.Add, [.. rolls]);
        }

        public static DiceResult operator +(int value, Dice dice)
        {
            var modifier = value;
            var roll = dice.Roll();

            var result = roll.Result + modifier;

            return new DiceResult(result, [new DiceModifier(value, DiceModifierType.Pure)], DiceOperation.Add, roll);
        }

        public static DiceResult operator -(int value, Dice dice)
        {
            var modifier = value;
            var roll = dice.Roll();

            var result = roll.Result - modifier;
            if (result < 0)
                result = 0;

            return new DiceResult(result, [new DiceModifier(value, DiceModifierType.Pure)], DiceOperation.Substract, roll);
        }

        public static DiceResult operator *(Rank dicesCount, Dice dice)
        {
            List<DiceRoll> rolls = new();

            var value = 0;
            for (int i = 0; i < dicesCount; i++)
            {
                var roll = dice.Roll();
                value += roll.Result;
            }

            return new DiceResult(value, [new DiceModifier(0, dicesCount)], DiceOperation.Add, [.. rolls]);
        }

        public static DiceResult operator +(Rank value, Dice dice)
        {
            var modifier = (int)value;
            var roll = dice.Roll();

            var result = roll.Result + modifier;

            return new DiceResult(result, [new DiceModifier(value, value)], DiceOperation.Add, roll);
        }

        public static DiceResult operator -(Rank value, Dice dice)
        {
            var modifier = (int)value;
            var roll = dice.Roll();

            var result = roll.Result - modifier;
            if (result < 0)
                result = 0;

            return new DiceResult(result, [new DiceModifier(value, value)], DiceOperation.Substract, roll);
        }
    }
}
