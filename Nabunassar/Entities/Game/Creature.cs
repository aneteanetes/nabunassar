using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;

namespace Nabunassar.Entities.Game
{
    internal class Creature : IEntity
    {
        public Creature()
        {
            ObjectId = Guid.NewGuid();
            PrimaryStats = new PrimaryStats(this);
            FormulaName = NabunassarGame.Game.Strings["Entities"][nameof(Creature)];
        }

        public Archetype Archetype { get; set; }

        public PrimaryStats PrimaryStats { get; set; }

        public Quad<BaseWorldAbility> WorldAbilities { get; set; } = new Quad<BaseWorldAbility>();

        public Quad<AbilityModel> BattleAbilities { get; set; } = new();

        public Guid ObjectId { get; set; }

        public string FormulaName {  get; set; }
    }
}
