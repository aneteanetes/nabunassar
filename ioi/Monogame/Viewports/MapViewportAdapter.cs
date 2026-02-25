using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace ioi.Monogame.Viewports
{

    internal class MapViewportAdapter : ScalingViewportAdapterCustom
    {
        private readonly GameHost _game;
        private readonly GameWindow _window;

        //
        // Summary:
        //     Size of horizontal bleed areas (from left and right edges) which can be safely
        //     cut off
        public int HorizontalBleed { get; }

        //
        // Summary:
        //     Size of vertical bleed areas (from top and bottom edges) which can be safely
        //     cut off
        public int VerticalBleed { get; }

        public BoxingMode BoxingMode { get; private set; }

        public bool IsBoxing { get; set; }

        public int BoundsWidth { get; set; }

        public int BoundsHeight { get; set; }


        public static float WidthPercent = 0.75f;
        public static float HeightPercent = 0.72f;

        //
        // Summary:
        //     Initializes a new instance of the MonoGame.Extended.ViewportAdapters.BoxingViewportAdapter.
        public MapViewportAdapter(GameHost game, GameWindow window, Viewport viewport, int virtualWidth, int virtualHeight, int horizontalBleed = 0, int verticalBleed = 0)
            : base(viewport, virtualWidth, virtualHeight)
        {
            _game = game;
            _window = window;
            window.ClientSizeChanged += OnClientSizeChanged;
            HorizontalBleed = horizontalBleed;
            VerticalBleed = verticalBleed;

            BoundsWidth = viewport.Width;
            BoundsHeight = viewport.Height;
        }

        public override void Dispose()
        {
            _window.ClientSizeChanged -= OnClientSizeChanged;
            base.Dispose();
        }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            BoundsWidth = (int)(_window.ClientBounds.Width * WidthPercent);
            BoundsHeight = (int)(_window.ClientBounds.Height * HeightPercent);
            //BoundsHeight

            Rectangle clientBounds = new Rectangle(0, 0, BoundsWidth, BoundsHeight);
            float value = (float)clientBounds.Width / (float)VirtualWidth;
            float value2 = (float)clientBounds.Height / (float)VirtualHeight;
            float value3 = (float)clientBounds.Width / (float)(VirtualWidth - HorizontalBleed);
            float value4 = (float)clientBounds.Height / (float)(VirtualHeight - VerticalBleed);
            float value5 = MathHelper.Max(value, value2);
            float value6 = MathHelper.Min(value3, value4);
            float num = MathHelper.Min(value5, value6);
            int num2 = (int)(num * (float)VirtualWidth + 0.5f);
            int num3 = (int)(num * (float)VirtualHeight + 0.5f);

            var mapViewportOffset = Vector2.Transform(_game.CellSize.ToVector2(), _game.CameraMain.GetViewMatrix());

            var mapViewpOffset = new Point((int)Math.Round(mapViewportOffset.X), (int)Math.Round(mapViewportOffset.Y));

            mapViewpOffset += new Point(_game.MainViewport.X, _game.MainViewport.Y);

            _game.MapViewport = new Viewport(mapViewpOffset.X, mapViewpOffset.Y, num2, num3);
        }

        public override void Reset()
        {
            base.Reset();
            OnClientSizeChanged(this, EventArgs.Empty);
        }

        public override Point PointToScreen(int x, int y)
        {
            Viewport viewport = Viewport;
            return base.PointToScreen(x - viewport.X, y - viewport.Y);
        }
        public override Matrix GetScaleMatrix()
        {
            float xScale = (float)_game.MapViewport.Width / (float)VirtualWidth;
            float yScale = (float)_game.MapViewport.Height / (float)VirtualHeight;
            return Matrix.CreateScale(xScale, yScale, 1f);
        }
    }
}
