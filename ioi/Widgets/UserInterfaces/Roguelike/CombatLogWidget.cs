using FontStashSharp;
using ioi.Entities.Struct;
using ioi.Widgets.Base;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class CombatLogWidget : ScreenWidget
    {
        private ScrollViewer scrollbox;
        private VerticalStackPanel vbox;
        private DynamicSpriteFont consolas;
        private bool added;

        public CombatLogWidget(GameHost game) : base(game)
        {
        }

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(26);
        }

        protected override Widget CreateWidget()
        {
            scrollbox = new ScrollViewer
            {
                Width = 960,
                Height = 731
            };

            vbox = new VerticalStackPanel();

            scrollbox.Content = vbox;

            return scrollbox;
        }

        public override void Update(GameTime gameTime)
        {
            if (added)
            {
                if (vbox.ActualBounds.Height > scrollbox.Height)
                    scrollbox.ScrollPosition = scrollbox.ScrollMaximum;
                added = false;
            }
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = 480;
            widget.Top = 94;
        }

        internal void Clear()
        {
            vbox.Widgets.Clear();
        }

        public void AppendLine(string text)
        {
            vbox.Widgets.Add(new Label()
            {
                Text = DrawText.Create($"{DateTime.Now:HH:mm:ss} > ", Color.DarkGray).ResetColor().Append(text).ToString(),
                Wrap = true,
                TextColor = Color.DarkGray,
                Font = consolas
            });

            vbox.InvalidateMeasure();
            added = true;
        }

        internal void AppendDelimiter()
        {
            var container = new Panel()
            {
                Height = 10,
            };

            vbox.Widgets.Add(container);
        }

        internal void AppendDelimiterLine(int round)
        {
            var container = new Grid()
            {
                Height = 40,
                Width= scrollbox.Width
            };

            var brush = new SolidBrush(new Color(75, 75, 75));

            var left = new Panel()
            {
                BorderThickness = new Myra.Graphics2D.Thickness(0, 1, 0, 1),
                Border = brush,
                Height = 2,
                VerticalAlignment = VerticalAlignment.Center,
                Margin=new Myra.Graphics2D.Thickness(10,0)
            };

            var label = new Label()
            {
                Font = consolas,
                Text = $"{round} {Game.Strings["Roguelike"]["round"]}",
                TextColor = brush.Color,
                VerticalAlignment= VerticalAlignment.Center
            };

            var right = new Panel()
            {
                BorderThickness = new Myra.Graphics2D.Thickness(0, 1, 0, 1),
                Border = brush,
                Height = 2,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(10, 0)
            };

            container.ColumnsProportions.Add(new Proportion(ProportionType.Part, 8));
            container.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2));
            container.ColumnsProportions.Add(new Proportion(ProportionType.Part, 8));

            Grid.SetColumn(left, 0);
            Grid.SetColumn(label, 1);
            Grid.SetColumn(right, 2);

            container.Widgets.Add(left);
            container.Widgets.Add(label);
            container.Widgets.Add(right);

            vbox.Widgets.Add(container);
        }
    }
}
