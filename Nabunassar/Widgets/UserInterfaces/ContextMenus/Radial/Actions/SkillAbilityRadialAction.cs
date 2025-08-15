using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Struct;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class SkillAbilityRadialAction : RadialMenuAction
    {
        BaseWorldAbility _ability;

        internal static Dictionary<Archetype, int> ArcheTypeCounter;

        public SkillAbilityRadialAction(RadialMenu menu, BaseWorldAbility ability, string name, TextureRegion icon, IEnumerable<RadialMenuAction> innerActions = null) : base(menu, Direction.LeftUp, null, innerActions)
        {
            Name = name;
            Icon = icon;
            _ability = ability;

            Direction pos = Direction.Idle;

            switch (ability.Archetype)
            {
                case Entities.Game.Enums.Archetype.Warrior:
                    pos = Direction.LeftUp;
                    break;
                case Entities.Game.Enums.Archetype.Wizard:
                    pos = Direction.RightUp;
                    break;
                case Entities.Game.Enums.Archetype.Rogue:
                    pos = Direction.RightDown;
                    break;
                case Entities.Game.Enums.Archetype.Priest:
                    pos = Direction.LeftDown;
                    break;
                default:
                    break;
            }

            if (ArcheTypeCounter[ability.Archetype] > 0)
            {
                pos = pos.Move();
            }
            else
            {
                ArcheTypeCounter[ability.Archetype]++;
            }

            Position = pos;
        }

        public override void OnClick()
        {
            _ability.Cast(Menu.GameObject);
            base.OnClick();
            this.Close();
        }

        internal static void ResetCounters()
        {
            ArcheTypeCounter = new Dictionary<Archetype, int>()
            {
                {Archetype.Warrior, 0},
                {Archetype.Wizard, 0},
                {Archetype.Rogue, 0},
                {Archetype.Priest, 0},
            };
        }
    }
}