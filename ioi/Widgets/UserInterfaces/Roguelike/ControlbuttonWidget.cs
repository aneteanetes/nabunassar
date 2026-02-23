using FontStashSharp;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class ControlbuttonWidget : Panel
    {
        public Label _label { get; set; }

        public ControlbuttonWidget(int width, SpriteFontBase font)
        {
            Width = width;
            Height = 41;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            _label = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = $"",
                Font = font,
                TextColor = Color.DarkGray,
            };

            this.Widgets.Add(_label);
        }

        public string Text
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        public Action Click { get; set; }

        public override void OnTouchDown()
        {
            Click?.Invoke();
        }

        public override void OnMouseEntered()
        {
            if(Click!=null)
            _label.TextColor = Color.LightGray;
        }

        public override void OnMouseLeft()
        {
            if(Click!=null)
            _label.TextColor = Color.DarkGray;
        }
    }
}
