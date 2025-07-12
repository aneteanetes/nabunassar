using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal abstract class BaseWorldAbility : AbilityModel
    {
        public int Value { get; set; }

        public Rank Rank { get; set; }

        public Dice Dice { get; set; }

        public string Description { get; set; }

        public Creature Creature { get; private set; }

        public NabunassarGame Game { get; private set; }

        public BaseWorldAbility(NabunassarGame game, AbilityModel model, Creature creature)
        {
            Creature = creature;
            Game = game;

            Name = Game.Strings["AbilityNames"][model.Name];
            Description = Game.Strings["AbilityDescriptions"][model.Name];
            Icon = model.Icon;
        }

        public abstract bool IsApplicable(GameObject gameObject);

        public void Cast(GameObject gameObject)
        {
            if(IsApplicable(gameObject))
                Execute(gameObject);
        }

        protected abstract void Execute(GameObject gameObject);
    }
}
