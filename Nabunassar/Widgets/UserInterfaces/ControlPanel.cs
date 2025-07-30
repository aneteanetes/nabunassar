using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.Menu;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using Nabunassar.Widgets.Views.IconButtons;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class ControlPanel : ScreenWidget
    {
        private HorizontalIconPanel _iconPanel;

        public override bool IsRemovable => false;

        public ControlPanel(NabunassarGame game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            var iconAsset = Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");

            var charImage = new TextureRegion(iconAsset, new Rectangle(544, 192, 16, 16));
            var @char = new IconButton(Game.Strings["UI"]["Characters"], charImage);

            var skillsImage = new TextureRegion(iconAsset, new Rectangle(496, 192, 16, 16));
            var skills = new IconButton(Game.Strings["UI"]["skill"], skillsImage);

            var foodImage = new TextureRegion(iconAsset, new Rectangle(528, 288, 16, 16));
            var food = new IconButton(Game.Strings["UI"]["Food"], foodImage);

            var inventory = new InventoryIconButton(Game);

            var journalImage = new TextureRegion(iconAsset, new Rectangle(528, 240, 16, 16));
            var journal = new IconButton(Game.Strings["UI"]["Journal"], journalImage);

            var globalMapImage = new TextureRegion(iconAsset, new Rectangle(512, 240, 16, 16));
            var globalMap = new IconButton(Game.Strings["UI"]["GlobalMap"], globalMapImage);

            var miniMap = new MinimapIconButton(Game);
            var settings = new SettingsIconButton(Game);

            _iconPanel = new HorizontalIconPanel(Content, new List<IconButton>()
            {
                @char,
                skills,
                food,
                inventory,
                journal,
                globalMap,
                miniMap,
                settings
            }, GetTitlePosition);

            base.LoadContent();
        }

        protected override Widget CreateWidget()
        {
            _iconPanel.HorizontalAlignment = HorizontalAlignment.Center;
            _iconPanel.VerticalAlignment = VerticalAlignment.Top;

            return _iconPanel;
        }

        private Vector2 GetTitlePosition(Image image)
        {
            var imgPos = image.ToGlobal(Point.Zero);
            return new Vector2(imgPos.X, imgPos.Y + 100);
        }
    }
}
