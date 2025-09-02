using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Effects;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.Views.DescriptionTolltip;

namespace Nabunassar.Widgets.Views.StatusEffects
{
    internal class StatusEffectWidget : Panel
    {
        private Label Charges;
        private NabunassarGame _game;
        private BaseEffect _effect;
        private static Texture2D _pixelTexture;

        public StatusEffectWidget(NabunassarGame game, BaseEffect effect)
        {
            _game = game;
            _effect = effect;
            var texture = game.Content.LoadTexture(effect.IconPath);

            var withheight = 38;

            if (_pixelTexture == null)
            {
                _pixelTexture = new Texture2D(game.GraphicsDevice, 1, 1);
                _pixelTexture.SetData<Color>([new Color(Color.Black, 0.7f)]);
            }

            Widgets.Add(new Image()
            {
                Background = game.Content.LoadTexture("Assets/Images/Borders/conditionbackground.png").NinePatch(16),
                Color = effect.IconColor,
                Width = withheight,
                Height = withheight,
                Renderable = new TextureRegion(texture),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });

            Charges = new Label()
            {
                Background = new TextureRegion(_pixelTexture),
                Width = 16,
                Height = 16,
                Font = game.Content.LoadFont(Fonts.BitterBold).GetFont(16),
                VerticalAlignment= VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                TextAlign= FontStashSharp.RichText.TextHorizontalAlignment.Center,
                Text = effect.Charges.ToString(),
                Visible = effect.Charges > 0
            };
            Widgets.Add(Charges);

            this.Width = withheight;
            this.Height = withheight;

            //this.Background = ScreenWidgetWindow.WindowBackground.NinePatch();
        }

        public override void OnMouseEntered()
        {
            var pan = new DescriptionPanel(_game, _effect.GetDescription());
            var globalMouse = this.ToGlobal(this.LocalMousePosition.Value.ToVector2());

            var pos = new Point(((int)globalMouse.X), this.Height.Value);

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
            Charges.Visible = _effect.Charges > 0;
            Charges.Text = _effect.Charges.ToString();
        }
    }
}
