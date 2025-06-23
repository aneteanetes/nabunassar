using System.Globalization;

namespace Nabunassar
{
    internal static class ColorFromHexExtensions
    {
        public static Color AsColor(this string hexcolor)
        {
            hexcolor = hexcolor.Replace("#", "");

            var a = int.Parse(hexcolor.Substring(0, 2), NumberStyles.HexNumber);
            var r = int.Parse(hexcolor.Substring(2, 2), NumberStyles.HexNumber);
            var g = int.Parse(hexcolor.Substring(4, 2), NumberStyles.HexNumber);
            var b = int.Parse(hexcolor.Substring(6, 2), NumberStyles.HexNumber);

            return new Color(r, g, b, a);
        }
    }
}
