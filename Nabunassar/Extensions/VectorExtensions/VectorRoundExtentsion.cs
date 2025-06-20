namespace Nabunassar
{
    internal static class VectorRoundExtentsion
    {
        public static Vector2 RoundNew(this Vector2 vector)
        {
            var x = MathF.Round(vector.X);
            var y = MathF.Round(vector.Y);

            return new Vector2(x, y);
        }
    }
}
