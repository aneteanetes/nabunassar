using Geranium.Reflection;

namespace ioi.Extensions.LocalizedStringsExtensions
{
    internal static class StringNotFoundExtensions
    {
        public static bool IsNotFound(this string @string) => @string.IsEmpty() || @string == GameHost.Game.Strings.NotFound;

        public static bool IsFound(this string @string) => @string.IsNotEmpty() && @string != GameHost.Game.Strings.NotFound;
    }
}
