using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class MoneyPanel : HorizontalStackPanel
    {
        private TextureRegion _img;
        private FontSystem _font;
        private int _size = 14;
        private bool _showAll;

        public MoneyPanel(Money cost, int size=14, bool showAll=false)
        {
            _size = size;
            _showAll = showAll;
            var content = NabunassarGame.Game.Content;

            var texture = content.Load<Texture2D>("Assets/Tilesets/others.png");
            _img = new TextureRegion(texture, new Rectangle(0, 0, 16, 16));
            _font = content.LoadFont(Fonts.BitterSemiBold);

            Fill(cost);
        }

        private void Fill(Money money)
        {
            if (money.Gold > 0 || _showAll)
            {
                var goldImg = new Image()
                {
                    Renderable = _img,
                    Width = _size,
                    Height = _size,
                    Color = "#EFBF04".AsColor(),
                };

                var goldTxt = new Label()
                {
                    Font = _font.GetFont(_size),
                    Text = money.Gold.ToString()
                };

                Widgets.Add(goldImg);
                Widgets.Add(goldTxt);

                Widgets.Add(new Panel() { Width = 5 });
            }

            if (money.Silver > 0 || _showAll)
            {
                var silverImg = new Image()
                {
                    Renderable = _img,
                    Width = _size,
                    Height = _size,
                    Color = "#C4C4C4".AsColor(),
                };

                var silverTxt = new Label()
                {
                    Font = _font.GetFont(_size),
                    Text = money.Silver.ToString()
                };

                Widgets.Add(silverImg);
                Widgets.Add(silverTxt);

                Widgets.Add(new Panel() { Width = 5 });
            }

            if (money.Copper > 0 || _showAll)
            {
                var copperImg = new Image()
                {
                    Renderable = _img,
                    Width = _size,
                    Height = _size,
                    Color = "#C68346".AsColor(),
                };

                var copperTxt = new Label()
                {
                    Font = _font.GetFont(_size),
                    Text = money.Copper.ToString()
                };

                Widgets.Add(copperImg);
                Widgets.Add(copperTxt);
            }
        }
    }
}