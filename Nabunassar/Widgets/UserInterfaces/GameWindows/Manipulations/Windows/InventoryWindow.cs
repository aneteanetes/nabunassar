using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows
{
    internal class InventoryWindow : ScreenWidgetWindow
    {
        public InventoryWindow(NabunassarGame game) : base(game)
        {
        }

        protected override Window CreateWindow()
        {
            var window = new Window();

            var grid = new Grid();

            

            return window;
        }

        protected override void InitWindow(Window window)
        {
            window.Title = Game.Strings["UI"]["PartyInventory"];
        }
    }
}
