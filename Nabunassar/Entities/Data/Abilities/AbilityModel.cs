using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;

namespace Nabunassar.Entities.Data.Abilities
{
    internal class AbilityModel
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Rank Rank { get; set; } = Rank.Basic;

        public Dice Dice { get; set; } = Dice.d4;
    }
}
