using Nabunassar.Entities.Game;
using Nabunassar.Struct;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Informations;

namespace Nabunassar.Widgets.UserInterfaces.ContextMenus.Radial.Actions
{
    internal class InformationRadialAction : RadialMenuAction
    {
        public InformationRadialAction(RadialMenu menu) : base(menu, Direction.Up,"info")
        {
            IsEnabled = true;
        }

        public override void OnClick()
        {
            Menu.Close();
            InformationWindow.Open(Game, this.Menu.GameObject);
        }
    }
}
