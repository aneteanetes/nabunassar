using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Windows;

namespace ioi.Widgets.Views.IconButtons
{
    internal class InventoryIconButton : IconButton
    {
        private GameHost _game;

        public InventoryIconButton(GameHost game) : base(game.Strings["UI"]["Inventory"], null)
        {
            _game = game;
            var iconAsset = game.Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            Icon = new TextureRegion(iconAsset, new Rectangle(592, 64, 16, 16));
        }

        public static void OpenCloseInventory(GameHost game, bool isControlBtn = false)
        {
            if (!game.IsDesktopWidgetExist<InventoryWindow>())
            {
                game.AddDesktopWidget(new InventoryWindow(game));
            }
            else
            {
                game.RemoveDesktopWidgets<InventoryWindow>();
                if (isControlBtn)
                    game.IsMouseMoveAvailable.Value = false;
            }
        }

        public override void OnClick()
        {
            OpenCloseInventory(_game, true);
        }
    }
}