using Geranium.Reflection;

namespace Nabunassar.Extensions.LocalizedStringsExtensions
{
    internal static class StringNotFoundExtensions
    {
        public static bool IsNotFound(this string @string) => @string.IsEmpty() || @string == NabunassarGame.Game.Strings.NotFound;

        public static bool IsFound(this string @string) => @string.IsNotEmpty() && @string != NabunassarGame.Game.Strings.NotFound;
    }
}
