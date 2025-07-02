using MonoGame.Extended;

namespace Nabunassar
{
    internal static class RectangleFExtensions
    {
        public static Vector2[] ToPolygon(this RectangleF rect, bool fromZero=true)
        {
            if (fromZero)
            {
                return [
                    Vector2.Zero,
                    new Vector2(rect.Width, 0),
                    new Vector2(rect.Width, rect.Height * -1),
                    new Vector2(0, rect.Height * -1)
                ];
            }
            else
            {
                var maxLeft = rect.Position.X + rect.Width;
                var maxTop = (rect.Position.Y + rect.Height) * -1;


                return [
                    fromZero ? Vector2.Zero : rect.Position,
                    new Vector2(maxLeft, rect.Position.Y),
                    new Vector2(maxLeft, rect.Position.Y+rect.Height * -1),
                    new Vector2(rect.Position.X, maxTop)
                ];
            }
        }
    }
}
