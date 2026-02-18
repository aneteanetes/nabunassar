using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace ioi.Monogame.Viewports
{
    internal class BoxingViewportAdapterCustom : ScalingViewportAdapterCustom
    {
        private readonly GameHost _game;
        private readonly GraphicsDevice _graphicsDevice;
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

        //
        // Summary:
        //     Initializes a new instance of the MonoGame.Extended.ViewportAdapters.BoxingViewportAdapter.
        public BoxingViewportAdapterCustom(GameHost gameHost, GameWindow window, GraphicsDevice graphicsDevice, Viewport viewport, int virtualWidth, int virtualHeight, int horizontalBleed = 0, int verticalBleed = 0)
            : base(viewport, virtualWidth, virtualHeight)
        {
            _game = gameHost;
            _graphicsDevice = graphicsDevice;
            _window = window;
            window.ClientSizeChanged += OnClientSizeChanged;
            HorizontalBleed = horizontalBleed;
            VerticalBleed = verticalBleed;
        }

        public override void Dispose()
        {
            _window.ClientSizeChanged -= OnClientSizeChanged;
            base.Dispose();
        }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            Rectangle clientBounds = _window.ClientBounds;
            float value = (float)clientBounds.Width / (float)VirtualWidth;
            float value2 = (float)clientBounds.Height / (float)VirtualHeight;
            float value3 = (float)clientBounds.Width / (float)(VirtualWidth - HorizontalBleed);
            float value4 = (float)clientBounds.Height / (float)(VirtualHeight - VerticalBleed);
            float value5 = MathHelper.Max(value, value2);
            float value6 = MathHelper.Min(value3, value4);
            float num = MathHelper.Min(value5, value6);
            int num2 = (int)(num * (float)VirtualWidth + 0.5f);
            int num3 = (int)(num * (float)VirtualHeight + 0.5f);
            if (num3 >= clientBounds.Height && num2 < clientBounds.Width)
            {
                BoxingMode = BoxingMode.Pillarbox;
            }
            else if (num2 >= clientBounds.Height && num3 <= clientBounds.Height)
            {
                BoxingMode = BoxingMode.Letterbox;
            }
            else
            {
                BoxingMode = BoxingMode.None;
            }

            int x = clientBounds.Width / 2 - num2 / 2;
            int y = clientBounds.Height / 2 - num3 / 2;

            _game.GraphicsDevice.Viewport = _graphicsDevice.Viewport = new Viewport(x, y, num2, num3);
            _game.mainViewport = Viewport = _graphicsDevice.Viewport;

            // backbuffer
            _game._backBuffer = new RenderTarget2D(_game.GraphicsDevice, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height);
            _game._screenShotTarget = new RenderTarget2D(_game.GraphicsDevice, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height);
            _game._shareTarget = new RenderTarget2D(_game.GraphicsDevice, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height);


            GameController.GlobalBlurShaderReset();
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
    }
}
