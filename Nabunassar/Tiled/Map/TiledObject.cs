#pragma warning disable IDE1006 // Naming Styles

using Geranium.Reflection;
using Microsoft.Xna.Framework;

namespace Nabunassar.Tiled.Map
{
    public class TiledObject
    {
        public int id { get; set; }

        public int gid { get; set; }

        public string file { get; set; }

        public double x { get; set; }

        public double y { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public Vector2 Position { get; set; }

        public bool collide => Properties?.FirstOrDefault(x => x.name == "collide")?.value == "true";

        public string objectgroup { get; set; }

        public List<TiledObjectProperty> Properties { get; set; } = new List<TiledObjectProperty>();

        public T GetPropValue<T>(string propName)
        {
            var p = Properties.FirstOrDefault(x => x.name.ToLower() == propName.ToLower());
            if (p == null)
                return default;

            return Convert.ChangeType(p.value, typeof(T)).As<T>();
        }

        public Point[] Polygon { get; set; } = new Point[0];

        public TiledTileset Tileset { get; set; }

    }

    public class TiledObjectProperty
    {
        public string name { get; set; }

        public string value { get; set; }
    }
}
#pragma warning restore IDE1006 // Naming Styles