using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Effects;
using Nabunassar.Resources;
using Nabunassar.Widgets.Views.DescriptionTolltip;

namespace Nabunassar.Widgets.Views.StatusEffects
{
    internal class StatusEffectWidget : Panel
    {
        private Label Charges;
        private NabunassarGame _game;
        private BaseEffect _effect;
        private static Texture2D _pixelTexture;

        public StatusEffectWidget(NabunassarGame game, BaseEffect effect, Size size=default)
        {
            if (size == default)
            {
                size = new Size(38, 38);
            }

            var chargeSizeValue = ((int)Math.Round(size.Width * 0.42));
            var chargeSize = new Size(chargeSizeValue, chargeSizeValue);

            _game = game;
            _effect = effect;
            var texture = game.Content.LoadTexture(effect.IconPath);

            var withheight = size.Width;

            if (_pixelTexture == null)
            {
                _pixelTexture = new Texture2D(game.GraphicsDevice, 1, 1);
                _pixelTexture.SetData<Color>([new Color(Color.Black, 0.7f)]);
            }

            Widgets.Add(new Image()
            {
                Background = new TextureRegion(game.Content.LoadTexture("Assets/Images/Borders/conditionbackground.png")),
                Color = effect.IconColor,
                Width = withheight,
                Height = withheight,
                Renderable = new TextureRegion(texture),
            });

            Charges = new Label()
            {
                Background = new TextureRegion(_pixelTexture),
                Width = chargeSize.Width,
                Height = chargeSize.Height,
                Font = game.Content.LoadFont(Fonts.BitterBold).GetFont(16),
                VerticalAlignment= VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                TextAlign= FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Text = effect.Charges.ToString(),
                Visible = effect.Charges > 1
            };
            Widgets.Add(Charges);

            this.Width = withheight;
            this.Height = withheight;

            //this.Background = ScreenWidgetWindow.WindowBackground.NinePatch();
        }

        public override void OnMouseEntered()
        {
            var globalMouse = this.ToGlobal(this.LocalMousePosition.Value.ToVector2());

            var pos = new Point(((int)globalMouse.X), this.ToGlobal(new Point(Left, Top)).Y + this.Height.Value);

            _game.AddDesktopWidget(new DescriptionTooltip(_game, _effect.GetDescription(), pos));
            base.OnMouseEntered();
        }

        public override void OnMouseLeft()
        {
            _game.RemoveDesktopWidgets<DescriptionTooltip>();
            base.OnMouseLeft();
        }

        public void Update(GameTime gameTime)
        {
            Charges.Visible = _effect.Charges > 1;
            Charges.Text = _effect.Charges.ToString();
        }
    }
}
