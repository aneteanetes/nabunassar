using Microsoft.Xna.Framework.Graphics;

namespace Nabunassar.Shaders
{
    internal class GlitterShader : PostProcessShader
    {
        private Texture2D _patternTexture;
        private static RenderTarget2D _buffer;

        public GlitterShader(NabunassarGame game) : base(game, "Assets/Shaders/Glitter.fx")
        {
        }

        public override void LoadContent()
        {
            _patternTexture = Content.LoadTexture("Assets/Images/Effects/texture.png");
            Effect.Parameters["patternTexture"].SetValue(_patternTexture);

            if (_buffer == null)
                _buffer = new RenderTarget2D(Game.GraphicsDevice, Game.Resolution.Width, Game.Resolution.Height);
        }
    }
}
