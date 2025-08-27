using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Struct;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class TabletSlot : HorizontalStackPanel
    {
        private static Texture2D square;

        public TabletSlot(NabunassarGame game, AbilityModel abilityModel)
        {
            if (square == null)
            {
                square = new Texture2D(game.GraphicsDevice, 6, 6);
                var colors = new Color[6 * 6];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = Color.White;
                }
                square.SetData<Color>(colors);
            }
            var renderable = new TextureRegion(square);

            for (int i = 0; i < 4; i++)
            {
                var image = new Image()
                {
                    Renderable = renderable,
                    Width = 8,
                    Height = 8,
                    Margin = new Myra.Graphics2D.Thickness(1),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Tooltip = abilityModel.GetSlotDescription(game),
                    Color = i == (int)abilityModel.Slot
                     ? Color.White
                     : Color.DarkGray
                };
                this.Widgets.Add(image);
            }
        }
    }
}