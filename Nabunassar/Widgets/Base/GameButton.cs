using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace Nabunassar.Widgets.Base
{
    internal class GameButton : Button
    {
        private static NinePatchRegion _backNormal;
        private static NinePatchRegion _backPressed;
        private static FontSystem _font;
        private Label _btnLabel;

        public GameButton(NabunassarGame game, string text)
        {
            if (_backNormal == null)
            {
                var backimgnorm = game.Content.Load<Texture2D>("Assets/Images/Borders/commonborder.png");
                var backimgfocus = game.Content.Load<Texture2D>("Assets/Images/Borders/commonborderpressed.png");
                _backNormal = new NinePatchRegion(backimgnorm, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));
                _backPressed = new NinePatchRegion(backimgfocus, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));
                _font = game.Content.LoadFont("Assets/Fonts/Retron2000.ttf");
            }


            MinWidth = 200;
            Height = 50;
            Background = _backNormal;
            OverBackground = _backNormal;
            PressedBackground = _backPressed;

            MouseEntered += NewGame_MouseEntered;
            MouseLeft += NewGame_MouseLeft;

            _btnLabel = new Label()
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(Color.Transparent),
                Font = _font.GetFont(28),
                TextColor = Globals.CommonColor,
                Margin = new Myra.Graphics2D.Thickness(10)
            };
            Content = _btnLabel;

            TouchDown += GameButton_Click;
        }

        private void GameButton_Click(object sender, MyraEventArgs e)
        {
            OnClick?.Invoke(sender.As<GameButton>(), e);
        }

        public string Text
        {
            get => _btnLabel.Text;
            set => _btnLabel.Text = value;
        }

        public Action<GameButton,MyraEventArgs> OnClick { get; set; }

        private void NewGame_MouseLeft(object sender, MyraEventArgs e)
        {
            sender.As<Button>().Content.As<Label>().TextColor = Globals.CommonColor;
        }

        private void NewGame_MouseEntered(object sender, MyraEventArgs e)
        {
            sender.As<Button>().Content.As<Label>().TextColor = Globals.CommonColorLight;
        }
    }
}
