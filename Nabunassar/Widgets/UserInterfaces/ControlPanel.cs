using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.Menu;
using Nabunassar.Widgets.UserInterfaces.GameWindows;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class ControlPanel : ScreenWidget
    {
        private Texture2D backFocused;
        private Texture2D back;
        private TextureRegion _charImage;
        private TextureRegion _skillsImage;
        private TextureRegion _foodImage;
        private TextureRegion _inventoryImage;
        private TextureRegion _journalImage;
        private TextureRegion _miniMapImage;
        private TextureRegion _globalMapImage;
        private TextureRegion _settingsImage;

        public override bool IsRemovable => false;

        public ControlPanel(NabunassarGame game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            backFocused = Content.Load<Texture2D>("Assets/Images/Borders/controlpanel_m.png");
            back = Content.Load<Texture2D>("Assets/Images/Borders/controlpanel_m.png");

            var iconAsset = Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");

            _charImage = new TextureRegion(iconAsset, new Rectangle(544, 192, 16, 16));
            _skillsImage = new TextureRegion(iconAsset, new Rectangle(464, 192, 16, 16));
            _foodImage = new TextureRegion(iconAsset, new Rectangle(528, 288, 16, 16));
            _inventoryImage = new TextureRegion(iconAsset, new Rectangle(592, 64, 16, 16));
            _journalImage = new TextureRegion(iconAsset, new Rectangle(528, 240, 16, 16));
            _miniMapImage = new TextureRegion(iconAsset, new Rectangle(768, 64, 16, 16));
            _globalMapImage = new TextureRegion(iconAsset, new Rectangle(512, 240, 16, 16));
            _settingsImage = new TextureRegion(iconAsset, new Rectangle(720, 256, 16, 16));

            base.LoadContent();
        }

        protected override Widget CreateWidget()
        {
            var panel = new HorizontalStackPanel();

            panel.Background = back.NinePatch();
            //panel.OverBackground = backFocused.NinePatch();

            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Top;

            //panel.Height = 48;

            AddButton(panel, _charImage, OpenCharacters,Game.Strings["UI"]["Characters"], 5);
            AddButton(panel, _skillsImage, OpenSkills, Game.Strings["UI"]["skill"]);
            AddButton(panel, _foodImage, OpenFood, Game.Strings["UI"]["Food"]);
            AddButton(panel, _inventoryImage, OpenInventory, Game.Strings["UI"]["Inventory"]);
            AddButton(panel, _journalImage, OpenJournal, Game.Strings["UI"]["Journal"]);
            AddButton(panel, _globalMapImage, OpenGlobalMap, Game.Strings["UI"]["GlobalMap"]);
            AddButton(panel, _miniMapImage, () => OpenCloseMiniMap(Game,true), Game.Strings["UI"]["Minimap"]);
            AddButton(panel, _settingsImage, OpenSettings, Game.Strings["UI"]["Settings"], 0,5);

            return panel;
        }

        private void OpenCharacters() { }

        private void OpenSkills() { }

        private void OpenFood() { }

        private void OpenInventory() { }

        private void OpenJournal() { }

        public static void OpenCloseMiniMap(NabunassarGame game, bool isControlBtn = false)
        {
            if (!game.IsDesktopWidgetExist<MinimapWindow>())
            {
                game.AddDesktopWidget(new MinimapWindow(game));
            }
            else
            {
                game.RemoveDesktopWidgets<MinimapWindow>();
                if(isControlBtn)
                    game.IsMouseMoveAvailable = false;
            }
        }

        private void OpenGlobalMap() { }

        private void OpenSettings()
        {
            Game.AddDesktopWidget(new MainMenu(Game,true));
            Game.ChangeGameActive();
        }

        private Image _selectedImage;

        private void AddButton(HorizontalStackPanel panel, TextureRegion image, Action click, string title, int paddingLeft=0,int paddingRight=0)
        {
            var img = new Image()
            {
                Renderable = image,
                Color = Globals.BaseColor,
                Width = 48,
                Height = 48,
                Left = paddingLeft,
                Padding = new Myra.Graphics2D.Thickness(5)
            };
            img.MouseEntered += (s, e) =>
            {
                _selectedImage = img;
                img.Color = Globals.CommonColor;

                var left = img.ToGlobal(Point.Zero).X;

                Game.AddDesktopWidget(new TitleWidget(Game, title, new Vector2(left, 100), Color.White, /*HorizontalAlignment.Center,*/null, VerticalAlignment.Top));
            };
            img.MouseLeft += (s, e) =>
            {
                img.Color = Globals.BaseColor;

                Game.RemoveDesktopWidgets<TitleWidget>(_selectedImage != img ? 1 : 0);
            };
            img.TouchDown += (s, e) => click();
            
            panel.Widgets.Add(img);

            if (paddingRight != 0)
                panel.Widgets.Add(new Panel() { Width = paddingRight });
        }
    }
}
