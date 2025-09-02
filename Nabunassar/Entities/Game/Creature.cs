using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Effects;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Game
{
    internal class Creature : IEntity
    {
        public Creature(Hero hero=default)
        {
            ObjectId = Guid.NewGuid();
            PrimaryStats = new PrimaryStats(this);
            FormulaName = NabunassarGame.Game.Strings["Entities"][nameof(Creature)];
            HeroLink = hero;
        }

        public Hero HeroLink { get; private set; }

        public Archetype Archetype { get; set; }

        public PrimaryStats PrimaryStats { get; set; }

        public TypedHashSetStackable<BaseEffect> Boons { get; set; } = new();

        public TypedHashSetStackable<BaseEffect> Conditions { get; set; } = new();

        public Quad<BaseWorldAbility> WorldAbilities { get; set; } = new Quad<BaseWorldAbility>();

        public Quad<AbilityModel> BattleAbilities { get; set; } = new();

        public Guid ObjectId { get; set; }

        public string FormulaName {  get; set; }

        public bool IsPrayerAvailable { get; set; }

        public void OnAfterBattle()
        {
            if (Archetype == Game.Enums.Archetype.Priest)
                IsPrayerAvailable = true;
        }
    }
}
