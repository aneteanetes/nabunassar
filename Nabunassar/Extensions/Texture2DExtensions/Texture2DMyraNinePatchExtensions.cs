using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace Nabunassar
{
    internal static class Texture2DMyraNinePatchExtensions
    {
        private static Dictionary<Texture2D, NinePatchRegion> _ninePatch = new();
        private static Dictionary<Texture2D, NinePatchRegion> _ninePatchDouble = new();

        internal static NinePatchRegion NinePatch(this Texture2D texture)
        {
            if(!_ninePatch.ContainsKey(texture))
                _ninePatch[texture] = new NinePatchRegion(texture, new Rectangle(0, 0, 48, 48), new Myra.Graphics2D.Thickness(12));

            return _ninePatch[texture];
        }

        internal static NinePatchRegion NinePatchDouble(this Texture2D texture)
        {
            if(!_ninePatchDouble.ContainsKey(texture))
                _ninePatchDouble[texture] = new NinePatchRegion(texture, new Rectangle(0, 0, 96, 96), new Myra.Graphics2D.Thickness(24));

            return _ninePatchDouble[texture];
        }
    }
}