using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens.Transitions;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Shaders;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public bool IsDrawBounds { get; internal set; }

        private RenderTarget2D _backRenderTarget = null;

        public void SetRenderTarget(RenderTarget2D target=null)
        {
            if (target == null)
            {
                if (_backRenderTarget == null)
                    GraphicsDevice.SetRenderTarget(null);
                else
                    GraphicsDevice.SetRenderTarget(_backRenderTarget);
            
            }
            else
            {
                GraphicsDevice.SetRenderTarget(target);
            }
        }

        public void ClearRenderTarget(Color color)
        {
            GraphicsDevice.Clear(color);
        }

        public RenderTarget2D GetBackBuffer()
        {
            return Game._backBuffer;
        }

        public void SetRenderTargetBackBuffer(RenderTarget2D target=null)
        {
            Game._backRenderTarget = target ?? Game._backBuffer;
        }

        public void ClearRenderTargetBackBuffer()
        {
            Game._backRenderTarget = null;
        }

        public SpriteBatchKnowed BeginDraw(bool isCameraDependant = true, SamplerState samplerState = null, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState=null, bool isTransformMatrix = true, Effect effect = default, Matrix? matrix=null)
        {
            var transformMatrix = Camera.GetViewMatrix();

            this.SpriteBatch.Begin(isCameraDependant
                ? transformMatrix
                : matrix);

            var sb = this.SpriteBatch.GetSpriteBatch(samplerState, sortMode, blendState, isTransformMatrix, effect);
            return sb;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            if (IsMakingScreenShot == true)
            {
                IsMakingScreenShot = false;
                Game.SetRenderTarget(_screenShotTarget);
                Game.ClearRenderTarget(Color.Black);
                Game.SetRenderTargetBackBuffer(_screenShotTarget);
            }

            //base.Draw(gameTime);

            if (PostProcessShaders.Count > 0)
            {
                ActivateBackBuffer();
            }

            //Game.Penumbra.BeginDraw();
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            if (isDrawFPS)
                DrawFPS();

            if (isDrawCoords)
                DrawPositions();

            if (IsMakingScreenShot == false)
            {
                Game.ClearRenderTargetBackBuffer();
                Game.SetRenderTarget(null);
            }
        }

        public bool IsPostEffects = false;

        private List<PostProcessShader> PostProcessShaders = new();

        public IEnumerable<PostProcessShader> ActivePostProcessShaders => PostProcessShaders;

        public void AddPostProcessor(PostProcessShader shader)
        {
            if (!shader.IsLoaded)
            {
                shader.LoadContent();
                shader.IsLoaded = true;
            }
            PostProcessShaders.Add(shader);
        }

        private void ActivateBackBuffer()
        {
            IsPostEffects = true;
            _backRenderTarget = _backBuffer;
            Game.SetRenderTarget(null);
            Game.ClearRenderTarget(Color.Transparent);
        }

        public void DisablePostProcessors(PostProcessShader shader = null)
        {
            _backRenderTarget = null;
            IsPostEffects = false;
            if (shader != null)
                PostProcessShaders.Remove(shader);
            else
                PostProcessShaders.Clear();
        }

        public void DrawFPS()
        {
            var sb = BeginDraw(false);

            sb.DrawText(Fonts.Retron, 30, FrameCounter.ToString(), new Vector2(1, 1), Color.Yellow);

            sb.End();
        }

        public void DrawPositions()
        {
            var sb = BeginDraw(false);

            sb.DrawText(Fonts.Retron, 20, "World: " + _worldPosition.ToString(), new Vector2(50, 100), Color.PeachPuff);
            sb.DrawText(Fonts.Retron, 20, "Display: " + _mousePosition.ToString(), new Vector2(50, 125), Color.AntiqueWhite);

            sb.End();
        }
    }
}