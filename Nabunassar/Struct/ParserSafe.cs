namespace Nabunassar
{
    internal static class ParserSafe
    {
        public static int AsInt(this string @string)
        {
            if(int.TryParse(@string, out int result))
                return result;

            return default;
        }

        public static double AsDouble(this string @string)
        {
            if (double.TryParse(@string, out double result))
                return result;

            return default;
        }

        public static float AsFloat(this string @string)
        {
            if (float.TryParse(@string, out float result))
                return result;

            return default;
        }

        public static bool AsBool(this string @string)
        {
            if (bool.TryParse(@string, out bool result))
                return result;

            return default;
        }

    }
}
