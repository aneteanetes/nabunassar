using Nabunassar.Tiled.Map;

namespace Nabunassar.Components
{
    internal class TileComponent
    {
        public TiledPolygon Polygon { get;private set; }

        public TileComponent(TiledPolygon polygon)
        {
            Polygon = polygon;
        }
    }
}
