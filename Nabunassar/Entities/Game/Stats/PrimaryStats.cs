using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Struct;

namespace Nabunassar.Entities.Game.Stats
{
    internal class PrimaryStats : Quad<Rank>
    {
        public PrimaryStats(IEntity entity)
        {
            Strength = Rank.d6.Entity(entity);
            Agility = Rank.d6.Entity(entity);
            Intelligence = Rank.d6.Entity(entity);
            Dialectics = Rank.d6.Entity(entity);
        }

        public Rank Strength { get => base.First; set => base.First = value; }

        public Rank Agility { get => base.Second; set => base.Second = value; }

        public Rank Intelligence { get => base.Third; set => base.Third = value; }

        public Rank Dialectics { get => base.Fourth; set => base.Fourth = value; }

        public Dice StrengthDice
        {
            get => Agility.AsDice().Entity(GetStatDescription(nameof(Strength)));
        }

        public Dice AgilityDice
        {
            get => Agility.AsDice().Entity(GetStatDescription(nameof(Agility)));
        }

        public Dice IntelligenceDice
        {
            get => Agility.AsDice().Entity(GetStatDescription(nameof(Intelligence)));
        }

        public Dice DialecticsDice
        {
            get => Agility.AsDice().Entity(GetStatDescription(nameof(Dialectics)));
        }

        public static IEntity GetStatDescription(string stat)
        {
            return NabunassarGame.Game.DataBase.AddEntity(new DescribeEntity()
            {
                FormulaName = NabunassarGame.Game.Strings["GameTexts"][stat]
            });
        }

        public int FreePoints = 0;

        public string GetName(int idx) => idx switch
        {
            0 => nameof(Strength),
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