using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct;

namespace Nabunassar.Entities.Data.Abilities
{
    internal class AbilityModel
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Rank Rank { get; set; } = Rank.Basic;

        public Dice Dice { get; set; } = Dice.d4;

        public int ItemId { get; set; }

        public bool IsCombat { get; set; }

        public QuadPosition Slot { get; set; }

        public virtual RollResult GetFormula() => default;

        public BaseWorldAbility CreateWorldAbility(NabunassarGame game, Creature creature)
        {
            switch (Name+ "Ability")
            {
                case nameof(LandscapeAbility):
                    {
                        return new LandscapeAbility(game, game.GameState.Party, creature, this);
                    }
                default:
                    return default;
            }
        }

        public AbilityModel Load(NabunassarGame game)
        {
            if (!IsCombat)
            {
                return CreateWorldAbility(game, new Creature());
            }
            else
            {
                //return combat ability
                return null;
            }
        }
    }
}
