using ioi.Entities.Game;
using ioi.Resources;

namespace ioi.Entities.Data.Abilities.WorldAbilities
{
    internal abstract class BaseWorldAbility : AbilityModel
    {
        public int Value { get; set; }

        public Creature Creature { get; private set; }

        public GameHost Game { get; private set; }

        public virtual int GetCharges()
        {
            return Creature.EnduranceNow / EnduranceCost;
        }

        public BaseWorldAbility(GameHost game, AbilityModel model, Creature creature)
        {
            Creature = creature;
            Game = game;

            var entity = GetEntity(creature,model.Name);

            AbilityRank = model.AbilityRank.Entity(entity);
            AbilityDice = model.AbilityDice.Entity(entity);

            Name = Game.Strings["AbilityNames"][model.Name];
            Description = Game.Strings["AbilityDescriptions"][model.Name];
            Icon = model.Icon;

            Slot = model.Slot;
            IsCombat = model.IsCombat;
            ItemId = model.ItemId;
            Archetype = model.Archetype;
            IsUsableInWorld = model.IsUsableInWorld;
        }

        private IEntity GetEntity(IEntity creatureEntity, string abilityNameToken)
        {
            var game = GameHost.Game;
            var strings = game.Strings.FineTuning();

            return DataBase.AddEntity(new DescribeEntity()
            {
                FormulaName = $"{strings["Entities"]["Skill"]} {strings["AbilityNames"][abilityNameToken]} {creatureEntity.FormulaName}"
            });
        }

        public abstract bool IsApplicable(GameObject gameObject);

        public virtual Result<bool> IsActive(GameObject gameObject)
        {
            if (Creature.EnduranceNow < this.EnduranceCost)
            {
                return new Result<bool>(false,$"{Game.Strings["UI"]["Not enough endurance points"]}: {this.EnduranceCost}");
            }

            return true;
        }

        public void Cast(GameObject gameObject)
        {
            if(IsApplicable(gameObject))
                Execute(gameObject);
        }

        /// <summary>
        /// Spend endurance for ability cast
        /// </summary>
        protected void SpentEndurance()
        {
            Creature.EnduranceNow -= this.EnduranceCost;
        }

        protected abstract void Execute(GameObject gameObject);
    }
}
