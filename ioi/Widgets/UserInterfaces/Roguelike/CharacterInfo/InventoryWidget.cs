using ioi.Components;
using ioi.Entities.Struct;
using ioi.Widgets.Base;
using ioi.Widgets.UserInterfaces.Roguelike.InfoList;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike.CharacterInfo
{
    internal class InventoryWidget : ScreenWidget
    {
        private LBRBWidget sort;
        private ObjectList inventoryList;
        private EquipmentWidget equip;
        private ObjectList equipList;

        public GameEntity Entity { get; }

        public InventoryWidget(GameHost game, GameEntity host) : base(game)
        {
            Entity = host;
        }

        protected override Widget CreateWidget()
        {
            var panel = new VerticalStackPanel()
            {
                Width = 960,
                Height = 731
            };

            sort = new LBRBWidget(Game, panel.Width.Value,
                new LBRBItem("all",143),
                new LBRBItem("weapon", 569),
                new LBRBItem("helm", 109),
                new LBRBItem("chest", 475),
                new LBRBItem("boots", 62),
                new LBRBItem("acs", 168),
                new LBRBItem("potion", 813),
                new LBRBItem("scroll", 587),
                new LBRBItem("res", 98));

            sort.OnChanged += Sort_OnChanged;

            panel.Widgets.Add(sort);

            var scroll = new ScrollViewer
            {
                Width = panel.Width.Value,
                Height = panel.Height.Value - sort.Height.Value,
            };

            panel.Widgets.Add(scroll);



            return panel;
        }

        private void Sort_OnChanged(object sender, LBRBItem e)
        {
            Console.WriteLine($"tab: {e.Id}");
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = 480;
            widget.Top = 94;

            var strings = Game.Strings["Roguelike"];
            var equiped = Game.GameState.Player.Entity.GetEquiped();

            equipList = new ObjectList(Game, equiped, 700,422, DrawText.Create($"{strings["equiped"]}:", Color.White));
            Game.AddDesktopWidget(equipList, Game.MyraDesktopIngame);

            var inv = Game.GameState.Player.Entity.GetInventory();
            inventoryList = new ObjectList(Game, inv, 655,960, DrawText.Create(""));
            inventoryList.Position = new Vector2(widget.Left, widget.Top + Game.CellSize.Y * 2);
            Game.AddDesktopWidget(inventoryList, Game.MyraDesktopIngame);
            inventoryList.UIWidget.Margin = new Myra.Graphics2D.Thickness(0, Game.CellSize.Y, 0, 0);

            equip = new EquipmentWidget(Game);
            Game.AddDesktopWidget(equip, Game.MyraDesktopIngame);
        }

        public override void Update(GameTime gameTime)
        {
            sort.Update(gameTime);
        }

        public override void Dispose()
        {
            equip?.Dispose();
            equipList?.Dispose();
            inventoryList?.Dispose();
            base.Dispose();
        }
    }
}