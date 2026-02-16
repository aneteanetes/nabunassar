using Myra.Graphics2D.UI;
using ioi.Widgets.Base;

namespace ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class ComboViewFocused : ComboView, IDisposable
    {
        private ScreenWidget _parentWidget;

        public ComboViewFocused(ScreenWidget parentWidget)
        {
            _parentWidget = parentWidget;
            parentWidget.BindWidgetBlockMouse(this.ListView);
        }

        public void Dispose()
        {
            _parentWidget.UnBindWidgetBlockMouse(this.ListView);
        }
    }
}
