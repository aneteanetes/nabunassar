using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Struct;
using SharpDX;
using System.Runtime.CompilerServices;

namespace Nabunassar.Entities.Data.Rankings
{
    internal class Rank
    {
        public int Value { get; }

        public bool IsDirty { get; }

        public Guid ObjectId { get; }

        public Rank(int rank, bool isDirty = false, Guid objectId = default)
        {
            Value = rank;
            IsDirty = isDirty;
            ObjectId = objectId;
        }

        public static Rank FromEntity(IEntity entity, Rank rank)
        {
            return new Rank(rank.Value, objectId: entity.ObjectId);
        }

        public Rank Entity(IEntity entity) => FromEntity(entity, this);

        public static Rank None = new Rank(0);
        public static Rank Basic = new Rank(1);
        public static Rank Advanced = new Rank(2);
        public static Rank Expert = new Rank(3);
        public static Rank Master = new Rank(4);
        public static Rank GrandMaster = new Rank(5);
        public static Rank Ultimate = new Rank(6);

        public static Rank d2 = None;
        public static Rank d4 = Basic;
        public static Rank d6 = Advanced;
        public static Rank d8 = Expert;
        public static Rank d10 = Master;
        public static Rank d12 = GrandMaster;
        public static Rank d20 = Ultimate;

        //public static DiceResult operator -(int val, Rank rank)
        //{
        //    var result = rank.Value - val;

        //    return new DiceResult(result, [new DiceModifier(val, DiceModifierType.Pure)], DiceOperation.Substract, [new DiceRoll(default, result, rank)]);
        //}

        public static DiceModifier operator -(Rank rank, int val)
        {
            var result = rank.Value - val;
            return new DiceModifier(result,val, DiceModifierType.RankModified, DiceOperation.Substract,rank);

            //return new DiceResult(result, [new DiceModifier(val, DiceModifierType.Pure)], DiceOperation.Substract, [new DiceRoll(default, result, rank)]);
        }

        //public static Rank operator +(int val, Rank rank)
        //{
        //    return new Rank(rank.Value + val, true);
        //}

        public static DiceModifier operator +(Rank rank, int val)
        {
            var result = rank.Value + val;
            return new DiceModifier(result,val, DiceModifierType.RankModified, DiceOperation.Add,rank);
            //return new DiceResult(result, [new DiceModifier(val, DiceModifierType.Pure)], DiceOperation.Substract, [new DiceRoll(default, result, rank)]);
        }

        public static DiceModifier operator *(Rank rank, int val)
        {
            var result = rank.Value * val;
            return new DiceModifier(result,val, DiceModifierType.RankModified, DiceOperation.Multiply, rank);
            //return new DiceResult(result, [new DiceModifier(val, DiceModifierType.Pure)], DiceOperation.Multiply, [new DiceRoll(default, result, rank)]);
        }

        public static implicit operator DiceModifier(Rank rank)
        {
            return new DiceModifier(rank.Value, rank.Value, DiceModifierType.Rank, DiceOperation.Unary, rank);
        }

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

        public static implicit operator DiceModifierType(Rank rank) => rank.IsDirty
                    ? DiceModifierType.RankModified
                    : DiceModifierType.Rank;
    }
}