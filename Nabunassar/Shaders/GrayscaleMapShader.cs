using Microsoft.Xna.Framework.Graphics;

namespace Nabunassar.Shaders
{
    internal class GrayscaleMapShader : PostProcessShader
    {
        private static RenderTarget2D _grayscaleMapBuffer;
        private static Texture2D _defaultTexture;

        private Texture2D _grayscaleMapTexture;
        private Func<float> _grayscaleGrayIntensive;
        private Func<Rectangle> _grayscaleMapDestination;

        public GrayscaleMapShader(NabunassarGame game, Func<Rectangle> destination, Func<float> grayIntensive, Texture2D patternTexture = null) : base(game, "Assets/Shaders/GrayscaleMap.fx")
        {
            _grayscaleMapTexture = patternTexture;
            _grayscaleMapDestination = destination;
            _grayscaleGrayIntensive = grayIntensive;
        }

        public override void LoadContent()
        {
            if (_defaultTexture == null)
                _defaultTexture = Content.LoadTexture("Assets/Images/Interface/circleblend400.png");

            if (_grayscaleMapTexture == null)
                _grayscaleMapTexture = _defaultTexture;

            if (_grayscaleMapBuffer == null)
                _grayscaleMapBuffer = new RenderTarget2D(Game.GraphicsDevice, Game.Resolution.Width, Game.Resolution.Height);
        }

        public override void Draw(GameTime gameTime, bool isLast = true)
        {
            Game.SetRenderTarget(_grayscaleMapBuffer);
            Game.ClearRenderTarget(Color.Transparent);

            var sb = Game.BeginDraw();
            sb.Draw(_grayscaleMapTexture, _grayscaleMapDestination(), Color.White);
            sb.End();

            Shader.Parameters["patternTexture"].SetValue(_grayscaleMapBuffer);
            Shader.Parameters["grayIntensive"].SetValue(_grayscaleGrayIntensive());

            base.Draw(gameTime, isLast);
        }
    }
}
