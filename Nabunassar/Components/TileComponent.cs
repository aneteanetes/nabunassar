using Nabunassar.Tiled.Map;

namespace Nabunassar.Components
{
    internal class TileComponent
    {
        public TiledBase Polygon { get;private set; }

        public TileComponent(TiledBase polygon)
        {
            Polygon = polygon;
        }
    }
}
