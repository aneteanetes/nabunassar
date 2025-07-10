using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;

namespace Nabunassar.Entities.Game
{
    internal class Creature
    {
        public PrimaryStats PrimaryStats { get; set; } =new PrimaryStats();

        public Quad<BaseWorldAbility> WorldAbilities { get; set; } = new Quad<BaseWorldAbility>();
    }
}
