using ioi.Struct;
using ioi.Widgets.Base;
using ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Windows;

namespace ioi.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class OpenRadialAction : RadialMenuAction
    {
        public OpenRadialAction(RadialMenu menu) : base(menu, Direction.Right, "open")
        {
        }

        public override void OnClick()
        {
            Close();
            LootWindow.Open(Menu.Game, GameObject);
            //ScreenWidgetWindow.Open(new ItemContainerWindow(Game, GameObject));
        }
    }
}