using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows
{
    internal class MinimapWindow : ScreenWidget
    {
        private Window minimapWindow;
        private FontSystem _font;
        private Texture2D _background;

        public MinimapWindow(NabunassarGame game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);
            _background = Content.Load<Texture2D>("Assets/Images/Borders/windowbackground.png");
            base.LoadContent();
        }

        private static Vector2 _windowPosition;

        protected override Widget InitWidget()
        {
            var minimap = Game.GameState.Minimap;

            if (minimapWindow == null)
            {
                var window = minimapWindow = new Window();
                window.TitleFont = _font.GetFont(19);
                window.CloseKey = Microsoft.Xna.Framework.Input.Keys.M;
                window.Closed += (s,e)=> this.Close();

                window.TouchUp += (s, e) =>
                {
                    _windowPosition = new Vector2(window.Left, window.Top);
                };

                window.Background = _background.NinePatch();//new SolidBrush("#252626".AsColor());

                var map = new Image()
                {
                    Renderable = new TextureRegion(minimap.Texture),
                    Width = ((int)minimap.OriginSize.X) / 3,
                    Height = ((int)minimap.OriginSize.Y) / 3
                };

                window.Content = map;
            }

            minimapWindow.Title = minimap.AreaName;
            this.Position = _windowPosition;

            return minimapWindow;
        }
    }
}
