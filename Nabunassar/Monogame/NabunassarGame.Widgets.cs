using Geranium.Reflection;
using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        private List<ScreenWidget> _screenWidgets = new();

        public int WidgetsCount()
        {
            return _screenWidgets.Where(x => x.IsRemovable).Count();
        }

        public T AddDesktopWidget<T>(T widget)
            where T : ScreenWidget
        {
            if (widget != default)
            {
                var uiWidget = widget.Load();

                if (!Components.Contains(widget))
                    Components.Add(widget);

                if (uiWidget is Window windowWidget)
                {
                    if (widget is ScreenWidgetWindow widgetWindow)
                    {
                        if (!widgetWindow.IsCanOpen())
                        {
                            widgetWindow.Dispose();
                            return null;
                        }

                        Point? pos = widget.Position == default ?
                            null
                            : widget.Position.ToPoint();

                        if (widgetWindow.IsModal)
                        {
                            windowWidget.ShowModal(DesktopContainer, pos);
                        }
                        else
                        {
                            windowWidget.Show(DesktopContainer, pos);
                        }
                    }
                }
                else
                {
                    _screenWidgets.Add(widget);
                    Desktop.Widgets.Add(uiWidget);
                }

                return widget;
            }

            return null;
        }

        public ScreenWidget GetDesktopWidget<T>()
        {
            var widget = _screenWidgets.FirstOrDefault(x=>x.GetType() == typeof(T));
            return widget;
        }

        public bool IsDesktopWidgetExist<T>()
        {
            return _screenWidgets.Exists(x=>x.GetType() == typeof(T));
        }

        public void RemoveDesktopWidgets()
        {
            _screenWidgets.Clear();
            Desktop.Widgets.Clear();
        }

        public void RemoveDesktopWidget(ScreenWidget widget)
        {
            if (widget == default && !widget.IsRemoved)
                return;

            var uiWidget = widget.GetWidgetReference();

            if (uiWidget is Window windowWidget)
                windowWidget.Close();
            else
                Desktop.Widgets.Remove(uiWidget);

            _screenWidgets.Remove(widget);
            widget.IsRemoved = true;
            widget.Dispose();
        }

        public void RemoveDesktopWidgets<T>(int skip=0)
        {
            var specifiedScreenWidgets = _screenWidgets.Where(x => x.GetType() == typeof(T)).ToArray();

            for (int i = 0; i < specifiedScreenWidgets.Length-skip; i++)
            {
                var specificScreenWidget = specifiedScreenWidgets[i];
                RemoveDesktopWidget(specificScreenWidget);
            }
        }
    }
}