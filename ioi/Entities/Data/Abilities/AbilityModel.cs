using Microsoft.Xna.Framework.Graphics;
using ioi.Entities.Data.Abilities.WorldAbilities;
using ioi.Entities.Data.Descriptions;
using ioi.Entities.Data.Dices;
using ioi.Entities.Data.Rankings;
using ioi.Entities.Game;
using ioi.Entities.Game.Enums;
using ioi.Entities.Struct.FixedCollections;
using ioi.Entities.Struct.FixedCollections.Quads;
using ioi.Resources;

namespace ioi.Entities.Data.Abilities
{
    internal class AbilityModel
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public string SystemName => this.GetType().Name.Replace("Ability", "");

        public string Description { get; set; }

        public int EnduranceCost { get; set; } = 1;

        public Rank AbilityRank { get; set; } = Rank.Basic;

        public Dice AbilityDice { get; set; } = Dice.d4;

        public Guid ItemId { get; set; }

        public bool IsCombat { get; set; }

        public QuadPosition Slot { get; set; }

        public Archetype Archetype { get; set; }

        public bool IsUsableInWorld { get; set; }

        public virtual RollResult GetFormula() => default;

        public BaseWorldAbility CreateWorldAbility(GameHost game, Creature creature)
        {
            switch (Name + "Ability")
            {
                case nameof(LandscapeAbility):
                    return new LandscapeAbility(game, game.GameState.Party, creature, this);
                case nameof(RevealAbility):
                    return new RevealAbility(game, creature, this);
                case nameof(TeleportationAbility):
                    return new TeleportationAbility(game, creature, this);
                case nameof(PrayerAbility):
                    return new PrayerAbility(game, creature, this);
                default:
                    throw new NotImplementedException($"Ability {Name} instantiating is not implemented!");
            }
        }

        internal virtual Description GetDescriptionData() => null;

        public string GetSlotDescription(GameHost game)
        {
            return $"{game.Strings["UI"]["Slot"]}: {game.Strings["UI"][Slot.ToString()]}";
        }

        public IEntity PercentEntity()
        {
            var game = GameHost.Game;
            return DataBase.AddEntity(new DescribeEntity()
            {
                FormulaName = game.Strings["Entities"]["Chance 100%"]
            });
        }

        public AbilityModel Load(GameHost game)
        {
            if (!IsCombat)
            {
                return CreateWorldAbility(game, new Creature(Archetype.Warrior));
            }
            else
            {
                //return combat ability
                return null;
            }
        }
    }
}
