using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Screens.Game;
using Nabunassar.Widgets.Menu;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar.Widgets.Views.IconButtons
{
    internal class SettingsIconButton : IconButton
    {
        private NabunassarGame _game;

        public override bool IsReactOnClick => false;

        public SettingsIconButton(NabunassarGame game) : base(game.Strings["UI"]["Minimap"], null)
        {
            _game = game;
            var iconAsset = game.Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            Icon = new TextureRegion(iconAsset, new Rectangle(720, 256, 16, 16));
        }

        public override void OnClick()
        {
            Open(_game);
        }

        private static void Open(NabunassarGame game)
        {
            MainGameScreen.GlobalBlurShader.Enable();
            game.AddDesktopWidget(new MainMenu(game, true));
            game.RemoveDesktopWidgets<TitleWidget>();
            game.ChangeGameActive();
        }

        public static void OpenCloseSettings(NabunassarGame game)
        {
            if (game.IsDesktopWidgetExist<MainMenu>())
            {
                MainGameScreen.GlobalBlurShader.Disable();
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