using Geranium.Reflection;
using Myra.Graphics2D.UI;
using ioi.Widgets.Base;
using ioi.Systems.Roguelike;
using ioi.Scripting;

namespace ioi
{
    internal partial class GameHost
    {
        private List<ScreenWidget> _screenWidgets = new();
        private List<ScreenWidgetWindow> _screenWindowWidgets = new();

        public ObjectMapSystem MapSystem { get; internal set; }
        public PlayerControlSystem PlayerControlSystem { get; internal set; }
        public PathfindSystem PathfindSystem { get; set; }

        public LuaScripts Lua { get; set; }
        public SpawnSystem SpawnSystem { get; internal set; }

        public int WidgetsCount()
        {
            return _screenWidgets.Where(x => x.IsRemovable).Count();
        }

        public T AddDesktopWidget<T>(T widget, Desktop desktop=null)
            where T : ScreenWidget
        {
            if (desktop == null)
                desktop = Game.MyraDesktop;

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
                            windowWidget.ShowModal(desktop, pos);
                        }
                        else
                        {
                            windowWidget.Show(desktop, pos);
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
                    desktop.Widgets.Add(uiWidget);
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

        public void RemoveDesktopWidgets(bool isAnnihilateAll=false, Desktop desktop=null)
        {
            if (desktop == null)
                desktop = Game.MyraDesktop;

            ScreenWidget[] forRemoves = new ScreenWidget[_screenWidgets.Count];
            _screenWidgets.CopyTo(forRemoves);

            foreach (var forRemove in forRemoves.Where(x=>!x.IsRemovable))
            {
                RemoveDesktopWidget(forRemove);
            }

            if (isAnnihilateAll)
            {
                desktop.Widgets.Clear();
                var all = _screenWidgets.Concat(_screenWindowWidgets).ToArray();
                foreach (var widget in all)
                {
                    RemoveDesktopWidget(widget);
                }
                if (Game.GameState != null)
                    Game.GameState.EscapeSwitch = false;
            }
            else
            {
                var notWindowWidgets = desktop.Widgets.Where(w => w.IsNot<Window>()).ToArray();
                if (notWindowWidgets.Length > 0)
                {
                    foreach (var notWidowWidget in notWindowWidgets)
                    {
                        desktop.Widgets.Remove(notWidowWidget);
                    }
                }
            }
        }

        public void RemoveDesktopWidget(ScreenWidget widget, Desktop desktop=null)
        {
            if (desktop == null)
                desktop = Game.MyraDesktop;

            if (widget == default && !widget.IsRemoved)
                return;

            var uiWidget = widget.GetWidgetReference();

            widget.IsRemoved = true;

            if (uiWidget is Window windowWidget)
                windowWidget.Close();
            else
                desktop.Widgets.Remove(uiWidget);

            _screenWidgets.Remove(widget);
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