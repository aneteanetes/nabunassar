using Geranium.Reflection;
using Nabunassar.Entities.Base;

namespace Nabunassar.Tiled.Map
{
    public class TiledBase : Propertied
    {
        public virtual Vector2 Position { get; set; }

        public TiledBase CopyBase()
        {
            return new TiledBase()
            {
                Properties = new Dictionary<string, string>(base.Properties)
            };
        }
    }
}
