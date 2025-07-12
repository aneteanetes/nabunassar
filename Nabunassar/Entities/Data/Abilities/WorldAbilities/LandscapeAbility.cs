using Nabunassar.Entities.Data.Formulas;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Data.Abilities.WorldAbilities
{
    internal class LandscapeAbility : BaseWorldAbility
    {
        public LandscapeAbility(NabunassarGame game, Creature creature) : base(game, game.DataBase.GetAbility("Landscape"), creature)
        {
        }

        protected override void Execute(GameObject gameObject)
        {
            if (gameObject == default)
                return;

            var isSuccess = SkillCheck.Roll(gameObject.LandscapeRank, gameObject.LandscapeDice, this.Rank, this.Dice, this.Creature.PrimaryStats.Agility);

#warning landscape always success
            if (true)
            {
                gameObject.Destroy();
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
