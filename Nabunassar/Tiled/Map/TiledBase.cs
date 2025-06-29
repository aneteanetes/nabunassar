using Geranium.Reflection;

namespace Nabunassar.Tiled.Map
{
    public class TiledBase
    {
        public Dictionary<string, string> Properties { get; set; } = new();

        private Dictionary<string,object> propertyValueCache = new();

        public T GetPropopertyValue<T>(string propName)
        {
            T value = default;

            if(propertyValueCache.ContainsKey(propName))
                return propertyValueCache[propName].As<T>();

            if (Properties.ContainsKey(propName))
            {
                var p = Properties[propName];
                if (typeof(T).IsEnum)
                {
                    value = Enum.Parse(typeof(T), p, true).As<T>();
                }
                else
                {
                    value = Convert.ChangeType(p, typeof(T)).As<T>();
                }
            }

            propertyValueCache[propName] = value;

            return value;
        }
    }
}
