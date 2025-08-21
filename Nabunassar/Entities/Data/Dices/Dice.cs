using Geranium.Reflection;
using info.lundin.math;
using Nabunassar.Entities.Data.Dices.Terms;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Resources;
using Newtonsoft.Json;

namespace Nabunassar.Entities.Data.Dices
{
    internal class Dice(int edges = 1, Guid objectId = default, int luckyRoll = 0) : DiceTerm
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

        public static Dice d100
        {
            get
            {
                var dice = new Dice(100);

                var game = NabunassarGame.Game;
                if (game != null)
                {
                    var chanceEntity = DataBase.AddEntity(new DescribeEntity()
                    {
                        FormulaName = game.Strings["Entities"]["Chance 100%"]
                    });

                    return dice.Entity(chanceEntity);
                }

                return dice.Entity(new DescribeEntity() { FormulaName = "%" });
            }
        }

        public static Dice FromEntity(IEntity entity, Dice dice)
        {
            return new Dice(dice.Edges, entity.ObjectId, dice.LuckyRoll);
        }

        public Dice Entity(IEntity entity) => FromEntity(entity, this);

#pragma warning restore IDE1006 // Naming Styles

        public DiceRoll Roll()
        {
            if (LuckyRoll == 0)
                return PureRoll();

            //lucky dice rolling
            if (rollsCounter == LuckyRoll)
            {
                var prevAvg = ((int)Math.Round(_previousRolls.Average()));

                var result = NabunassarGame.Randoms.Next(prevAvg, Edges);
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
            var result = NabunassarGame.Randoms.Next(1,Edges);

            return new DiceRoll(this, result);
        }

        public DiceRoll RollMax()
        {
            return new DiceRoll(this,Edges);
        }

        public static DiceResult operator +(Dice one, Dice another)
        {
            var oneRoll = one.Roll();
            var anotherRoll = another.Roll();

            return new DiceResult(oneRoll, DiceOperation.Add, anotherRoll);
        }

        public static DiceResult operator -(Dice one, Dice another)
        {
            var oneRoll = one.Roll();
            var twoRoll = another.Roll();

            var valueResult = oneRoll.Result - twoRoll.Result;
            if(valueResult<0)
                valueResult = 0;

            return new DiceResult(oneRoll, DiceOperation.Substract, twoRoll);
        }

        public static DiceResult operator *(int dicesCount, Dice dice)
        {
            var term = new DiceTermMultiply(new DiceTermUnary(dicesCount), dice);
            return new DiceResult(term);
        }

        public static DiceResult operator *(Dice dice, int multiplier)
        {
            var roll = dice.Roll();
            return new DiceResult(roll, DiceOperation.Multiply, new DiceTermUnary(multiplier));
        }

        public static DiceResult operator +(int value, Dice dice)
        {
            var roll = dice.Roll();
            return new DiceResult(roll, DiceOperation.Add, new DiceTermUnary(value));
        }

        public static DiceResult operator -(int value, Dice dice)
        {
            var roll = dice.Roll();
            return new DiceResult(roll, DiceOperation.Add, new DiceTermUnary(value));
        }

        public static DiceResult operator *(Rank rank, Dice dice)
        {
            var term = new DiceTermMultiply(rank, dice);
            return new DiceResult(term);
        }

        public static DiceResult operator *(DiceTerm mod, Dice dice)
        {
            var term = new DiceTermMultiply(mod, dice);
            return new DiceResult(term);
        }

        public static DiceResult operator +(Rank rank, Dice dice)
        {
            var roll = dice.Roll();
            return new DiceResult(rank, DiceOperation.Add, roll);
        }

        public static DiceResult operator +(DiceTerm mod, Dice dice)
        {
            var roll = dice.Roll();
            return new DiceResult(mod, DiceOperation.Add, roll);
        }

        public static DiceResult operator -(Dice dice, Rank rank)
        {
            var roll = dice.Roll();
            return new DiceResult(dice, DiceOperation.Add, rank);
        }

        public static DiceResult operator -(DiceTerm mod, Dice dice)
        {
            var roll = dice.Roll();
            return new DiceResult(mod, DiceOperation.Substract, roll);
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

        public override int GetValue(bool isMax = false)
        {
            DiceRoll roll;

            if (isMax)
                roll = RollMax();
            else
                roll = Roll();

            return roll.Result;
        }

        public override string ToFormula()
        {
            return this.ToString();
        }

        public override string ToValue(bool isMax = false)
        {
            return GetValue(isMax).ToString();
        }
    }
}
