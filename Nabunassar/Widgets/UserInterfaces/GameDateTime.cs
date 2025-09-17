using FontStashSharp;
using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class GameDateTime : ScreenWidget
    {
        private FontSystem _font;
        private Label _label;

        public override bool IsRemovable => false;

        public GameDateTime(NabunassarGame game) : base(game)
        {
        }

        public override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);

            base.LoadContent();
        }

        protected override Widget CreateWidget()
        {
            _label = new Label()
            {
                Font = _font.GetFont(23),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                //Border = new SolidBrush(Color.White),
                //BorderThickness = new Myra.Graphics2D.Thickness(2),
                Padding = new Myra.Graphics2D.Thickness(3),
                Background = ScreenWidgetWindow.WindowBackground.NinePatch(),
                TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
            };

            return _label;
        }

        public override void Update(GameTime gameTime)
        {
            _label.Text = Game.GameState.Calendar.ToTimeString();
            _label.Tooltip = Game.GameState.Calendar.ToFullString();
            _label.TextColor = Game.IsGameActive ? Color.White : Color.Gray;
        }
    }
}
