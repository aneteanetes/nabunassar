using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.Views.StatusEffects
{
    internal class StatusEffect : Panel
    {
        public StatusEffect(NabunassarGame game, string img, Color imgColor, Description description)
        {
            var texture = game.Content.LoadTexture(img);
            Widgets.Add(new Image()
            {
                Color = imgColor,
                Width = 30,
                Height = 30,
                Renderable = new TextureRegion(texture),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });

            this.Width = 32;
            this.Height = 32;

            this.Background = ScreenWidgetWindow.WindowBackground.NinePatch();
        }
    }
}
