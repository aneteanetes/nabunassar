using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Items;
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

        public override void LoadContent()
        {
            _bitterFont = Content.LoadFont(Fonts.BitterSemiBold);
            _retronFont = Content.LoadFont(Fonts.Retron);
            _inventoryItemViews = Game.GameState.Party.Inventory.Items.Select(x=>new ItemView(x,Content)).ToList();

            _itemPanel = new ItemsPanel(_inventoryItemViews, _bitterFont, Game.GameState.Party.Inventory.RemoveItem, Game.GameState.Party.Inventory.AddItem);

            CreateItemFilterIcons();
        }

        private ItemsPanel _itemPanel;
        private HorizontalStackPanel _filterPanel;

        protected override Window CreateWindow()
        {
            var window = new Window();

            var panel = InnerPanel();

            window.Content = panel;

            return window;
        }

        private void Inventory_ItemAdded(object sender, Item e)
        {
            var viewItem = new ItemView(e, Game.Content);
            _itemPanel.AddItem(viewItem);
            _itemPanel.Refresh();
        }

        public VerticalStackPanel InnerPanel()
        {
            var panel = new VerticalStackPanel();
            Game.GameState.Party.Inventory.ItemAdded += Inventory_ItemAdded;

            var moneySortGrid = new Grid();
            moneySortGrid.Margin = new Myra.Graphics2D.Thickness(5);
            moneySortGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            var money = new MoneyPanel(Game.GameState.Party.Money, 20, true);
            money.HorizontalAlignment = HorizontalAlignment.Left;

            Grid.SetColumn(money, 0);
            moneySortGrid.Widgets.Add(money);

            var sort = SortBox();
            sort.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(sort, 1);
            moneySortGrid.Widgets.Add(sort);

            panel.Widgets.Add(moneySortGrid);

            _filterPanel = ItemFilter();
            _filterPanel.HorizontalAlignment = HorizontalAlignment.Center;
            _filterPanel.Margin = new Myra.Graphics2D.Thickness(10, 5, 10, 5);
            panel.Widgets.Add(_filterPanel);

            var items = _itemPanel;
            _itemPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            items.Margin = new Myra.Graphics2D.Thickness(5);
            panel.Widgets.Add(items);

            var weight = WeightBar();
            weight.Margin = new Myra.Graphics2D.Thickness(5);
            panel.Widgets.Add(weight);

            panel.Width = _itemPanel.Width+10;

            return panel;
        }

        private Panel WeightBar()
        {
            return new WeightBar(Game,_bitterFont);
        }

        private HorizontalStackPanel ItemFilter()
        {
            var panel = new HorizontalIconPanel(Game.Content, _iconButtons, x => Game.Desktop.MousePosition.ToVector2());

            panel.Select(_iconButtons.FirstOrDefault());

            return panel;
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
            var dropDown = new ComboViewFocused();

            var byType = CreateSortType(Game.Strings["UI"]["SortByType"]);
            var byAlpha = CreateSortType(Game.Strings["UI"]["SortByAlphabet"]);
            var byNew = CreateSortType(Game.Strings["UI"]["SortByNew"]);
            dropDown.Widgets.Add(byType);
            dropDown.Widgets.Add(byAlpha);
            dropDown.Widgets.Add(byNew);

            dropDown.SelectedIndexChanged += DropDown_SelectedIndexChanged;

            switch (Game.GameState.UIState.SelectedInventorySortIndex)
            {
                case 1:
                    dropDown.SelectedItem = byAlpha;
                    break;
                case 2:
                    dropDown.SelectedItem = byNew;
                    break;
                default:
                    dropDown.SelectedItem = byType;
                    break;
            }

            return dropDown;
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
                    _itemPanel.Order(x => x.GetObjectName());
                    break;
                case 2:
                    _itemPanel.Order(x => x.DateTimeRecived, false);
                    break;
                default:
                    break;
            }
            Game.GameState.UIState.SelectedInventorySortIndex = dropDown.SelectedIndex ?? 0;
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
            _itemPanel.Update(gameTime);
        }

        public override void Dispose()
        {
            ItemsPanel.ResetDragAndDrop();
            ControlPanel.CloseInventory();
            base.Dispose();
        }
    }
}
