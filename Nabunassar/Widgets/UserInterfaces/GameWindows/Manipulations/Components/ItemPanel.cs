using FontStashSharp;
using Geranium.Reflection;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.Views;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class ItemPanel : ScrollViewer
    {
        private Dictionary<Item, Panel> itemPanelMap = new();
        private VerticalStackPanel _itemsPanel;
        private Panel _selectedPanel;
        private Item _selectedItem;
        private IBrush _defaultPanelBackground;
        private List<ItemView> _itemViews;

        public Panel SelectedPanel => _selectedPanel;
        public Item SelectedItem => _selectedItem;

        public ItemPanel(List<ItemView> itemViews, FontSystem font, Action<Panel, Item> click = null, Action<Panel, Item> dblClick = null, int width=350)
        {
            MinHeight = 400;
            Width = width;

            this.ShowVerticalScrollBar = true;

            _itemsPanel = new VerticalStackPanel();
            _itemsPanel.Width = width-20;

            Content = _itemsPanel;

            _itemViews = itemViews;

            foreach (var itemView in itemViews)
            {
                _itemsPanel.Widgets.Add(CreateItemPanel(itemView, font, click, dblClick));
            }
        }

        public void Remove(Item item)
        {
            _itemsPanel.Widgets.Remove(itemPanelMap[item]);
        }

        public Panel CreateItemPanel(ItemView itemView, FontSystem font, Action<Panel, Item> click, Action<Panel, Item> dblClick)
        {
            var size = 48;
            var pan = new Panel
            {
                Height = 56,
            };
            pan.OverBackground = ScreenWidgetWindow.WindowBackground.NinePatch();
            pan.HorizontalAlignment = HorizontalAlignment.Stretch;

            pan.TouchDown += (s, e) => Pan_TouchDown(pan, itemView.Item);

            if (click != default)
                pan.TouchDown += (s, e) => click(s.As<Panel>(), itemView.Item);

            if (dblClick != default)
                pan.TouchDoubleClick += (s, e) => dblClick(s.As<Panel>(), itemView.Item);

            var grid = new Grid()
            {
                Padding = new Thickness(6)
            };
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2f));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 8f));

            var icon = new Image()
            {
                Renderable = itemView.Icon,
                Width = size,
                Height = size,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Widgets.Add(icon);
            Grid.SetColumn(icon, 0);

            var textgold = new Panel()
            {
                Height = 50
            };

            var fontSize = 18;

            Grid.SetColumnSpan(textgold, 2);

            var text = new Label()
            {
                Font = font.GetFont(fontSize),
                Text = itemView.Item.GetObjectName(),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Wrap = true
            };
            textgold.Widgets.Add(text);

            var money = new MoneyPanel(itemView.Cost)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Padding = new Thickness(0,5,0,0)
            };
            textgold.Widgets.Add(money);

            grid.Widgets.Add(textgold);
            Grid.SetColumn(textgold, 1);

            pan.Widgets.Add(grid);

            itemPanelMap[itemView.Item] = pan;

            return pan;
        }

        private void Pan_TouchDown(Panel sender, Item item)
        {
            if (_selectedPanel != null)
            {
                _selectedPanel.Background = _defaultPanelBackground;
                _selectedItem = null;
            }

            _selectedPanel = sender;
            _selectedItem = item;

            _selectedPanel.Background = ScreenWidgetWindow.WindowBackground.NinePatch();
        }

        public void ResetSelectedItem()
        {
            _selectedItem = null;
            _selectedPanel = null;
        }

        private List<ItemView> _filteredItems = null;

        public void ResetFilter()
        {
            foreach (var kv in itemPanelMap)
            {
                kv.Value.Visible = true;
            }
            _filteredItems?.Clear();
            _filteredItems = null;

            if (_order != default)
            {
                _order();
            }
        }

        internal void Filter(Func<Item, bool> filter)
        {
            ResetFilter();
            _filteredItems = [];

            foreach (var itemView in _itemViews)
            {
                if (!filter(itemView.Item))
                {
                    itemPanelMap[itemView.Item].Visible = false;
                }
                else
                    _filteredItems.Add(itemView);
            }

            if (_order != default)
            {
                _order();
            }
        }

        private Action _order = null;

        internal void Order<T>(Func<Item, T> filter)
        {
            _order = () =>
            {
                var ordered = (_filteredItems ?? _itemViews).Select(x => x.Item).OrderBy(filter);
                _itemsPanel.Widgets.Clear();
                foreach (var item in ordered)
                {
                    _itemsPanel.Widgets.Add(itemPanelMap[item]);
                }
            };
            _order();
        }
    }
}
