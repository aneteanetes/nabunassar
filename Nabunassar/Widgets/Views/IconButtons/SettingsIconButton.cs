using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Widgets.Menu;

namespace Nabunassar.Widgets.Views.IconButtons
{
    internal class SettingsIconButton : IconButton
    {
        private NabunassarGame _game;

        public SettingsIconButton(NabunassarGame game) : base(game.Strings["UI"]["Minimap"], null)
        {
            _game = game;
            var iconAsset = game.Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            Icon = new TextureRegion(iconAsset, new Rectangle(720, 256, 16, 16));
        }

        public override void OnClick()
        {
            _game.AddDesktopWidget(new MainMenu(_game, true));
            _game.ChangeGameActive();
        }
    }
}