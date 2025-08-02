using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal abstract class BaseWorldAbility : AbilityModel
    {
        public int Value { get; set; }

        public Creature Creature { get; private set; }

        public NabunassarGame Game { get; private set; }

        public BaseWorldAbility(NabunassarGame game, AbilityModel model, Creature creature)
        {
            Creature = creature;
            Game = game;

            var entity = GetEntity(creature);

            Rank = Rank.Entity(entity);
            Dice = Dice.Entity(entity);

            Name = Game.Strings["AbilityNames"][model.Name];
            Description = Game.Strings["AbilityDescriptions"][model.Name];
            Icon = model.Icon;
        }

        private IEntity GetEntity(IEntity creatureEntity)
        {
            var game = NabunassarGame.Game;
            var strings = game.Strings.FineTuning();
            var landScapeAbilityModel = game.DataBase.GetAbility("Landscape");

            return game.DataBase.AddEntity(new DescribeEntity()
            {
                FormulaName = $"{strings["Entities"]["Skill"]} {strings["AbilityNames"][landScapeAbilityModel.Name]} {creatureEntity.FormulaName}"
            });
        }

        public abstract bool IsApplicable(GameObject gameObject);

        public abstract Result<bool> IsActive(GameObject gameObject);

        public void Cast(GameObject gameObject)
        {
            if(IsApplicable(gameObject))
                Execute(gameObject);
        }

        protected abstract void Execute(GameObject gameObject);
    }
}
