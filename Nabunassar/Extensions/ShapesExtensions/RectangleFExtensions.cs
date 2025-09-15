using FontStashSharp;
using MonoGame.Extended;

namespace Nabunassar
{
    internal static class RectangleFExtensions
    {
        public static Vector2[] ToPolygon(this RectangleF rect, bool fromZero = true)
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

        public static RectangleF Multiple(this RectangleF rect, float multiplier)
        {
            var newWidth = rect.Width * multiplier;
            var newHeight = rect.Height * multiplier;

            var newX = rect.X + rect.Width / 2 - newWidth / 2;
            var newY = rect.Y + rect.Height / 2 - newHeight / 2;

            return new RectangleF(newX, newY, newWidth, newHeight);
        }

        public static RectangleF Add(this RectangleF rect, float measure)
        {
            var newWidth = rect.Width + measure;
            var newHeight = rect.Height + measure;

            var newX = rect.X + rect.Width / 2 - newWidth / 2;
            var newY = rect.Y + rect.Height / 2 - newHeight / 2;

            return new RectangleF(newX, newY, newWidth, newHeight);
        }

        public static RectangleF MultipleX(this RectangleF rect, float multiplier)
        {
            var newWidth = rect.Width * multiplier;

            var newX = rect.X + rect.Width / 2 - newWidth / 2;

            return new RectangleF(newX, rect.Y, newWidth, rect.Height);
        }

        public static RectangleF MultipleY(this RectangleF rect, float multiplier)
        {
            var newHeight = rect.Height * multiplier;

            var newY = rect.Y + rect.Height / 2 - newHeight / 2;

            return new RectangleF(rect.X, newY, rect.Width, newHeight);
        }

        public static Vector2 GetOrigin(this RectangleF rect)
        {
            var boundsOriginX = rect.Position.X + rect.BoundingRectangle.Width / 2;
            var boundsOriginY = rect.Position.Y + rect.BoundingRectangle.Height / 2;
            return new Vector2(boundsOriginX, boundsOriginY);

        }

        public static bool InBounds(this RectangleF rect, Vector2 point, out Vector2 newPosition)
        {
            if (rect.Intersects(new RectangleF(point, new SizeF(1, 1))))
            {
                newPosition = point;
                return true;
            }
            else
            {
                newPosition.X = Math.Max(point.X, rect.X);
                newPosition.Y = Math.Max(point.Y, rect.Y);

                newPosition.X = Math.Min(newPosition.X, rect.X + rect.Width);
                newPosition.Y = Math.Min(newPosition.Y, rect.Y + rect.Height);

                return false;
            }
        }

        public static BoundingBox ToBoundingBox(this RectangleF rect)
        {
            return new BoundingBox(new Vector3(rect.Left, rect.Top, 0), new Vector3(rect.Right, rect.Bottom, 0));
        }
    }
}