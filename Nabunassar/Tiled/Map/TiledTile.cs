using MonoGame.Extended;

namespace Nabunassar.Tiled.Map
{
    public class TiledTile
    {
        public int Id { get; set; }

        public Dictionary<string, string> Properties { get; set; } = new();

        public List<RectangleF> Bounds { get; set; } = new();
    }
}
