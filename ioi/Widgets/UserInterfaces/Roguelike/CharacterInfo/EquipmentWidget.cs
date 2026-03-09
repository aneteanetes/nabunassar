using FontStashSharp;
using ioi.Components;
using ioi.Struct;
using ioi.Widgets.Base;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike.CharacterInfo
{
    internal class EquipmentWidget : ScreenWidget
    {
        private DynamicSpriteFont consolas;
        private VerticalStackPanel panel;
        private Label gearscore;
        private Label skillpoints;
        private Label profpoints;

        public bool IsDirty { get; set; } = true;

        public EquipmentWidget(GameHost game) : base(game)
        {
            Position = new Vector2(14, 674);
        }

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(26);
        }

        protected override Widget CreateWidget()
        {
            panel = new VerticalStackPanel
            {
                Width = 415,
                Height = 200
            };

            var strings = Game.Strings["Roguelike"];

            var largeBottomMargin = 10;

            gearscore = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0,15,10,0),
                TextColor = "#b32e2e".AsColor(),
                Font = consolas,
                Text = $"{strings["gearscore"]}:"
            };            
            panel.Widgets.Add(gearscore);

            skillpoints = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 15, 10, 0),
                TextColor = Color.Purple,
                Font = consolas,
                Text = $"{strings["skillpoints"]}:"
            };
            panel.Widgets.Add(skillpoints);

            profpoints = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 5, 10, 0),
                TextColor = "#5c005c".AsColor(),
                Font = consolas,
                Text = $"{strings["profpoints"]}:"
            };
            panel.Widgets.Add(profpoints);

            return panel;
        }

        public override void Update(GameTime gameTime)
        {
            var strings = Game.Strings["Roguelike"];
            var entity = Game.GameState.Player.Entity;

            if (IsDirty)
            {
                gearscore.Text = $"{strings["gearscore"]}: {entity["gs"].Number}";
                skillpoints.Text = $"{strings["skillpoints"]}: {entity["sp"].Number}";
                profpoints.Text = $"{strings["profpoints"]}: {entity["pp"].Number}";

                IsDirty = false;
            }
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = ((int)Position.X);
            widget.Top = ((int)Position.Y);
        }
    }
}
