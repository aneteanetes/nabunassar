using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Widgets.UserInterfaces.PartyUI
{
    internal class HeroPortrait : Panel, IFeatured
    {
        private Image _portrait;

        public HeroPortrait(NabunassarGame game, Hero hero)
        {
            var imageWidth = 100;
            var imageHeight = 88;

            this.Width = imageWidth+10;
            this.Height = 150;

            var roundedTexture = game.Content.LoadTexture("Assets/Images/Borders/conditionbackground.png");
            var roundedBackground = roundedTexture.NinePatch(16);

            Background = roundedBackground;

            var texture = game.Content.Load<Texture2D>("Assets/Tilesets/" + hero.Tileset);
            var imageBorder = new Panel()
            {
                Width = imageWidth+10,
                Height = imageHeight+10,
                Background = roundedBackground
            };
            _portrait = new Image()
            {
                Renderable = new TextureRegion(texture, new Rectangle(0, 31, 16, 14)),
                Width = imageWidth,
                Height = imageHeight,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                //Margin = new Myra.Graphics2D.Thickness(5)
            };

            _frames.Add(new TextureRegion(texture, new Rectangle(0, 31, 16, 14)));
            _frames.Add(new TextureRegion(texture, new Rectangle(16, 31, 16, 14)));
            _frames.Add(new TextureRegion(texture, new Rectangle(32, 31, 16, 14)));
            _frames.Add(new TextureRegion(texture, new Rectangle(48, 31, 16, 14)));

            imageBorder.Widgets.Add(_portrait);

            Widgets.Add(imageBorder);
        }

        private int currentFrame = 0;
        private List<TextureRegion> _frames = new();

        public void Update(GameTime gameTime)
        {
            //return;
            if(this.CanUpdate(gameTime, TimeSpan.FromSeconds(0.3)))
            {
                currentFrame++;
                if (currentFrame == _frames.Count)
                    currentFrame = 0;

                _portrait.Renderable = _frames[currentFrame];
            }
        }
    }
}
