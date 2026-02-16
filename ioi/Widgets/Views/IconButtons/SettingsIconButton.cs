using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using ioi.Screens.Game;
using ioi.Widgets.Menu;
using ioi.Widgets.UserInterfaces;

namespace ioi.Widgets.Views.IconButtons
{
    internal class SettingsIconButton : IconButton
    {
        private GameHost _game;

        public override bool IsReactOnClick => false;

        public SettingsIconButton(GameHost game) : base(game.Strings["UI"]["Settings"], null)
        {
            _game = game;
            var iconAsset = game.Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            Icon = new TextureRegion(iconAsset, new Rectangle(720, 256, 16, 16));
        }

        public override void OnClick()
        {
            Open(_game);
        }

        private static void Open(GameHost game)
        {
            GameController.GlobalBlurShader.Enable();
            game.AddDesktopWidget(new MainMenu(game, true));
            game.RemoveDesktopWidgets<TitleWidget>();
            game.ChangeGameActive();
        }

        public static void OpenCloseSettings(GameHost game)
        {
            if (game.IsDesktopWidgetExist<MainMenu>())
            {
                GameController.GlobalBlurShader.Disable();
                game.RemoveDesktopWidgets<MainMenu>();
                game.ChangeGameActive();
            }
            else
            {
                Open(game);
            }
        }
    }
}