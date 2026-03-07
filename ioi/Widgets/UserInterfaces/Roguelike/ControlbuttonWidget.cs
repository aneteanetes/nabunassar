using FontStashSharp;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class ControlbuttonWidget : Panel
    {
        private Container hbox;

        public Label _label { get; set; }

        private Image img;
        private SpriteFontBase _font;

        public ControlbuttonWidget(int width, SpriteFontBase font)
        {
            _font = font;
            Width = width;
            Height = 41;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;


            hbox = new HorizontalStackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            _label = new Label
            {
                Text = $"",
                Font = font,
                TextColor = Color.DarkGray,
                VerticalAlignment= VerticalAlignment.Center,
                Margin=new Thickness(5,0,0,0)
            };
            hbox.Widgets.Add( _label );

            this.Widgets.Add(hbox);
        }

        public string Text
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        public void AddImage(IImage img)
        {
            hbox.Widgets.Insert(0, new Image()
            {
                Renderable = img,
                Height = 34,
                Width = 34,
                Color= Color.DarkGray,
                VerticalAlignment = VerticalAlignment.Center,
                Top=-4
            });
        }

        public void AddDelimiter(string text)
        {
            hbox.Widgets.Insert(0, new Label()
            {
                Text = text,
                Font = _font,
                TextColor = Color.DarkGray,
                VerticalAlignment = VerticalAlignment.Center
            });
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

        internal void Reset()
        {
            _label.Text = string.Empty;
            hbox.Widgets.Clear();
            hbox.Widgets.Add(_label);
        }
    }
}
