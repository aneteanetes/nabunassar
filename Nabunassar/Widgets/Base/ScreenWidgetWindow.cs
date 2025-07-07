using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace Nabunassar.Widgets.Base
{
    internal abstract class ScreenWidgetWindow : ScreenWidget
    {
        protected ScreenWidgetWindow(NabunassarGame game) : base(game)
        {
        }

        public virtual bool IsModal => false;

        protected abstract Window CreateWindow();

        protected override void LoadContent()
        {
            WindowBackground = Content.Load<Texture2D>("Assets/Images/Borders/windowbackground.png");
        }

        protected abstract void InitWindow(Window window);

        private Window _window;
        private Vector2 _windowPosition;
        protected Texture2D WindowBackground;

        protected override Widget InitWidget()
        {
            if (_window == null)
            {
                _window = CreateWindow(); 
                _window.TouchUp += (s, e) =>
                {
                    _windowPosition = new Vector2(_window.Left, _window.Top);
                };
                _window.Closed += (s, e) => this.Close();
            }

            _window.Background = WindowBackground.NinePatch();

            if (!IsModal)
                this.Position = _windowPosition;

            InitWindow(_window);

            if (IsModal)
                Game.DisableMouseSystems();

            return _window;
        }

        public override void Close()
        {
            Game.EnableSystems();
            base.Close();
        }

        public static void Open<T>(T window)
            where T : ScreenWidgetWindow
        {
            NabunassarGame.Game.AddDesktopWidget(window);
        }
    }
}
