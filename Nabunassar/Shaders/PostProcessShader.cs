using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Monogame.Content;

namespace Nabunassar.Shaders
{
    internal class PostProcessShader : IDisposable
    {
        private static RenderTarget2D _bufferOfBuffers;
        protected NabunassarGame Game;

        protected NabunassarContentManager Content => Game.Content;

        protected Effect Effect;

        public PostProcessShader(NabunassarGame game, string shaderPath)
        {
            Game = game;
            Effect = LoadShader(shaderPath);

            if (_bufferOfBuffers == null)
                _bufferOfBuffers = new RenderTarget2D(Game.GraphicsDevice, Game.Viewport.Width, Game.Viewport.Height);
        }

        public virtual void LoadContent() {
            IsLoaded = true;
        }

        public bool IsLoaded = false;

        public virtual Effect LoadShader(string shaderPath)
        {
            return Content.Load<Effect>(shaderPath);
        }

        public virtual void Activate()
        {
            Game.IsPostEffects = true;
            Game.SetRenderTargetBackBuffer();
            Game.SetRenderTarget(null);
            Game.ClearRenderTarget(Color.Transparent);
        }

        public virtual void Disable()
        {
            Game.DisablePostProcessors(this);
        }

        public virtual void Enable()
        {
            Game.AddPostProcessor(this);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, bool isLast=true)
        {
            var backBuffer = Game.GetBackBuffer();

            Game.GraphicsDevice.SetRenderTarget(null);
            Game.GraphicsDevice.Clear(Color.Black);

            if (!isLast)
            {
                Game.GraphicsDevice.SetRenderTarget(_bufferOfBuffers);
            }

            var sb = Game.BeginDraw(false, effect: Effect);            
            sb.Draw(backBuffer, Game.Viewport, Color.White);
            sb.End();

            if (!isLast)
            {
                Game.GraphicsDevice.SetRenderTarget(backBuffer);
                sb = Game.BeginDraw(false);
                sb.Draw(_bufferOfBuffers, Game.Viewport, Color.White);
                sb.End();
            }
        }

        public virtual void Dispose()
        {
            Game.DisablePostProcessors(this);
        }
    }
}
