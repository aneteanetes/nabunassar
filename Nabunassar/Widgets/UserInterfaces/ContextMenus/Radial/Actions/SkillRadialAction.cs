using Nabunassar.Struct;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class SkillRadialAction : RadialMenuAction
    {
        public SkillRadialAction(RadialMenu menu, IEnumerable<RadialMenuAction> innerActions = null) : base(menu, Direction.LeftUp, "skill", innerActions)
        {
            IsEnabled = true;
        }
    }
}
