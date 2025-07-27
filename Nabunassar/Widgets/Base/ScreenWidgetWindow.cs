using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Resources;

namespace Nabunassar.Widgets.Base
{
    internal abstract class ScreenWidgetWindow : ScreenWidget
    {
        protected ScreenWidgetWindow(NabunassarGame game) : base(game)
        {
        }

        public virtual bool IsModal => false;

        public virtual bool IsCloseable => true;

        protected abstract Window CreateWindow();

        protected override void LoadContent()
        {
            WindowBackground = Content.Load<Texture2D>("Assets/Images/Borders/windowbackground.png");
        }

        protected abstract void InitWindow(Window window);

        protected Window Window;
        private Vector2 _windowPosition;
        protected Texture2D WindowBackground;

        protected override Widget InitWidget()
        {
            if (Window == null)
            {
                Window = CreateWindow(); 
                Window.TouchUp += (s, e) =>
                {
                    _windowPosition = new Vector2(Window.Left, Window.Top);
                };
                Window.Closed += (s, e) => this.Close();
            }

            Window.Background = WindowBackground.NinePatch();

            if (!IsModal)
                this.Position = _windowPosition;

            InitWindow(Window);

            if (IsModal)
                Game.DisableMouseSystems();

            if (!IsCloseable)
            {
                var closeBtn = Window.TitlePanel.GetChildren(true, x => x.Is<Button>()).FirstOrDefault();
                if(closeBtn != null)
                    Window.TitlePanel.Widgets.Remove(closeBtn);
            }


            return Window;
        }

        protected void StandartWindowTitle(Window window, string titleText)
        {
            window.Title = titleText;
            window.TitleFont = Game.Content.LoadFont(Fonts.Retron).GetFont(24);

            window.TitlePanel.Background = WindowBackground.NinePatch();
            window.TitlePanel.Padding = Thickness.Zero;

            var label = window.TitlePanel.GetChildren().FirstOrDefault(x => x.GetType() == typeof(Label)).As<Label>();
            if (label != null)
                label.HorizontalAlignment = HorizontalAlignment.Center;
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
