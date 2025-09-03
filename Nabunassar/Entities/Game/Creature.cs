using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Effects;
using Nabunassar.Entities.Data.Rankings;
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
            HPNow = HPMax;
        }

        public Hero HeroLink { get; private set; }

        public Archetype Archetype { get; set; }

        public Rank CreatureRank { get; set; } = Rank.d8;

        public int HPMax
        {
            /*
            d4 3
            d6 6
            d8 9
            d10 12
            d12 15
            */
            get
            {
                var unit = CreatureRank * 3;
                var value = PrimaryStats.Strength * unit;

                return value.GetValue(true);
            }
        }

        public int HPNow { get; set; }

        public PrimaryStats PrimaryStats { get; set; }

        public TypedHashSetStackable<BaseEffect> Effects { get; set; } = new();

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
