using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows
{
    internal class ItemContainerWindow : ScreenWidgetWindow
    {
        private FontSystem _font;
        private Texture2D back;
        private int minimalHeight = 400;
        private GameObject _container;
        private Dictionary<Item, TextureRegion> _textures = new();

        public ItemContainerWindow(NabunassarGame game, GameObject container) : base(game)
        {
            _container = container;
        }

        protected override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);
            back = Content.Load<Texture2D>("Assets/Images/Icons/iconopacity50.png");

            foreach (var item in _container.Items(Game))
            {
                var reg = item.IconRegion;
                var texture = Content.Load<Texture2D>(item.Icon);
                var region = new TextureRegion(texture, new Rectangle(reg.X, reg.Y, reg.Width, reg.Height));


                _textures[item] = region;
            }

            base.LoadContent();
        }

        protected override Window CreateWindow()
        {
            var window = new Window();

            var grid = new Grid();
            grid.MaxHeight = 500;

            var scroll = new ScrollViewer();

            var panel = new VerticalStackPanel();
            panel.MinHeight = minimalHeight;
            panel.Width = 300;

            scroll.Height = minimalHeight;
            scroll.Content = panel;

            scroll.ShowHorizontalScrollBar = true;

            var btnPanel = new VerticalStackPanel();
            
            grid.Widgets.Add(btnPanel);
            grid.Widgets.Add(scroll);

            Grid.SetRow(scroll, 0);
            Grid.SetRow(btnPanel, 1);

            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 8.5f));
            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1.5f));

            window.Content = grid;

            FillWindow(panel, _container.Items(Game));
            FillButtons(btnPanel);

            return window;
        }

        protected void FillButtons(VerticalStackPanel panel)
        {
            var takeBtn = new Button()
            {
                Height = 25,
                Background = WindowBackground.NinePatch(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            var takeTxt = new Label()
            {
                Text = Game.Strings["UI"]["Take"],
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = _font.GetFont(24),
            };

            takeBtn.Click += TakeBtn_Click;
            takeBtn.Content = takeTxt;
            takeBtn.PressedBackground = new SolidBrush(Color.Black);


            var takeAllBtn = new Button()
            {
                Height = 25,
                Background = WindowBackground.NinePatch(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            var takeAllTxt = new Label()
            {
                Text = Game.Strings["UI"]["TakeAll"],
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = _font.GetFont(24),
            };

            takeAllBtn.Click += TakeAllBtn_Click;
            takeAllBtn.Content = takeAllTxt;
            takeAllBtn.PressedBackground = new SolidBrush(Color.Black);


            var closeBtn = new Button()
            {
                Height = 25,
                Background = WindowBackground.NinePatch(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            var closeBtnTxt = new Label()
            {
                Text = Game.Strings["UI"]["Close"],
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = _font.GetFont(24),
            };

            closeBtn.Click += (x, y) => Close();
            closeBtn.Content = closeBtnTxt;
            closeBtn.PressedBackground = new SolidBrush(Color.Black);

            panel.Widgets.Add(takeBtn);
            panel.Widgets.Add(takeAllBtn);
            panel.Widgets.Add(closeBtn);
        }

        private void TakeAllBtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("take all items");
        }

        private void TakeBtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("take specific btn");
        }

        protected override void InitWindow(Window window)
        {
            StandartWindowTitle(window, _container.GetObjectName());
        }

        private void FillWindow(VerticalStackPanel panel, List<Item> items)
        {
            foreach (var item in items)
            {
                panel.Widgets.Add(CreateItemPanel(item));
            }
        }

        private Panel CreateItemPanel(Item item)
        {
            var pan = new Panel
            {
                Height = 32
            };

            pan.OverBackground = WindowBackground.NinePatch();

            var grid = new Grid();
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1.2f));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 8.8f));

            var icon = new Image()
            {
                Renderable = _textures[item],
                Width = 32,
                Height = 32,                
            };

            var text = new Label()
            {
                Font = _font.GetFont(20),
                Text = item.GetObjectName(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Wrap = true,
            };

            grid.Widgets.Add(icon);
            grid.Widgets.Add(text);

            Grid.SetColumn(icon, 0);
            Grid.SetColumn(text, 1);

            pan.Widgets.Add(grid);

            return pan;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.GameState.Party.IsObjectNear(_container))
                this.Close();
        }
    }
}
