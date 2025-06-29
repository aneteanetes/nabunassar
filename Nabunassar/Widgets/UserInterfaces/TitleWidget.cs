using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class TitleWidget : ScreenWidget
    {
        private static Texture2D _backgroundImage;
        private static Vector2 _position;
        private static FontSystem _font; 

        private GameObject _gameObject;

        public TitleWidget(NabunassarGame game, GameObject gameObject, Vector2 position) : base(game)
        {
            _gameObject = gameObject;
            _position=position;
        }

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

            labelText.Text = _gameObject?.ObjectType.ToString() ?? "Объект №15";
            labelText.Top = -45;
            labelText.Left = (int)(compiledFont.MeasureString(labelText.Text).X / 2) + /* padding*/5;

            panel.Left = ((int)_position.X);
            panel.Top = ((int)_position.Y);

            panel.Widgets.Add(labelText);

            return panel;
        }

        private Label labelText;

        //public override void Update(GameTime gameTime)
        //{
        //    var uiWidget = this.UIWidget;
        //    if (uiWidget != null && labelText.ActualBounds!=default)
        //    {
        //        uiWidget.Top = (int)(_position.Y - labelText.ActualBounds.Size.Y);
        //        uiWidget.Left = (int)(_position.X + labelText.ActualBounds.Size.X / 2);
        //    }

        //    base.Update(gameTime);
        //}

        //public override void Draw(GameTime gameTime)
        //{
        //    var uiWidget = this.UIWidget;
        //    if (uiWidget != null && labelText.ActualBounds != default)
        //    {
        //        uiWidget.Top = (int)(_position.Y - labelText.ActualBounds.Size.Y);
        //        uiWidget.Left = (int)(_position.X + labelText.ActualBounds.Size.X);
        //    }

        //    base.Draw(gameTime);
        //}
    }
}
