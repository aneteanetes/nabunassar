using FontStashSharp;
using ioi.Entities.Struct;
using ioi.Widgets.Base;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class LogWidget : ScreenWidget
    {
        private DynamicSpriteFont consolas;
        private Label _label;

        public LogWidget(GameHost game) : base(game)
        {
        }

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(24);
        }

        protected override Widget CreateWidget()
        {
            var panel = new Panel()
            {
                Width = 1446,
                Height = 59,
                Margin=new Myra.Graphics2D.Thickness(5,0,0,0)
            };

            _label = new Label
            {
                Font = consolas,
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Widgets.Add(_label);

            return panel;
        }

        public void SetText(DrawText text)
        {
            _label.Text = text.ToString();
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = 13;
            widget.Top = 874;
        }
    }
}
