using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Nabunassar.Resources;

namespace Nabunassar.Widgets.Base
{
    internal abstract class ScreenWidgetWindow : ScreenWidget
    {
        protected ScreenWidgetWindow(NabunassarGame game) : base(game)
        {
        }

        public bool IsCanOpen()
        {
            if (_isUnique)
            {
                if (_uniqueWindows.ContainsKey(this.GetType()))
                    return _isUniqueFunc(this);

                _uniqueWindows[this.GetType()] = this;
                return true;
            }

            return true;
        }

        private bool _isUnique = false;
        private Func<object, bool> _isUniqueFunc;
        private static Dictionary<Type,object> _uniqueWindows = new();

        public void MakeUnique(Func<object,bool> checkUnique)
        {
            _isUnique = true;
            _isUniqueFunc = checkUnique;
            this.OnDispose += () =>
            {
                var type = this.GetType();
                if (_uniqueWindows.ContainsKey(type))
                {
                    if (_uniqueWindows[type] == this)
                        _uniqueWindows.Remove(this.GetType());
                }
            };
        }

        public virtual bool IsCloseable => true;

        protected abstract Window CreateWindow();

        protected override void LoadContent()
        {
            WindowBackground = Content.Load<Texture2D>("Assets/Images/Borders/windowbackground.png");
        }

        protected abstract void InitWindow(Window window);

        protected Window Window;
        private Vector2 _windowPosition;
        public static Texture2D WindowBackground;

        protected override Widget CreateWidget()
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
