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
    }
}
