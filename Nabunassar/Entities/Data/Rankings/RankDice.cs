using Nabunassar.Entities.Data.Dices;

namespace Nabunassar.Entities.Data.Rankings
{
    internal class RankDice
    {
        public Dice Dice { get; set; }

        public Rank Rank { get; set; }

        public static RankDice BaseD4 => new RankDice() { Dice = Dice.d4, Rank = Rank.Basic };

        public RankDice Entity(IEntity entity) => new RankDice()
        {
            Dice = Dice.Entity(entity),
            Rank = Rank.Entity(entity)
        };

        public override string ToString()
        {
            return $"{Rank.Value}d{Dice.Edges}";
        }
    }
}
