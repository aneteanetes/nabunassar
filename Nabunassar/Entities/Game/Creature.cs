using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.ComplexValues;
using Nabunassar.Entities.Data.Effects;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Entities.Game.Stats;
using Nabunassar.Entities.Struct.FixedCollections.Quads;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Game
{
    internal class Creature : GameObject, IEntity, IClonable<Creature>
    {
        public virtual Creature Clone(Creature instance = null)
        {
            var obj = instance ?? new Creature(Archetype);

            Clone(obj as GameObject);

            obj.PrimaryStats = PrimaryStats.Clone();

            return obj;
        }

        public Creature(Archetype archetype, Hero hero = default)
        {
            ObjectId = Guid.NewGuid();
            PrimaryStats = new PrimaryStats(this);
            FormulaName = NabunassarGame.Game.Strings["Entities"][nameof(Creature)];
            HeroLink = hero;
            HPNow = HPMax;
            EnduranceNow = EnduranceMax;
            ArmorClassBase = NabunassarGame.Game.DataBase.GetFromDictionary<int>("Data/Formulas/BaseArmorClassByArchetype.json", archetype.ToString());
            ArmorClass = new ComplexValue<int>();
            ArmorClass.AddValue(ArmorClassBase, ComplexValueType.Base);
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
                var value = PrimaryStats.Constitution * unit;

                return value.GetValue(true);
            }
        }

        public int HPNow { get; set; }

        public int HPNowPercent => (HPNow / HPMax) * 100;

        public int EnduranceMax
        {
            get
            {

                var value = PrimaryStats.Constitution * CreatureRank;
                return value.GetValue(true);
            }
        }

        private int _enduranceNow;
        public int EnduranceNow
        {
            get => _enduranceNow;
            set => _enduranceNow = Math.Clamp(value, 0, EnduranceMax);
        }

        public PrimaryStats PrimaryStats { get; set; }

        public TypedHashSetStackable<BaseEffect> Effects { get; set; } = new();

        public Quad<BaseWorldAbility> WorldAbilities { get; set; } = new Quad<BaseWorldAbility>();

        public Quad<AbilityModel> BattleAbilities { get; set; } = new();

        public string FormulaName {  get; set; }

        public bool IsPrayerAvailable { get; set; }

        public ComplexValue<int> ArmorClass { get; internal set; } = new();

        public int ArmorClassBase { get; }

        public int WillPoints { get; set; }

        public int ArmorClassCD => ((int)Math.Ceiling(ArmorClass.GetValueExcept(ComplexValueType.Base) / 2d));

        public void OnAfterBattle()
        {
            if (Archetype == Game.Enums.Archetype.Priest)
                IsPrayerAvailable = true;
        }
        
        public HPWounds BattlerWounds()
        {
            var percent = (HPNow / HPMax) * 100;
            var wounds = NabunassarGame.Game.DataBase.Get<Dictionary<HPWounds, int>>("Data/HPWounds/WoundPercentage.json");
            var wound = wounds.FirstOrDefault(x => x.Value <= percent);

            return wound.Key;
        }
    }
}
