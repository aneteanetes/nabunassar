using Geranium.Reflection;

namespace Nabunassar.Tiled.Map
{
    public class TiledBase
    {
        public Dictionary<string, string> Properties { get; set; } = new();

        public T GetPropopertyValue<T>(string propName)
        {
            if (Properties.ContainsKey(propName))
            {
                var p = Properties[propName];
                if (typeof(T).IsEnum)
                {
                    return Enum.Parse(typeof(T), p, true).As<T>();
                }
                else
                {
                    return Convert.ChangeType(p, typeof(T)).As<T>();
                }
            }
            return default;
        }
    }
}
