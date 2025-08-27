using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows;

namespace Nabunassar.Widgets.Views.IconButtons
{
    internal class InventoryIconButton : IconButton
    {
        private NabunassarGame _game;

        public InventoryIconButton(NabunassarGame game) : base(game.Strings["UI"]["Inventory"], null)
        {
            _game = game;
            var iconAsset = game.Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            Icon = new TextureRegion(iconAsset, new Rectangle(592, 64, 16, 16));
        }

        public static void OpenCloseInventory(NabunassarGame game, bool isControlBtn = false)
        {
            if (!game.IsDesktopWidgetExist<InventoryWindow>())
            {
                game.AddDesktopWidget(new InventoryWindow(game));
            }
            else
            {
                game.RemoveDesktopWidgets<InventoryWindow>();
                if (isControlBtn)
                    game.IsMouseMoveAvailable = false;
            }
        }

        public override void OnClick()
        {
            OpenCloseInventory(_game, true);
        }
    }
}