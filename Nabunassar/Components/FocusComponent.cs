using Nabunassar.Entities.Game;

namespace Nabunassar.Components
{
    internal class FocusComponent
    {
        public Action<GameObject> OnFocus { get; set; }

        public Action<GameObject> OnUnfocus { get; set; }   

        public FocusComponent(Action<GameObject> onFocus, Action<GameObject> onUnfocus)
        {
            OnFocus = onFocus;
            OnUnfocus = onUnfocus;
        }
    }
}
