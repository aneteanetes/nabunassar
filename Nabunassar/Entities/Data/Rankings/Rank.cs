using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Data.Dices;

namespace Nabunassar.Entities.Data.Rankings
{
    internal readonly struct Rank
    {
        public readonly int Value { get; }

        public readonly bool IsDirty { get; }

        public readonly Description Descripted { get; }

        public Rank(int rank, bool isDirty = false, Description description = null)
        {
            Value = rank;
            IsDirty = isDirty;
        }

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

        public Rank Description(Description description)
        {
            return new Rank(Value, IsDirty, description);
        }

        public static Rank operator -(int val, Rank rank)
        {
            return new Rank(rank.Value - val, true);
        }

        public static Rank operator -(Rank rank, int val)
        {
            return new Rank(rank.Value - val, true);
        }

        public static Rank operator +(int val, Rank rank)
        {
            return new Rank(rank.Value + val, true);
        }

        public static Rank operator +(Rank rank, int val)
        {
            return new Rank(rank.Value + val, true);
        }

        public static Rank operator *(Rank rank, int val)
        {
            return new Rank(rank.Value * val, true);
        }


        public static implicit operator Rank(Dice dice) => dice.Edges switch
        {
            4 => Rank.Basic,
            6 => Rank.Advanced,
            8 => Rank.Expert,
            10 => Rank.Master,
            12 => Rank.GrandMaster,
            20 => Rank.Ultimate,
            _ => Rank.None,
        };

        public static implicit operator Dice(Rank rank) => rank.Value switch
        {
            1 => Dice.d4,
            2 => Dice.d6,
            3 => Dice.d8,
            4 => Dice.d10,
            5 => Dice.d12,
            6 => Dice.d20,
            _ => new Dice(0),
        };

        public static implicit operator int(Rank rank) => rank.Value;

        public static implicit operator DiceModifierType(Rank rank) => rank.IsDirty
                    ? DiceModifierType.RankModified
                    : DiceModifierType.Rank;
    }
}