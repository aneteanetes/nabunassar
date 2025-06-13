namespace Nabunassar.Tiled.Map
{
    public class TiledTile
    {
        public int Id { get; set; }

        public Dictionary<string, string> Properties { get; set; } = new();
    }
}
