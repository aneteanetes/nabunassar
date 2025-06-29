using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        private List<ScreenWidget> _screenWidgets = new();

        public void AddDesktopWidget(ScreenWidget widget)
        {
            if (widget != default)
            {
                _screenWidgets.Add(widget);

                widget.Initialize();
                var uiWidget = widget.Load();
                Components.Add(widget);
                Desktop.Widgets.Add(uiWidget);
            }
        }

        public void RemoveDesktopWidgets()
        {
            _screenWidgets.Clear();
            Desktop.Widgets.Clear();
        }

        public void RemoveDesktopWidget(ScreenWidget widget)
        {
            if (widget == default)
                return;

            var uiWidget = widget.GetWidgetReference();
            Desktop.Widgets.Remove(uiWidget);
            _screenWidgets.Remove(widget);
            widget.Dispose();
        }

        public void RemoveDesktopWidgets<T>()
        {
            var pecifiedScreenWidgets = _screenWidgets.Where(x => x.GetType() == typeof(T)).ToArray();
            foreach (var specificScreenWidget in pecifiedScreenWidgets)
            {
                RemoveDesktopWidget(specificScreenWidget);
            }
        }
    }
}