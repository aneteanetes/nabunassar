using Microsoft.Xna.Framework.Graphics;

namespace ioi.Monogame.Viewports
{
    public abstract class ViewportAdapterCustom : IDisposable
    {
        public Viewport Viewport { get; protected set; }

        public abstract int VirtualWidth { get; }

        public abstract int VirtualHeight { get; }

        public abstract int ViewportWidth { get; }

        public abstract int ViewportHeight { get; }

        public Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);

        public Point Center => BoundingRectangle.Center;

        protected ViewportAdapterCustom(Viewport viewport)
        {
            Viewport = viewport;
        }

        public virtual void Dispose()
        {
        }

        public abstract Matrix GetScaleMatrix();

        public Point PointToScreen(Point point)
        {
            return PointToScreen(point.X, point.Y);
        }

        public virtual Point PointToScreen(int x, int y)
        {
            Matrix matrix = Matrix.Invert(GetScaleMatrix());
            return Vector2.Transform(new Vector2(x, y), matrix).ToPoint();
        }

        public virtual void Reset()
        {
        }
    }
}
