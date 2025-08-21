using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Dices.Terms;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;
using SharpDX;
using System.Runtime.CompilerServices;

namespace Nabunassar.Entities.Data.Rankings
{
    internal class Rank : DiceTerm
    {
        public int Value { get; }

        public Guid ObjectId { get; }

        public Rank(int rank, Guid objectId = default)
        {
            Value = rank;
            ObjectId = objectId;
        }

        public static Rank FromEntity(IEntity entity, Rank rank)
        {
            return new Rank(rank.Value, objectId: entity.ObjectId);
        }

        public Rank Entity(IEntity entity) => FromEntity(entity, this);

        /// <summary>
        /// 0
        /// </summary>
        public static Rank None = new Rank(0);
        /// <summary>
        /// 1
        /// </summary>
        public static Rank Basic = new Rank(1);
        /// <summary>
        /// 2
        /// </summary>
        public static Rank Advanced = new Rank(2);
        /// <summary>
        /// 3
        /// </summary>
        public static Rank Expert = new Rank(3);
        /// <summary>
        /// 4
        /// </summary>
        public static Rank Master = new Rank(4);
        /// <summary>
        /// 5
        /// </summary>
        public static Rank GrandMaster = new Rank(5);
        /// <summary>
        /// 6
        /// </summary>
        public static Rank Ultimate = new Rank(6);

        /// <summary>
        /// 0
        /// </summary>
        public static Rank d2 = None;
        /// <summary>
        /// 1
        /// </summary>
        public static Rank d4 = Basic;
        /// <summary>
        /// 2
        /// </summary>
        public static Rank d6 = Advanced;
        /// <summary>
        /// 3
        /// </summary>
        public static Rank d8 = Expert;
        /// <summary>
        /// 4
        /// </summary>
        public static Rank d10 = Master;
        /// <summary>
        /// 5
        /// </summary>
        public static Rank d12 = GrandMaster;
        /// <summary>
        /// 6
        /// </summary>
        public static Rank d20 = Ultimate;

        //public static DiceResult operator -(int val, Rank rank)
        //{
        //    var result = rank.Value - val;

        //    return new DiceResult(result, [new DiceModifier(val, DiceModifierType.Pure)], DiceOperation.Substract, [new DiceRoll(default, result, rank)]);
        //}

        public static DiceTerm operator -(Rank rank, int val)
        {
            return new DiceTermOperation(DiceOperation.Substract, rank, new DiceTermUnary(val));
        }

        //public static Rank operator +(int val, Rank rank)
        //{
        //    return new Rank(rank.Value + val, true);
        //}

        public static DiceTerm operator +(Rank rank, int val)
        {
            return new DiceTermOperation(DiceOperation.Add, rank, new DiceTermUnary(val));
        }

        public static DiceTerm operator *(Rank rank, int val)
        {
            return new DiceTermOperation(DiceOperation.Multiply, rank, new DiceTermUnary(val));
        }

        //public static implicit operator DiceModifier(Rank rank)
        //{
        //    return new DiceModifier(rank.Value, rank.Value, DiceModifierType.Rank, DiceOperation.Unary, rank);
        //}

        public Dice AsDice() => (Dice)this;

        public static explicit operator Rank(Dice dice) => dice.Edges switch
        {
            4 => Rank.Basic,
            6 => Rank.Advanced,
            8 => Rank.Expert,
            10 => Rank.Master,
            12 => Rank.GrandMaster,
            20 => Rank.Ultimate,
            _ => Rank.None,
        };

        public static explicit operator Dice(Rank rank) => rank.Value switch
        {
            1 => Dice.d4,
            2 => Dice.d6,
            3 => Dice.d8,
            4 => Dice.d10,
            5 => Dice.d12,
            6 => Dice.d20,
            _ => new Dice(0),
        };

        public string GetName(NabunassarGame game) => Value switch
        {
            1 => (string)game.Strings["Enums/RankNames"][nameof(Rank.Basic)],
            2 => (string)game.Strings["Enums/RankNames"][nameof(Rank.Advanced)],
            3 => (string)game.Strings["Enums/RankNames"][nameof(Rank.Expert)],
            4 => (string)game.Strings["Enums/RankNames"][nameof(Rank.Master)],
            5 => (string)game.Strings["Enums/RankNames"][nameof(Rank.GrandMaster)],
            6 => (string)game.Strings["Enums/RankNames"][nameof(Rank.Ultimate)],
            _ => (string)game.Strings["Enums/RankNames"][nameof(Rank.None)],
        };

        public override int GetValue(bool isMax = false)
        {
            return this.Value;
        }

        public override string ToFormula()
        {
            var game = NabunassarGame.Game;
            var entity = DataBase.GetEntity(ObjectId);

            var desc = "";
            if (game != null)
            {
                desc = $" ({game.Strings["GameTexts"][nameof(Rank)]} {entity.FormulaName})";
            }

            return $"{Value}{desc}";
        }

        public override string ToValue(bool isMax = false)
        {
            return Value.ToString();
        }
    }
}