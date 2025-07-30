using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using Nabunassar.Widgets.Views;
using Nabunassar.Widgets.Views.IconButtons;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows
{
    internal class InventoryWindow : ScreenWidgetWindow
    {
        private FontSystem _bitterFont;
        private FontSystem _retronFont;
        private List<IconButton> _iconButtons;
        private List<ItemView> _inventoryItemViews;

        public InventoryWindow(NabunassarGame game) : base(game)
        {
            MakeUnique(x => false);
        }

        protected override void LoadContent()
        {
            _bitterFont = Content.LoadFont(Fonts.BitterSemiBold);
            _retronFont = Content.LoadFont(Fonts.Retron);
            _inventoryItemViews = Game.GameState.Party.Inventory.Items.Select(x=>new ItemView(x,Content)).ToList(); 
            
            _itemPanel = new ItemPanel(_inventoryItemViews, _bitterFont, null, null);

            CreateItemFilterIcons();
        }

        private ItemPanel _itemPanel;
        private Label _weightLabel;
        private HorizontalProgressBar _weightBar;
        private HorizontalStackPanel _filterPanel;

        protected override Window CreateWindow()
        {
            var window = new Window();

            var panel = new VerticalStackPanel();

            var moneySortGrid = new Grid();
            moneySortGrid.Margin = new Myra.Graphics2D.Thickness(5);
            moneySortGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            var money = new MoneyPanel(Game.GameState.Party.Money,20,true);
            money.HorizontalAlignment = HorizontalAlignment.Left;

            Grid.SetColumn(money,0);
            moneySortGrid.Widgets.Add(money);

            var sort = SortBox();
            sort.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(sort,1);
            moneySortGrid.Widgets.Add(sort);

            panel.Widgets.Add(moneySortGrid);

            _filterPanel = ItemFilter();
            _filterPanel.HorizontalAlignment = HorizontalAlignment.Center;
            _filterPanel.Margin = new Myra.Graphics2D.Thickness(10,5,10,5);
            panel.Widgets.Add(_filterPanel);

            var items = _itemPanel;
            _itemPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            items.Margin = new Myra.Graphics2D.Thickness(5);
            panel.Widgets.Add(items);

            var weight = WeightBar();
            weight.Margin = new Myra.Graphics2D.Thickness(5);
            panel.Widgets.Add(weight);

            window.Content = panel;

            return window;
        }

        private Panel WeightBar()
        {
            var panel = new Panel();

            _weightBar = new HorizontalProgressBar();

            _weightLabel = new Label()
            {
                Font = _bitterFont.GetFont(16),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            RecalculateWeightView();

            panel.Widgets.Add(_weightBar);
            panel.Widgets.Add(_weightLabel);

            return panel;
        }

        private void RecalculateWeightView()
        {
            _weightBar.Maximum = Game.GameState.Party.Weight;
            _weightBar.Value = Game.GameState.Party.Inventory.Weight;
            _weightLabel.Text = $"{_weightBar.Value}/{_weightBar.Maximum}";
        }

        private HorizontalStackPanel ItemFilter()
        {
            return new HorizontalIconPanel(Game.Content, _iconButtons, x => Game.Desktop.MousePosition.ToVector2());
        }

        private void CreateItemFilterIcons()
        {
            _iconButtons = new();

            var allTexture = Content.Load<Texture2D>("Assets/Tilesets/transparent_packed.png");
            var allIcon = new TextureRegion(allTexture, new Rectangle(720, 64, 16, 16));

            _iconButtons.Add(new InventoryFilterIconButton(Game.Strings["UI"]["All"], allIcon, _itemPanel));

            foreach (var value in typeof(ItemType).GetAllValues<ItemType>())
            {
                var info = value.GetInfo(Game);
                var texture = Content.Load<Texture2D>(info.Item1.texture);
                var icon = new TextureRegion(texture, info.Item1.ToRectangle());
                _iconButtons.Add(new InventoryFilterIconButton(info.Item2, icon, _itemPanel, x => x.ItemType == value));
            }

        }

        private ComboView SortBox()
        {
            var dropDown = new ComboView();

            dropDown.SelectedIndexChanged += DropDown_SelectedIndexChanged;
            dropDown.SelectedIndex = 0;

            dropDown.Widgets.Add(CreateSortType(Game.Strings["UI"]["SortByType"]));
            dropDown.Widgets.Add(CreateSortType(Game.Strings["UI"]["SortByAlphabet"]));
            dropDown.Widgets.Add(CreateSortType(Game.Strings["UI"]["SortByNew"]));

            dropDown.ListView.MouseEntered += ListView_MouseEntered;

            return dropDown;
        }

        private bool NOLOOSEBLOCK = false;

        private void ListView_MouseEntered(object sender, EventArgs e)
        {
            NOLOOSEBLOCK = true;
        }
        private void Window_MouseLeft(object sender, EventArgs e)
        {
            if (NOLOOSEBLOCK)
                Game.IsMouseMoveAvailable = false;
            NOLOOSEBLOCK = false;
#if DEBUG
            Console.WriteLine("InventoryWindow mouse block restored.");
#endif
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.MouseLeft += Window_MouseLeft;
        }

        private void DropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropDown = sender as ListView;
            switch (dropDown.SelectedIndex)
            {
                case 0:
                    _itemPanel.Order(x => x.ItemType);
                    break;
                case 1:
                    _itemPanel.Order(x=>x.GetObjectName());
                    break;
                case 2:
                    _itemPanel.Order(x => x.DateTimeRecived);
                    break;
                default:
                    break;
            }
        }

        private Label CreateSortType(string text)
        {
            return new Label
            {
                Text = text,
                Font = _retronFont.GetFont(20),
            };
        }

        protected override void InitWindow(Window window)
        {
            this.StandartWindowTitle(window, Game.Strings["UI"]["PartyInventory"]);
        }

        public override void Update(GameTime gameTime)
        {
            _itemPanel.Width = _filterPanel.ActualBounds.Size.X;
        }
    }
}
