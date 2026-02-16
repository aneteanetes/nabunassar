using ioi.Entities.Data.Dices;
using ioi.Entities.Data.Rankings;
using ioi.Entities.Game.Enums;
using ioi.Entities.Struct.FixedCollections.Quads;
using ioi.Resources;

namespace ioi.Entities.Game.Stats
{
    internal class PrimaryStats : Quad<Rank>, IClonable<PrimaryStats>
    {
        public PrimaryStats Clone(PrimaryStats instance = null)
        {
            var obj = instance ?? new PrimaryStats(default);

            obj.Constitution = new Rank(Constitution.Value, Constitution.ObjectId);
            obj.Agility = new Rank(Agility.Value, Agility.ObjectId);
            obj.Intelligence = new Rank(Intelligence.Value, Intelligence.ObjectId);
            obj.Dialectics = new Rank(Dialectics.Value, Dialectics.ObjectId);

            return obj;
        }

        public PrimaryStats(IEntity entity)
        {
            Constitution = Rank.d6.Entity(entity);
            Agility = Rank.d6.Entity(entity);
            Intelligence = Rank.d6.Entity(entity);
            Dialectics = Rank.d6.Entity(entity);
        }

        public Rank Constitution { get => base.First; set => base.First = value; }

        public Rank Agility { get => base.Second; set => base.Second = value; }

        public Rank Intelligence { get => base.Third; set => base.Third = value; }

        public Rank Dialectics { get => base.Fourth; set => base.Fourth = value; }

        public Dice ConstitutionDice
        {
            get => Constitution.AsDice().Entity(GetStatDescription(nameof(Constitution)));
        }

        public Dice AgilityDice
        {
            get => Agility.AsDice().Entity(GetStatDescription(nameof(Agility)));
        }

        public Dice IntelligenceDice
        {
            get => Intelligence.AsDice().Entity(GetStatDescription(nameof(Intelligence)));
        }

        public Dice DialecticsDice
        {
            get => Dialectics.AsDice().Entity(GetStatDescription(nameof(Dialectics)));
        }

        public static IEntity GetStatDescription(string stat)
        {
            return DataBase.AddEntity(new DescribeEntity()
            {
                FormulaName = GameHost.Game.Strings["GameTexts"][stat]
            });
        }

        public int FreePoints = 0;

        public string GetName(int idx) => idx switch
        {
            0 => nameof(Constitution),
            1 => nameof(Agility),
            2 => nameof(Intelligence),
            3 => nameof(Dialectics),
            _ => null,
        };

        public void Decrease(int idx)
        {
            var rank = this[idx];
            if (rank.Value != 1)
            {
                this[idx] = new Rank(rank.Value - 1);
                FreePoints++;
            }
        }

        public void Increase(int idx)
        {
            if (FreePoints > 0)
            {
                var rank = this[idx];
                if (rank.Value < 5)
                {
                    this[idx] = new Rank(rank.Value + 1);
                    FreePoints--;
                }
            }
        }
    }
}