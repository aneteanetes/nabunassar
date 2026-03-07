using MonoGame.Extended.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace ioi
{
    public static class TextureRegionsExtensions
    {
        public static TextureRegion ToMyraRegion(this Texture2DRegion texture2DRegion)
        {
            return new TextureRegion(texture2DRegion.Texture, new Rectangle()
            {
                X = texture2DRegion.X,
                Y = texture2DRegion.Y,
                Width = texture2DRegion.Width,
                Height = texture2DRegion.Height
            });
        }
    }
}
