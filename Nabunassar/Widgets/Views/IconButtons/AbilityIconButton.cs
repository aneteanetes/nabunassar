using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows;

namespace Nabunassar.Widgets.Views.IconButtons
{
    internal class AbilityIconButton : IconButton
    {
        private NabunassarGame _game;

        public AbilityIconButton(NabunassarGame game) : base(game.Strings["UI"]["Abilities"], null)
        {
            _game = game;
            var iconAsset = game.Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            Icon = new TextureRegion(iconAsset, new Rectangle(464, 176, 16, 16));
        }

        public static void OpenCloseAbilities(NabunassarGame game, bool isControlBtn = false)
        {
            if (!game.IsDesktopWidgetExist<AbilitiesWindow>())
            {
                game.AddDesktopWidget(new AbilitiesWindow(game));
            }
            else
            {
                game.RemoveDesktopWidgets<AbilitiesWindow>();
                if (isControlBtn)
                    game.IsMouseMoveAvailable = false;
            }
        }

        public override void OnClick()
        {
            OpenCloseAbilities(_game, true);
        }
    }
}