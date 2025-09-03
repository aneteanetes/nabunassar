using Geranium.Reflection;
using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        private List<ScreenWidget> _screenWidgets = new();
        private List<ScreenWidgetWindow> _screenWindowWidgets = new();

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
                            windowWidget.ShowModal(MyraDesktop, pos);
                        }
                        else
                        {
                            windowWidget.Show(MyraDesktop, pos);
                        }
                        widget.OnAfterAddedWidget(windowWidget);

                        _screenWindowWidgets.Add(widgetWindow);
                        void screenWidgetWindowCollectionClear()
                        {
                            _screenWindowWidgets.Remove(widgetWindow);
                            widgetWindow.OnDispose -= screenWidgetWindowCollectionClear;
                        }
                        widgetWindow.OnDispose += screenWidgetWindowCollectionClear;
                    }
                }
                else
                {
                    _screenWidgets.Add(widget);
                    MyraDesktop.Widgets.Add(uiWidget);
                    widget.OnAfterAddedWidget(uiWidget);
                }

                return widget;
            }


            return null;
        }

        public ScreenWidget GetDesktopWidget<T>()
        {
            var widget = _screenWidgets.FirstOrDefault(x=>x.GetType() == typeof(T));

            if(widget==null)
                return _screenWindowWidgets.FirstOrDefault(x => x.GetType() == typeof(T));

            return widget;
        }

        public bool IsDesktopWidgetExist<T>()
        {
            return GetDesktopWidget<T>() != null;
        }

        public void RemoveDesktopWidgets()
        {
            _screenWidgets.Clear();
            var notWindowWidgets = MyraDesktop.Widgets.Where(w => w.IsNot<Window>()).ToArray();
            if (notWindowWidgets.Length>0)
            {
                foreach (var notWidowWidget in notWindowWidgets)
                {
                    MyraDesktop.Widgets.Remove(notWidowWidget);
                }
            }
        }

        public void RemoveDesktopWidget(ScreenWidget widget)
        {
            if (widget == default && !widget.IsRemoved)
                return;

            var uiWidget = widget.GetWidgetReference();

            if (uiWidget is Window windowWidget)
                windowWidget.Close();
            else
                MyraDesktop.Widgets.Remove(uiWidget);

            _screenWidgets.Remove(widget);
            widget.IsRemoved = true;
            widget.Dispose();
        }

        public void RemoveDesktopWidgets<T>(int skip=0)
        {
            var specifiedScreenWidgets = _screenWidgets.Where(x => x.GetType() == typeof(T)).ToArray();

            if(specifiedScreenWidgets.Length==0)
                specifiedScreenWidgets = _screenWindowWidgets.Where(x => x.GetType() == typeof(T)).ToArray();

            for (int i = 0; i < specifiedScreenWidgets.Length-skip; i++)
            {
                var specificScreenWidget = specifiedScreenWidgets[i];
                RemoveDesktopWidget(specificScreenWidget);
            }
        }
    }
}