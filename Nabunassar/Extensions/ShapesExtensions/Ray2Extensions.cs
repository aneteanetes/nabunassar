using MonoGame.Extended;

namespace Nabunassar
{
    internal static class Ray2Extensions
    {
        public static Ray2 Normalize(this Ray2 ray)
        {
            return new Ray2(ray.Position.NormalizedCopy(), ray.Direction.NormalizedCopy());
        }

        public static Ray2 NormalizeDirection(this Ray2 ray)
        {
            return new Ray2(ray.Position, ray.Direction.NormalizedCopy());
        }
    }
}
