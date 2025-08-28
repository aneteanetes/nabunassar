using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class ComboViewFocused : ComboView
    {
        public ComboViewFocused()
        {
            this.ListView.MouseEntered += this.ListView_MouseEntered;
        }

        private void ListView_MouseEntered(object sender, MyraEventArgs e)
        {
            ScreenWidget.NOLOOSEBLOCK = true;
        }
    }
}
