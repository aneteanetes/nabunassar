using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Windows;

namespace ioi.Widgets.Views.IconButtons
{
    internal class AbilityIconButton : IconButton
    {
        private GameHost _game;

        public AbilityIconButton(GameHost game) : base(game.Strings["UI"]["Abilities"], null)
        {
            _game = game;
            var iconAsset = game.Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            Icon = new TextureRegion(iconAsset, new Rectangle(464, 176, 16, 16));
        }

        public static void OpenCloseAbilities(GameHost game, bool isControlBtn = false)
        {
            if (!game.IsDesktopWidgetExist<AbilitiesWindow>())
            {
                game.AddDesktopWidget(new AbilitiesWindow(game));
            }
            else
            {
                game.RemoveDesktopWidgets<AbilitiesWindow>();
                if (isControlBtn)
                    game.IsMouseMoveAvailable.Value = false;
            }
        }

        public override void OnClick()
        {
            OpenCloseAbilities(_game, true);
        }
    }
}