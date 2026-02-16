namespace ioi.Entities.Data.Animations
{
    internal class AnimationFile
    {
        public string Tileset { get; set; }

        public int TileWidth { get; set; }

        public int TileHeight { get; set; }

        public List<AnimationInFile> Animations { get; set; }
    }
}
