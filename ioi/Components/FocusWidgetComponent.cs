using ioi.Entities.Game;
using ioi.Widgets.Base;
using ioi.Widgets.UserInterfaces;

namespace ioi.Components
{
    internal class FocusWidgetComponent
    {
        public Func<FocusEvent, TitleWidget> WidgetFactory { get; private set; }

        public GameObject GameObject { get; private set; }

        public ScreenWidget CurrentScreenWidget { get; set; }

        public FocusWidgetComponent(GameObject gameObject, Func<FocusEvent, TitleWidget> widgetFactory)
        {
            GameObject = gameObject;
            WidgetFactory = widgetFactory;
        }

        public Action<GameObject> OnFocus { get; set; }

        public Action<GameObject> OnUnfocus { get; set; }
    }
}