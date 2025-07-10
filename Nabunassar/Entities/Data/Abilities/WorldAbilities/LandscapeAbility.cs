using Nabunassar.Components.Effects;
using Nabunassar.Entities.Data.Formulas;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal class LandscapeAbility : BaseWorldAbility
    {
        public LandscapeAbility(NabunassarGame game, AbilityModel model, Creature creature) : base(game, model, creature)
        {
        }

        public override void Cast(GameObject gameObject)
        {
            if (gameObject == default)
                return;

            var isSuccess = SkillCheck.Roll(gameObject.LandscapeRank, gameObject.LandscapeDice, this.Rank, this.Dice, this.Creature.PrimaryStats.Agility);

#warning landscape always success
            if (true)
            {
                gameObject.Entity.AddDissolve(() =>
                {
                    gameObject.Destroy();
                });
            }

#warning landscape ability
            // destory object
            // take resources
        }

        public override bool IsApplicable(GameObject gameObject)
        {
            var objType = gameObject.ObjectType;
            return objType is not ObjectType.NPC;
        }
    }
}
