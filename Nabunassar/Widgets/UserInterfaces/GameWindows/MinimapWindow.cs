using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows
{
    internal class MinimapWindow : ScreenWidgetWindow
    {
        private static Vector2 _windowPosition;

        private FontSystem _font;
        private Texture2D _background;
        private int _leftOffset;
        private int _topOffset;


        public MinimapWindow(NabunassarGame game) : base(game)
        {
            MakeUnique(x => false);
            _leftOffset = 10;
            _topOffset = 40;
        }

        public override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);
            _background = Content.Load<Texture2D>("Assets/Images/Borders/windowbackground.png");
            base.LoadContent();
        }

        protected override Window CreateWindow()
        {
            var minimap = Game.GameState.Minimap;

            var window = new Window();
            window.TitleFont = _font.GetFont(19);
            window.CloseKey = Microsoft.Xna.Framework.Input.Keys.M;
            window.Closed += (s, e) => this.Close();

            window.TouchUp += (s, e) =>
            {
                _windowPosition = new Vector2(window.Left, window.Top);
            };

            window.Background = _background.NinePatch();

            var map = new Image()
            {
                Renderable = new TextureRegion(minimap.Texture),
                Width = ((int)minimap.OriginSize.X) / 3,
                Height = ((int)minimap.OriginSize.Y) / 3
            };

            window.Content = map;

            //window.HorizontalAlignment = HorizontalAlignment.Right;
            //window.VerticalAlignment = VerticalAlignment.Bottom;

            if (Position != default)
            {                
                _windowPosition = new Vector2(Game.MyraDesktop.LayoutBounds.Width - ((map.Width ?? 0) + _leftOffset), Game.MyraDesktop.LayoutBounds.Height - ((map.Height ?? 0) + _topOffset));
            }

            return window;
        }

        protected override void InitWindow(Window window)
        {
            var minimap = Game.GameState.Minimap;

            window.Title = minimap.AreaName;
            this.Position = _windowPosition;

            if (this.Position == default)
                this.Position = new Vector2(1);
        }

        //public override void Update(GameTime gameTime)
        //{
        //    if (this.Window.HorizontalAlignment == HorizontalAlignment.Right && this.Window.ContainerBounds.Size.X > 1)
        //    {
        //        _windowPosition = new Vector2(this.Window.ContainerBounds.Size.X, this.Window.ContainerBounds.Size.Y);
        //        this.Window.HorizontalAlignment = HorizontalAlignment.Left;
        //        this.Window.VerticalAlignment = VerticalAlignment.Top;
        //        this.Position = _windowPosition;
        //        this.Window.Left = ((int)_windowPosition.X) - this.Window.Width.Value;
        //        this.Window.Top = ((int)_windowPosition.Y) - this.Window.Height.Value;
        //    }

        //    base.Update(gameTime);
        //}
    }
}