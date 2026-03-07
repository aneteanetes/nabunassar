using FontStashSharp;
using ioi.Components;
using ioi.Entities.Struct;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike.Misc
{
    internal class ItemRow : HorizontalStackPanel
    {
        public GameHost Game { get; }

        public ItemRow(GameHost game, GameEntity entity, DynamicSpriteFont font)
        {
            Game = game;

            var strings = Game.Strings["Roguelike"];
            var goldenrod = new SolidBrush(Color.Goldenrod);
            var imgH = ((int)Math.Round(game.CellSize.Y * 1.5));
            var imgW = ((int)Math.Round(game.CellSize.X * 1.5));

            Border = goldenrod;
            BorderThickness = new Myra.Graphics2D.Thickness(1);
            Margin = new Myra.Graphics2D.Thickness(15, 5);

            var iconContainer = new Panel()
            {
                Width = 75,
                Height = 75,
                Border = goldenrod,
                BorderThickness = new Myra.Graphics2D.Thickness(0, 0, 1, 0),
            };
            var icon = new Image()
            {
                Color = entity.Color("color"),
                Renderable = entity.GetTilesetRegion().ToMyraRegion(),
                Width = imgW,
                Height = imgH,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            iconContainer.Widgets.Add(icon);
            

            this.Widgets.Add(iconContainer);

            var vbox = new VerticalStackPanel();

            vbox.Margin = new Myra.Graphics2D.Thickness(8, 8, 8, 0);

            var name = new Label()
            {
                Text = entity.GetNameColored(),
                TextColor = entity.Color("color"),
                Font = font
            };
            vbox.Widgets.Add(name);

            var statslabel = new Label()
            {
                TextColor = Color.DarkGray,
                Font = font,
            };

            DrawText statsText = DrawText.Create(" ");

            var stats = entity["itemstats"].Table;
            foreach (var stat in stats.Keys)
            {
                var statValue = entity[stat.String].Number;

                if (statValue == 0)
                    continue;

                statsText.Append(strings[$"{stat.String}_cut"])
                    .Append(statValue > 0 ? "+" : "-")
                    .Append(statValue.ToString())
                    .Append(" ");
            }
            statslabel.Text = statsText.ToString();

            vbox.Widgets.Add(statslabel);

            this.Widgets.Add(vbox);
        }
    }
}
