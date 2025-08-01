using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using Nabunassar.Widgets.Views;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows
{
    internal class LootWindow : ScreenWidgetWindow
    {
        private GameObject _container;
        private FontSystem _font;
        private FontSystem _fontRetron;
        private Texture2D back;
        private int minimalHeight = 400;
        private List<ItemView> itemViews = new();
        private ItemPanel _itemsPanel;

        public LootWindow(NabunassarGame game, GameObject container) : base(game)
        {
            _container = container;
            MakeUnique(x =>
            {
                if (x is LootWindow containerWindow)
                {
                    return containerWindow._container != container;
                }

                return true;
            });
        }

        public static void Open(NabunassarGame game, GameObject container)
        {
            var window = new LootWindow(game, container);
            ScreenWidgetWindow.Open(window);
        }

        public override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.BitterSemiBold);
            _fontRetron = Content.LoadFont(Fonts.Retron);
            back = Content.Load<Texture2D>("Assets/Images/Icons/iconopacity50.png");

            foreach (var item in _container.Items(Game))
            {
                itemViews.Add(new ItemView(item, Content));
            }

            base.LoadContent();
        }

        protected override Window CreateWindow()
        {
            var window = new Window();

            var grid = new Grid();
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 4.8f));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 0.2f));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 4.8f));

            var container = ContainerItemsGrid();

            var inventoryWindow = new InventoryWindow(Game);
            inventoryWindow.LoadContent();
            var inventory = inventoryWindow.InnerPanel();

            var split = new Button()
            {
                Enabled = false,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 5
            };

            Grid.SetColumn(container, 0);
            Grid.SetColumn(split, 1);
            Grid.SetColumn(inventory, 2);

            grid.Widgets.Add(container);
            grid.Widgets.Add(split);
            grid.Widgets.Add(inventory);

            grid.Width = container.Width + 25 + inventory.Width; 

            window.Content = grid;
            window.CloseKey = Microsoft.Xna.Framework.Input.Keys.Escape;

            return window;
        }

        protected override void InitWindow(Window window)
        {
            StandartWindowTitle(window, _container.GetObjectName());
        }

        public Grid ContainerItemsGrid()
        {
            var grid = new Grid();
            grid.MaxHeight = 500;

            _itemsPanel = new ItemPanel(itemViews, _font, null, Pan_TouchDoubleClick);

            grid.Width = _itemsPanel.Width;

            var btnPanel = new VerticalStackPanel();

            grid.Widgets.Add(btnPanel);
            grid.Widgets.Add(_itemsPanel);

            Grid.SetRow(_itemsPanel, 0);
            Grid.SetRow(btnPanel, 1);

            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 8.5f));
            grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1.5f));

            FillButtons(btnPanel);


            return grid;
        }

        private Button _takeBtn;
        private Button _takeAllBtn;

        protected void FillButtons(VerticalStackPanel panel)
        {
            var takeBtn = _takeBtn = new Button()
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
                Font = _fontRetron.GetFont(24),
            };

            takeBtn.Click += TakeBtn_Click;
            takeBtn.Content = takeTxt;
            takeBtn.PressedBackground = new SolidBrush(Color.Black);


            _takeAllBtn = new Button()
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
                Font = _fontRetron.GetFont(24),
            };

            _takeAllBtn.Click += TakeAllBtn_Click;
            _takeAllBtn.Content = takeAllTxt;
            _takeAllBtn.PressedBackground = new SolidBrush(Color.Black);


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
                Font = _fontRetron.GetFont(24),
            };

            closeBtn.Click += (x, y) => Close();
            closeBtn.Content = closeBtnTxt;
            closeBtn.PressedBackground = new SolidBrush(Color.Black);

            panel.Widgets.Add(takeBtn);
            panel.Widgets.Add(_takeAllBtn);
            panel.Widgets.Add(closeBtn);
        }

        private void TakeAllBtn_Click(object sender, EventArgs e)
        {
            _itemsPanel.ResetSelectedItem();

            var items = _container.Items(Game).ToArray();
            var count = items.Length;

            for (int i = 0; i < count; i++)
            {
                TakeItem(items[i]);
            }
            CloseIfEmpty();
        }

        private void TakeBtn_Click(object sender, EventArgs e)
        {
            TakeItem(_itemsPanel.SelectedItem);
            _itemsPanel.ResetSelectedItem();
        }

        private void TakeItem(Item item)
        {
            Game.GameState.Party.Inventory.AddItem(item);
            _container.RemoveItem(item);

            _itemsPanel.Remove(item);
            CloseIfEmpty();
        }

        private void CloseIfEmpty()
        {
            if(_container.IsEmpty())
            {
                Close();
            }
        }

        private void Pan_TouchDoubleClick(Panel sender, Item item)
        {
            TakeBtn_Click(sender, null);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.GameState.Party.IsObjectNear(_container))
                this.Close();

            _takeBtn.Enabled = _itemsPanel.Items.Count() > 0;
            _takeBtn.Enabled = _itemsPanel.SelectedItem != null;

            _takeAllBtn.Enabled = _itemsPanel.Items.Count() > 0;
        }
    }
}
