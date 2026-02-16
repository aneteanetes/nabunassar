using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using ioi.Widgets.UserInterfaces.GameWindows;

namespace ioi.Widgets.Views.IconButtons
{
    internal class MinimapIconButton : IconButton
    {
        private GameHost _game;

        public override bool IsReactOnClick => false;

        public MinimapIconButton(GameHost game) : base(game.Strings["UI"]["Minimap"], null)
        {
            _game = game;
            var iconAsset = game.Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            Icon = new TextureRegion(iconAsset, new Rectangle(768, 64, 16, 16));
        }

        public static void OpenCloseMiniMap(GameHost game, bool isControlBtn = false)
        {
            if (!game.IsDesktopWidgetExist<MinimapWindow>())
            {
                game.AddDesktopWidget(new MinimapWindow(game));
            }
            else
            {
                game.RemoveDesktopWidgets<MinimapWindow>();
                if (isControlBtn)
                    game.IsMouseMoveAvailable.Value = false;
            }
        }

        public override void OnClick()
        {
            OpenCloseMiniMap(_game, true);
        }
    }
}