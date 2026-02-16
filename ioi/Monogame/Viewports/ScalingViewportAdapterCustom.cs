using Microsoft.Xna.Framework.Graphics;

namespace ioi.Monogame.Viewports
{
    public class ScalingViewportAdapterCustom : ViewportAdapterCustom
    {
        public override int VirtualWidth { get; }

        public override int VirtualHeight { get; }

        public override int ViewportWidth => Viewport.Width;

        public override int ViewportHeight => Viewport.Height;

        public ScalingViewportAdapterCustom(Viewport viewport, int virtualWidth, int virtualHeight)
            : base(viewport)
        {
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
        }

        public override Matrix GetScaleMatrix()
        {
            float xScale = (float)ViewportWidth / (float)VirtualWidth;
            float yScale = (float)ViewportHeight / (float)VirtualHeight;
            return Matrix.CreateScale(xScale, yScale, 1f);
        }
    }
}
