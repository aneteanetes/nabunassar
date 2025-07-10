using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Struct;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class SkillAbilityRadialAction : RadialMenuAction
    {
        BaseWorldAbility _ability;

        public SkillAbilityRadialAction(RadialMenu menu, BaseWorldAbility ability, string name, TextureRegion icon, IEnumerable<RadialMenuAction> innerActions = null) : base(menu, Direction.LeftUp, null, innerActions)
        {
            Name = name;
            Icon = icon;
            _ability = ability;
        }

        public override void OnClick()
        {
            Menu.Close();
            _ability.Cast(Menu.GameObject);
            base.OnClick();
        }
    }
}
