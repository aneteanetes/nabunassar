using Geranium.Reflection;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace Nabunassar
{
    public static class GetAttributeValueExtension
    {
        /// <summary>
        /// Integer attribute value
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static int GetTagAttrInteger(this XElement xElement, string attr)
        {
            int.TryParse(xElement.Attribute(attr).Value, out int value);
            return value;
        }

        public static string GetTagAttrString(this XElement xElement, string attr)
        {
            return xElement.Attribute(attr).Value;
        }

        public static Color GetTagAttrColor(this XElement xElement, string attr)
        {
            var @string = xElement.Attribute(attr).Value;

            var conv = new System.Drawing.ColorConverter();
            var sysDrawColor = conv.ConvertFromString(@string).As<System.Drawing.Color>();

            return new Color(sysDrawColor.R, sysDrawColor.G, sysDrawColor.B, sysDrawColor.A);
        }
    }
}
