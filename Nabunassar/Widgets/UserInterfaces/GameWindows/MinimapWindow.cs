using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows
{
    internal class MinimapWindow : ScreenWidgetWindow
    {
        private FontSystem _font;
        private Texture2D _background;

        public MinimapWindow(NabunassarGame game) : base(game)
        {
            MakeUnique(x => false);
        }

        protected override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);
            _background = Content.Load<Texture2D>("Assets/Images/Borders/windowbackground.png");
            base.LoadContent();
        }

        private static Vector2 _windowPosition;

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

            if (Position != default)
                _windowPosition = new Vector2(Position.X - ((map.Width ?? 0)+10), Position.Y - ((map.Height ?? 0) + 40));

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
    }
}
