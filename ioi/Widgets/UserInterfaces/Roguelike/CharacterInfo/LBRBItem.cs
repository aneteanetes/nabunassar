namespace ioi.Widgets.UserInterfaces.Roguelike.CharacterInfo
{
    internal class LBRBItem
    {
        public string Tileset { get; set; }

        public int TileId { get; set; }

        public string Id { get; set; }

        public LBRBItem(string id, int tileId, string tileset = "Consolas1")
        {
            Tileset = tileset;
            Id = id;
            TileId= tileId;
        }
    }
}