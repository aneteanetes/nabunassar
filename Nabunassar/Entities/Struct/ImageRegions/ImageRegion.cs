namespace Nabunassar.Entities.Struct.ImageRegions
{
    internal record struct ImageRegion(int X, int Y, int Width, int Height)
    {
        public ImageRegion() : this(default, default, default, default)
        {

        }

        public readonly int X { get; } = X;
        public readonly int Y { get; } = Y;
        public readonly int Width { get; } = Width;
        public readonly int Height { get; } = Height;
    }
}