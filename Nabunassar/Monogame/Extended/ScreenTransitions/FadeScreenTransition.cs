using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Nabunassar
{
    public class FadeScreenTransition : ScreenTransition
    {
        private readonly GraphicsDevice _graphicsDevice;

        private readonly Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;

        public Color Color { get; }

        public FadeScreenTransition(GraphicsDevice graphicsDevice, Color color, float duration = -1)
            : base(duration)
        {
            Color = color;
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(graphicsDevice);
        }

        public override void Dispose()
        {
            _spriteBatch.Dispose();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            _spriteBatch.FillRectangle(0f, 0f, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, Color * base.Value);
            _spriteBatch.End();
        }
    }
}
