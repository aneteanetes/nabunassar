namespace Nabunassar.Entities.Struct.ImageRegions
{
    internal record struct ImageRegion(int x, int y, int width, int height, string texture = null)
    {
        public ImageRegion() : this(default, default, default, default, default)
        {

        }

        public readonly string Texture { get; } = texture;
        public readonly int X { get; } = x;
        public readonly int Y { get; } = y;
        public readonly int Width { get; } = width;
        public readonly int Height { get; } = height;

        public Rectangle ToRectangle()=>new Rectangle(X, Y, Width, Height);
    }
}