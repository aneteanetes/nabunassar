using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class TitleWidget : ScreenWidget
    {
        private static Texture2D _backgroundImage;
        private static Vector2 _position;
        private static FontSystem _font;
        private string _text;
        private Color _color;

        public TitleWidget(NabunassarGame game, string text, Vector2 position, Color color = default) : base(game)
        {
            _color = color;
            if(_color ==default)
                _color = Color.White;

            _position =position;
            _text=text ?? "[Title not found!]";
        }

        protected virtual string GetText() => _text;

        protected override void LoadContent()
        {
            if(_backgroundImage == null)
            {
                _font = Content.LoadFont(Fonts.Retron);
                _backgroundImage = Content.Load<Texture2D>("Assets/Images/Borders/panel-transparent-center-022_colored_white.png");

            }
            base.LoadContent();
        }

        protected override Widget InitWidget()
        {
            var panel = new Panel();

            var backNormal = new NinePatchRegion(_backgroundImage, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));

            var compiledFont = _font.GetFont(20);

            labelText = new Label();
            labelText.Padding = Thickness.FromString("10");
            labelText.Background = backNormal;
            labelText.FocusedBackground = backNormal;
            labelText.OverBackground = backNormal;
            labelText.Font = compiledFont;
            labelText.TextColor = _color;

            labelText.Text = GetText();
            labelText.Top = -45;

            float sexteenPixels = Game.Camera.WorldToScreen(new Vector2(16)).X;
            var textMeasure = compiledFont.MeasureString(labelText.Text).X + 20;

            labelText.Left = (int)(sexteenPixels / 2 - textMeasure / 2);

            panel.Left = ((int)_position.X);
            panel.Top = ((int)_position.Y);

            panel.Widgets.Add(labelText);

            return panel;
        }

        private Label labelText;
    }
}
