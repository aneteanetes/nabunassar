using Nabunassar.Entities.Data.Abilities.WorldAbilities;
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

        public PrimaryStats PrimaryStats { get; set; }

        public Quad<BaseWorldAbility> WorldAbilities { get; set; } = new Quad<BaseWorldAbility>();

        public Guid ObjectId { get; set; }

        public string FormulaName {  get; set; }
    }
}
