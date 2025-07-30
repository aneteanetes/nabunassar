namespace Nabunassar
{
    internal static class GetAllEnumValuesExtensions
    {
        public static IEnumerable<T> GetAllValues<T>(this Type enumType)
        {
            return Enum.GetValues(enumType).Cast<T>();
        }
    }
}
