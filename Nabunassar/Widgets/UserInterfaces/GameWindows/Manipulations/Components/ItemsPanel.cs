using FontStashSharp;
using Geranium.Reflection;
using MonoGame.Extended.Input;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.Views;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class ItemsPanel : ScrollViewer, IDisposable
    {
        private Dictionary<Item, Panel> itemPanelMap = new();
        private VerticalStackPanel _itemsPanel;
        private Panel _selectedPanel;
        private Item _selectedItem;
        private FontSystem _font;
        private List<ItemView> _itemViews = new();
        private bool _isShortView;
        private Action<Item> _onSourceDrop;

        private NabunassarGame Game => NabunassarGame.Game;

        public Action<Panel, Item> DblClick;

        public Panel SelectedPanel => _selectedPanel;
        public Item SelectedItem => _selectedItem;
        public ItemView SelectedItemView => _itemViews.FirstOrDefault(x => x.Item == _selectedItem);

        public IEnumerable<ItemView> Items => _itemViews;

        public ItemsPanel(List<ItemView> itemViews, FontSystem font, Action<Item> onSourceDrop, Action<Item> onDrop, Action<Panel, Item> dblClick = null, int width = 350, bool isShortView=false)
        {
            if (isShortView)
                width = 80;

            MinHeight = 400;
            Width = width;

            this.ShowVerticalScrollBar = true;

            _itemsPanel = new VerticalStackPanel();
            _itemsPanel.Width = width - 20;

            _isShortView = isShortView;
            _onSourceDrop = onSourceDrop;

            Content = _itemsPanel;

            _font = font;
            DblClick = dblClick;

            foreach (var itemView in itemViews)
            {
                AddItem(itemView, isShortView);
            }

            //this.TouchUp += (s, e) => ItemPanel_TouchUp(_itemsPanel, onSourceDrop,onDrop);
        }

        private void AddItem(ItemView item, bool isShortView)
        {
            var panel = CreateItemPanel(item, _font, isShortView);
            _itemViews.Add(item);
            _itemsPanel.Widgets.Add(panel);
            Refresh();
        }

        public void AddItem(ItemView item) => AddItem(item, _isShortView);

        public void Remove(Item item)
        {
            _itemsPanel.Widgets.Remove(itemPanelMap[item]);
            var itemView = _itemViews.FirstOrDefault(x => x.Item == item);
            _itemViews.Remove(itemView);
        }


        public event EventHandler<Item> ItemMouseEntered
        {
            add
            {
                itemPanelMap.ForEach(x =>
                {
                    x.Value.MouseEntered += (s, arg) => value?.Invoke(x.Value, x.Key);
                });
            }
            remove
            {
                itemPanelMap.ForEach(x =>
                {
                    x.Value.MouseEntered -= (s, arg) => value?.Invoke(x.Value, x.Key);
                });
            }
        }

        public event EventHandler<Item> ItemMouseLeft
        {
            add
            {
                itemPanelMap.ForEach(x =>
                {
                    x.Value.MouseLeft += (s, arg) => value?.Invoke(x.Value, x.Key);
                });
            }
            remove
            {
                itemPanelMap.ForEach(x =>
                {
                    x.Value.MouseLeft -= (s, arg) => value?.Invoke(x.Value, x.Key);
                });
            }
        }

        public event EventHandler<Item> ItemTouchDown
        {
            add
            {
                itemPanelMap.ForEach(x =>
                {
                    x.Value.TouchDown += (s, arg) => value?.Invoke(x.Value, x.Key);
                });
            }
            remove
            {
                itemPanelMap.ForEach(x =>
                {
                    x.Value.TouchDown -= (s, arg) => value?.Invoke(x.Value, x.Key);
                });
            }
        }        

        public Panel CreateItemPanel(ItemView itemView, FontSystem font, bool isShortView)
        {
            var size = 48;

            var container = new VerticalStackPanel();

            var pan = new Panel();
            if(!isShortView)
            {
                pan.Height = 56;
            }
            else
            {
                pan.Height = 64;
            }
            pan.BorderThickness = new Thickness(0, 0, 0, 1);
            pan.Border = new SolidBrush(Color.White);
            pan.OverBackground = ScreenWidgetWindow.WindowBackground.NinePatch();
            if (!isShortView)
                pan.HorizontalAlignment = HorizontalAlignment.Stretch;

            pan.TouchDown += (s, e) => Pan_TouchUp(pan, itemView.Item);
            //pan.TouchDown += (s, e) => Pan_TouchDown(pan, itemView);

            pan.TouchDoubleClick += (s, e) => DblClick?.Invoke(s.As<Panel>(), itemView.Item);

            var grid = new Grid()
            {
                Padding = new Thickness(6)
            };

            var icon = new Image()
            {
                Renderable = itemView.Icon,
                Width = size,
                Height = size,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Widgets.Add(icon);
            Grid.SetColumn(icon, 0);

            if (!isShortView)
            {
                grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2f));
                grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 8f));

                var textgold = new Grid()
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
                Grid.SetColumn(text, 0);
                Grid.SetRow(text,0);
                Grid.SetColumnSpan(text,2);

                var money = new MoneyPanel(itemView.Cost)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Padding = new Thickness(0, 5, 0, 0)
                };
                textgold.Widgets.Add(money);
                Grid.SetColumn(money, 1);
                Grid.SetRow(money, 1);

                grid.Widgets.Add(textgold);
                Grid.SetColumn(textgold, 1);

                AddItemDetailsLong(textgold,itemView);
            }
            else
            {
                AddItemDetailsShort(pan, itemView);
            }

            pan.Widgets.Add(grid);

            itemPanelMap[itemView.Item] = pan;

            container.Widgets.Add(pan);

            return pan;
        }

        private void AddItemDetailsLong(Grid grid, ItemView itemView)
        {
            if (itemView.Item.ItemSubtype == ItemSubtype.Tablet)
            {
                if (itemView.Item.TryGetAbility(out var ability))
                {
                    var panel = new Panel();
                    panel.HorizontalAlignment = HorizontalAlignment.Stretch;

                    var tabletSlot = new TabletSlot(Game, ability);
                    tabletSlot.VerticalAlignment = VerticalAlignment.Center;
                    panel.Widgets.Add(tabletSlot);

                    var info = ability.Archetype.GetInfo(Game);

                    var texture = info.Item1.ToTextureRegion(Game);
                    var classImage = new Image()
                    {
                        Renderable = texture,
                        Width = 20,
                        Height = 20,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tooltip = info.Item2
                    };
                    panel.Widgets.Add(classImage);

                    Grid.SetColumn(panel, 0);
                    Grid.SetRow(panel, 1);
                    grid.Widgets.Add(panel);
                }
            }
        }

        private void AddItemDetailsShort(Panel pan, ItemView itemView)
        {

            if (itemView.Item.ItemSubtype == ItemSubtype.Tablet)
            {
                if (itemView.Item.TryGetAbility(out var ability))
                {
                    var tabletSlot = new TabletSlot(Game, ability);
                    tabletSlot.HorizontalAlignment = HorizontalAlignment.Center;
                    pan.Widgets.Add(tabletSlot);
                }
            }
        }

        private static Point _mousePos;
        private static Panel _touchedPanel;
        private static ItemsPanel _touchedItemPanel;
        private static ItemView _touched;
        private static List<Widget> _touchedWidgets;

        private void Pan_TouchDown(object sender, ItemView itemView)
        {
            var state = MouseExtended.GetState();
            _mousePos = state.Position; 
            _touched = itemView;
            _touchedPanel = itemPanelMap[itemView.Item];
            _touchedItemPanel = this;
        }

        private void Pan_TouchUp(Panel sender, Item item)
        {
            if (!_isDragging)
            {
                ResetSelectedItem();

                _selectedPanel = sender;
                _selectedItem = item;

                _selectedPanel.Background = ScreenWidgetWindow.WindowBackground.NinePatch();

                ResetDragAndDrop();
            }
        }

        private void ItemPanel_TouchUp(VerticalStackPanel panel, Action<Item> onSourceDrop, Action<Item> onDrop)
        {
            if (_touchedPanel!=null)
            {
                if (panel.Widgets.Contains(_touchedPanel))
                {
                    MakePanelVisible(_touchedPanel);
                }
                else
                {
                    _touchedItemPanel._onSourceDrop?.Invoke(_touched.Item);
                    _touchedItemPanel.Remove(_touched.Item);
                    this.AddItem(_touched,_isShortView);
                    onDrop?.Invoke(_touched.Item);
                }
                ResetDragAndDrop();
            }
        }

        public static void ResetDragAndDrop()
        {
            _touchedPanel = null;
            _touched = null;
            _touchedWidgets = null;
            _isDragging = false;
            _mousePos = Point.Zero;

            NabunassarGame.Game.RemoveDesktopWidgets<ItemDragWidget>();
        }

        public void ResetSelectedItem()
        {
            if (_selectedPanel != null)
            {
                _selectedPanel.Background = default;
                _selectedItem = null;
            }

            _selectedItem = null;
            _selectedPanel = null;
        }

        private List<ItemView> _filteredItems = null;
        private Func<Item, bool> _filter;

        public void Refresh()
        {
            Filter();
        }

        public void ResetFilter()
        {
            _filter = null;
        }

        internal void Filter(Func<Item, bool> filter = null)
        {
            foreach (var kv in itemPanelMap)
            {
                kv.Value.Visible = true;
            }
            _filteredItems?.Clear();
            _filteredItems = null;

            if (filter != null)
                _filter = filter;

            if (_filter != null)
            {
                _filteredItems = [];
                foreach (var itemView in _itemViews)
                {
                    if (!_filter(itemView.Item))
                    {
                        itemPanelMap[itemView.Item].Visible = false;
                    }
                    else
                        _filteredItems.Add(itemView);
                }
            }

            if (_order != default)
            {
                _order(_filteredItems ?? _itemViews);
            }
        }

        private Action<List<ItemView>> _order = null;

        internal void Order<T>(Func<Item, T> filter, bool isAscendant=true)
        {
            _order = itemViews =>
            {
                var query = itemViews.Select(x => x.Item);

                if (isAscendant)
                    query = query.OrderBy(filter);
                else
                    query = query.OrderByDescending(filter);

                var ordered = query.ToArray();

                _itemsPanel.Widgets.Clear();
                foreach (var item in ordered)
                {
                    _itemsPanel.Widgets.Add(itemPanelMap[item]);
                }

                ResetSelectedItem();
            };
            _order(_filteredItems ?? _itemViews);
        }

        private static bool _isDragging = false;

        public void Update(GameTime gameTime)
        {
            return;
            //drag logic

            //var state = MouseExtended.GetState();
            //if (_touchedItemPanel == this && _mousePos!=Point.Zero && state.Position != _mousePos && !_isDragging)
            //{
            //    _isDragging = true;
            //    MakePanelInvisible(itemPanelMap[_touched.Item]);
            //    var drag = new ItemDragWidget(Game, _touched);
            //    Game.AddDesktopWidget(drag);
            //}
        }

        private void MakePanelInvisible(Panel panel)
        {
            panel.BorderThickness = Thickness.Zero;
            _touchedWidgets = [.. panel.Widgets];
            panel.Widgets.Clear();
        }

        private void MakePanelVisible(Panel panel)
        {
            panel.BorderThickness = new Thickness(0, 0, 0, 1);
            if (_touchedWidgets != null)
                foreach (var widget in _touchedWidgets)
                {
                    panel.Widgets.Add(widget);
                }
        }

        public void Dispose()
        {
            itemPanelMap.Clear();
        }
    }
}