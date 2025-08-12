using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Resources;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public bool IsDrawBounds { get; internal set; }

        internal RenderTarget2D BackRenderTarget = null;

        public void SetRenderTarget(RenderTarget2D target=null)
        {
            if (target == null)
            {
                if (BackRenderTarget == null)
                    GraphicsDevice.SetRenderTarget(null);
                else
                    GraphicsDevice.SetRenderTarget(BackRenderTarget);
            
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

        Effect _globalEffect = null;

        public SpriteBatchKnowed BeginDraw(bool isCameraDependant = true, SamplerState samplerState = null, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState=null, bool isTransformMatrix = true, Effect effect = default)
        {
            var transformMatrix = Camera.GetViewMatrix();
            this.SpriteBatch.Begin(isCameraDependant ? transformMatrix : null);

            var sb = this.SpriteBatch.GetSpriteBatch(samplerState, sortMode, blendState, isTransformMatrix, effect ?? _globalEffect);
            return sb;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            if (_grayscaleMapTexture != null)
            {
                DrawGrayscaleMap();
            }

            Game.Penumbra.BeginDraw();
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            if(_grayscaleMapTexture!=null)
            {
                BackRenderTarget = null;
                SetRenderTarget(null);
                ClearRenderTarget(Color.Black);
                var sb = BeginDraw(false,effect: _grayscaleMapShader);
                sb.Draw(_backBuffer, Game.Resolution, Color.White);
            }

            SpriteBatch.End();

            if (isDrawFPS)
                DrawFPS();

            if (isDrawCoords)
                DrawPositions();
        }

        private Texture2D _grayscaleMapTexture;
        private Func<float> _grayscaleGrayIntensive;
        private Func<Rectangle> _grayscaleMapDestination;

        public void GrayscaleMapActivate(Texture2D texturePattern, Func<Rectangle> destination, Func<float> grayIntensive)
        {
            _grayscaleMapTexture = texturePattern;
            _grayscaleMapDestination = destination;
            _grayscaleGrayIntensive = grayIntensive;
        }

        public void GrayscaleMapDisable()
        {
            _grayscaleMapTexture = null;
            _grayscaleMapDestination = null;
            _globalEffect = null;
        }

        public void DrawGrayscaleMap()
        {
            Game.SetRenderTarget(_grayscaleMapBuffer);
            Game.ClearRenderTarget(Color.Transparent);

            var sb = BeginDraw();
            sb.Draw(_grayscaleMapTexture, _grayscaleMapDestination(), Color.White);
            sb.End();

            BackRenderTarget = _backBuffer;

            Game.SetRenderTarget(null);
            Game.ClearRenderTarget(Color.Transparent);

            _grayscaleMapShader.Parameters["patternTexture"].SetValue(_grayscaleMapBuffer);
            _grayscaleMapShader.Parameters["grayIntensive"].SetValue(_grayscaleGrayIntensive());
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