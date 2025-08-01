using Nabunassar.Struct;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
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