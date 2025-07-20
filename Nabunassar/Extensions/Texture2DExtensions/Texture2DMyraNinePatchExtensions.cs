using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace Nabunassar
{
    internal static class Texture2DMyraNinePatchExtensions
    {
        internal static NinePatchRegion NinePatch(this Texture2D texture)
        {
            return new NinePatchRegion(texture, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));
        }

        internal static NinePatchRegion NinePatchDouble(this Texture2D texture)
        {
            return new NinePatchRegion(texture, new Rectangle(0, 0, 96, 96), new Myra.Graphics2D.Thickness(24));
        }
    }
}
