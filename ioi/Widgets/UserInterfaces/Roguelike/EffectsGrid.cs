using FontStashSharp;
using ioi.Components;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class EffectsGrid : Grid
    {
        Func<GameEntity> _entityFetcher;
        private List<LabelEffect> _cells = new();

        public EffectsGrid(Func<GameEntity> entityFetcher, SpriteFontBase font)
        {
            _entityFetcher = entityFetcher;

            var squadcellsize = 30;

            HorizontalAlignment = HorizontalAlignment.Center;
            Padding = new Myra.Graphics2D.Thickness(4, 0, 0, 0);
            Margin = new Myra.Graphics2D.Thickness(0, 5);

            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    var cell = new LabelEffect
                    {
                        Text = "",
                        TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                        TextColor = Color.LightSeaGreen,
                        Font = font,
                        Height = squadcellsize,
                        Width = squadcellsize
                    };
                    _cells.Add(cell);
                    Widgets.Add(cell);
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                }
            }

            Width = 390;
        }

        private class LabelEffect : Label
        { 
            
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
