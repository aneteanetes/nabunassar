using FontStashSharp;
using ioi.Components;
using ioi.Widgets.Base;
using ioi.Widgets.UserInterfaces.Roguelike.Misc;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class InfoWidget : ScreenWidget
    {
        private DynamicSpriteFont consolas;
        private ObjectMap[] _objects;

        public InfoWidget(GameHost game, ObjectMap[] objs) : base(game)
        {
            _objects = objs;
            Position = new Vector2(14, 24);
        }

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(26);
        }

        protected override Widget CreateWidget()
        {
            var panel = new VerticalStackPanel
            {
                Width = 425,
                Height = 800,
                Background = new SolidBrush(Color.Black)
            };

            var strings = Game.Strings["Roguelike"];
            var largeBottomMargin = 20;

            var title = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(0, 15, 10, largeBottomMargin),
                TextColor = Color.Cyan,
                Text = $"{strings["objects"]}:",
                Font = consolas
            };
            panel.Widgets.Add(title);

            foreach (var obj in _objects)
            {
                panel.Widgets.Add(AddRow(obj.Entity));
            }

            return panel;
        }

        private Widget AddRow(GameEntity gameEntity)
        {
            if (gameEntity["type"].String == "item")
                return new ItemRow(Game, gameEntity, consolas);

            return default;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = ((int)Position.X);
            widget.Top = ((int)Position.Y);
        }

        public override void Dispose()
        {
        }
    }
}
