using Geranium.Reflection;
using Microsoft.Xna.Framework;

namespace Nabunassar.Tiled.Map
{
    public class TiledPolygon
    {
        public TiledPolygon(int gid) => Gid = gid;

        public int Gid { get; set; }

        public string FileName { get; set; }

        public bool FlippedHorizontally { get; set; }

        public bool FlippedVertically { get; set; }

        public bool FlippedDiagonally { get; set; }

        public int TileOffsetX { get; set; }

        public int TileOffsetY { get; set; }

        public Vector2 Position { get; set; } = Vector2.Zero;

        public TiledLayer Layer { get; set; }

        public TiledTileset Tileset { get; set; }

        public Dictionary<string,string> Properties { get; set; }

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
