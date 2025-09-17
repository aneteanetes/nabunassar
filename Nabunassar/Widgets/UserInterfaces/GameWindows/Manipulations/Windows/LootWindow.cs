using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Affects;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;
using Nabunassar.Widgets.Views;
using static System.Net.Mime.MediaTypeNames;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Windows
{
    internal class LootWindow : TwoSideWindow
    {
        private GameObject _container;
        private FontSystem _font;
        private FontSystem _fontRetron;
        private List<ItemView> itemViews = new();
        private ItemsPanel _itemsPanel;

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

            foreach (var item in _container.Items(Game))
            {
                itemViews.Add(new ItemView(item, Content));
            }

            base.LoadContent();
        }

        protected override Widget LeftSide() => ContainerItemsGrid();

        protected override Widget RightSide()
        {
            var inventoryWindow = new InventoryWindow(Game);
            inventoryWindow.LoadContent();
            _inventory = inventoryWindow.InnerPanel();
            GetInventoryItemPanel().DblClick = (sender, item) =>
            {
                PutBtn_Click(null, null);
            };

            _putBtn = CreateBtn(Game.Strings["UI"]["Put"]);
            _putBtn.Click += PutBtn_Click;

            _inventory.Widgets.Add(_putBtn);

            return _inventory;
        }

        private void PutBtn_Click(object sender, MyraEventArgs e)
        {
            var invPan = GetInventoryItemPanel();
            var itemView = invPan.SelectedItemView;

            Game.GameState.Party.Inventory.RemoveItem(itemView.Item);
            _container.AddItem(itemView.Item);
            _itemsPanel.AddItem(itemView);
            invPan.Remove(itemView.Item);

            invPan.ResetSelectedItem();
        }

        private ItemsPanel GetInventoryItemPanel() => _inventory.Widgets.FirstOrDefault(x => x.Is<ItemsPanel>()).As<ItemsPanel>();

        protected override void InitWindow(Window window)
        {
            StandartWindowTitle(window, _container.GetObjectName());
        }

        public Grid ContainerItemsGrid()
        {
            var grid = new Grid();
            grid.MaxHeight = 500;

            _itemsPanel = new ItemsPanel(itemViews, _font, x =>
            {
                _container.RemoveItem(x);

            },
            x =>
            {
                _itemsPanel.AddItem(itemViews.FirstOrDefault(v => v.Item == x));
            }, Pan_TouchDoubleClick);

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
        private VerticalStackPanel _inventory;
        private Button _putBtn;

        private Button CreateBtn(string text)=>new DefaultButton(text);

        protected void FillButtons(VerticalStackPanel panel)
        {
            var takeBtn = _takeBtn = CreateBtn(Game.Strings["UI"]["Take"]);
            takeBtn.Click += TakeBtn_Click;

            _takeAllBtn = CreateBtn(Game.Strings["UI"]["TakeAll"]);
            _takeAllBtn.Click += TakeAllBtn_Click;

            var closeBtn = CreateBtn(Game.Strings["UI"]["Close"]);
            closeBtn.Click += (x, y) => Close();

            panel.Widgets.Add(takeBtn);
            panel.Widgets.Add(_takeAllBtn);
            panel.Widgets.Add(closeBtn);
        }

        private void TakeAllBtn_Click(object sender, MyraEventArgs e)
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

        private void TakeBtn_Click(object sender, MyraEventArgs e)
        {
            TakeItem(_itemsPanel.SelectedItem);
            _itemsPanel.ResetSelectedItem();
        }

        private void TakeItem(Item item)
        {
            Game.GameState.Party.Inventory.AddItem(item);
            _container.RemoveItem(item);

            _itemsPanel.Remove(item);

            Game.GameState.AddMessage(DrawText.Create($"{Game.Strings["UI"]["Got"]}: {item.GetObjectName()} ({item.GetCount()})"));

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

            _itemsPanel.Update(gameTime);

            _putBtn.Enabled = Game.GameState.Party.Inventory.Items.Count() > 0
                && GetInventoryItemPanel().SelectedItem != default;

            _inventory.Widgets.FirstOrDefault(x => x.Is<WeightBar>())
                .As<WeightBar>()?
                .RecalculateWeightView();
        }

        public override void Dispose()
        {
            ItemsPanel.ResetDragAndDrop();
            base.Dispose();
        }
    }
}
