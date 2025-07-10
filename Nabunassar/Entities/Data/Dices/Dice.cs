using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Dices
{
    internal struct Dice(int edges = 1, int lucky=0)
    {
        public int Edges { get; } = edges;

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

#pragma warning restore IDE1006 // Naming Styles

        public int Roll()
        {
            if (LuckyRoll == 0)
                return PureRoll();

            //lucky dice rolling
            if (rollsCounter == LuckyRoll)
            {
                var prevAvg = ((int)Math.Round(_previousRolls.Average()));

                return NabunassarGame.Random.Next(prevAvg, Edges + 1);
            }
            else
            {
                var value = PureRoll();
                _previousRolls[rollsCounter] = value;
                rollsCounter++;

                return value;
            }
        }

        private int PureRoll() => NabunassarGame.Random.Next(Edges + 1);

        public static int operator +(Dice one, Dice another)
        {
            return one.Roll()+another.Roll();
        }

        public static int operator -(Dice one, Dice another)
        {
            return one.Roll() - another.Roll();
        }

        public static int operator *(int dices, Dice dice)
        {
            var value = 0;
            for (int i = 0; i < dices; i++)
            {
                value += dice.Roll();
            }

            return value;
        }
        
        public static int operator +(int value, Dice dice)
        {
            return value + dice.Roll();
        }

        public static int operator -(int value, Dice dice)
        {
            return dice.Roll() - value;
        }
    }
}
