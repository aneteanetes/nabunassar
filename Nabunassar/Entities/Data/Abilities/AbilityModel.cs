using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Entities.Struct;
using Nabunassar.Resources;

namespace Nabunassar.Entities.Data.Abilities
{
    internal class AbilityModel
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Rank AbilityRank { get; set; } = Rank.Basic;

        public Dice AbilityDice { get; set; } = Dice.d4;

        public int ItemId { get; set; }

        public bool IsCombat { get; set; }

        public QuadPosition Slot { get; set; }

        public Archetype Archetype { get; set; }

        public bool IsUsableInWorld { get; set; }

        public virtual RollResult GetFormula() => default;

        public BaseWorldAbility CreateWorldAbility(NabunassarGame game, Creature creature)
        {
            switch (Name + "Ability")
            {
                case nameof(LandscapeAbility):
                    return new LandscapeAbility(game, game.GameState.Party, creature, this);
                case nameof(RevealAbility):
                    return new RevealAbility(game, creature, this);
                default:
                    case nameof(PrayerAbility):
                    return new PrayerAbility(game, creature, this);
                    throw new NotImplementedException($"Ability {Name} instantiating is not implemented!");
            }
        }

        public string GetSlotDescription(NabunassarGame game)
        {
            return $"{game.Strings["UI"]["Slot"]}: {game.Strings["UI"][Slot.ToString()]}";
        }

        public IEntity PercentEntity()
        {
            var game = NabunassarGame.Game;
            return DataBase.AddEntity(new DescribeEntity()
            {
                FormulaName = game.Strings["Entities"]["Chance 100%"]
            });
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
