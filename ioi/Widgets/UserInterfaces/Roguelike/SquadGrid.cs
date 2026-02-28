using FontStashSharp;
using ioi.Components;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class SquadGrid : Grid
    {
        Func<GameEntity> _entityFetcher;
        List<LabelWithEntity> _cells = new();

        public SquadGrid(Func<GameEntity> entityFetcher, SpriteFontBase font)
        {
            this._entityFetcher = entityFetcher;
            HorizontalAlignment = HorizontalAlignment.Center;

            var squadcellsize = 60;

            var entity = entityFetcher();

            var c = 0;
            foreach (var member in entity.Squad.Members)
            {
                var cell = new LabelWithEntity(member)
                {
                    Text = member["icon"].String,
                    TextColor = member.Color("color"),
                    TextAlign = FontStashSharp.RichText.TextHorizontalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Myra.Graphics2D.Thickness(0, 10),
                    Margin = new Myra.Graphics2D.Thickness(5),
                    Font = font,
                    Border = new SolidBrush(entity == member ? Color.IndianRed : Color.DarkGoldenrod),
                    BorderThickness = new Myra.Graphics2D.Thickness(1),
                    Height = squadcellsize,
                    Width = squadcellsize
                };

                _cells.Add(cell);
                Widgets.Add(cell);
                Grid.SetColumn(cell, c);

                c++;
            }

            Width = 390;
        }

        private class LabelWithEntity : Label
        {
            public GameEntity Entity { get; }

            public static SolidBrush NotSelectedColor = new(Color.DarkGoldenrod);
            public static SolidBrush SelectedColor = new(Color.IndianRed);

            public LabelWithEntity(GameEntity entity)
            {
                Entity = entity;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var cell in _cells)
            {
                cell.Border = _entityFetcher() == cell.Entity
                    ? LabelWithEntity.SelectedColor
                    : LabelWithEntity.NotSelectedColor;
            }
        }
    }
}
