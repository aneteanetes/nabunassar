using Nabunassar.Struct;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class SpellsRadialAction : RadialMenuAction
    {
        public SpellsRadialAction(RadialMenu menu, IEnumerable<RadialMenuAction> innerActions = null) : base(menu, Direction.RightUp, "spell", innerActions)
        {
            IsEnabled = true;
        }
    }
}
