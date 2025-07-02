using Nabunassar.Struct;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class TrapsRadialAction : RadialMenuAction
    {
        public TrapsRadialAction(RadialMenu menu) : base(menu, Direction.DownRight, "trap", [
            new SearchTrapsRadialAction(menu),
            new DisarmRadialAction(menu),
            new TakeTrapRadialAction(menu)
            ])
        {
        }

        public override void OnClick()
        {
            Menu.Fullfill(InnerActions,this);
        }
    }
}