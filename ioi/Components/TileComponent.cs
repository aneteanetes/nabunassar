using ioi.Tiled.Map;

namespace ioi.Components
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
