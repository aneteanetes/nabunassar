using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Struct;

namespace Nabunassar.Entities.Game.Stats
{
    internal class PrimaryStats : Quad<Rank>
    {
        public PrimaryStats()
        {
            Strength = Rank.d6;
            Agility = Rank.d6;
            Intelligence = Rank.d6;
            Dialectics = Rank.d6;
        }

        public Rank Strength { get => base.First; set => base.First = value; }

        public Rank Agility { get => base.Second; set => base.Second = value; }

        public Rank Intelligence { get => base.Third; set => base.Third = value; }

        public Rank Dialectics { get => base.Fourth; set => base.Fourth = value; }

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
            var value = this[idx];
            if (value != 1)
            {
                this[idx] = value - 1;
                FreePoints++;
            }
        }

        public void Increase(int idx)
        {
            if (FreePoints > 0)
            {
                var value = this[idx];
                if (value < 5)
                {
                    this[idx] = value + 1;
                    FreePoints--;
                }
            }
        }
    }
}