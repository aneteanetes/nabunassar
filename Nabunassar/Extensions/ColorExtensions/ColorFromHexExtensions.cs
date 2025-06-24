using System.Globalization;

namespace Nabunassar
{
    internal static class ColorFromHexExtensions
    {
        public static Color AsColor(this string hexcolor)
        {
            hexcolor = hexcolor.Replace("#", "");

            int a=255, r, g, b;

            if (hexcolor.Length > 6)
            {
                a = int.Parse(hexcolor.Substring(0, 2), NumberStyles.HexNumber);
                r = int.Parse(hexcolor.Substring(2, 2), NumberStyles.HexNumber);
                g = int.Parse(hexcolor.Substring(4, 2), NumberStyles.HexNumber);
                b = int.Parse(hexcolor.Substring(6, 2), NumberStyles.HexNumber);
            }
            else
            {
                r = int.Parse(hexcolor.Substring(0, 2), NumberStyles.HexNumber);
                g = int.Parse(hexcolor.Substring(2, 2), NumberStyles.HexNumber);
                b = int.Parse(hexcolor.Substring(4, 2), NumberStyles.HexNumber);
            }

            return new Color(r, g, b, a);
        }
    }
}
