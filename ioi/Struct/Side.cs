namespace ioi.Struct
{
    public enum Side
    {
        Left,
        Right
    }

    public static class SideExtensions
    {
        public static Side Opposite(this Side side) => side == Side.Left ? Side.Right : Side.Left;
    }
}