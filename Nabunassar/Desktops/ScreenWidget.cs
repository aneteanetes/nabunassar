using Myra.Graphics2D.UI;
using Nabunassar.Monogame;

namespace Nabunassar.Desktops
{
    internal abstract class ScreenWidget : ILoadable
    {
        protected NabunassarGame Game { get; private set; }
        private Widget _widget;

        public ScreenWidget(NabunassarGame game)
        {
            Game = game;
        }

        protected abstract Widget InitWidget();

        public Widget Load()
        {
            _widget = InitWidget();
            return _widget;
        }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public void Dispose()
        {
            UnloadContent();
            Game.Desktop.Root = null;
        }
    }
}
