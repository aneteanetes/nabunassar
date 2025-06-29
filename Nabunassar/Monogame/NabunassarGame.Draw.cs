using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Resources;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public bool IsDrawBounds { get; internal set; }

        public SpriteBatchKnowed BeginDraw(bool isCameraDependant = true, SamplerState samplerState = null, bool alphaBlend = false, bool isTransformMatrix = true, Effect effect = default)
        {
            var transformMatrix = Camera.GetViewMatrix();
            this.SpriteBatch.Begin(isCameraDependant ? transformMatrix : null);

            var sb = this.SpriteBatch.GetSpriteBatch(samplerState, alphaBlend, isTransformMatrix);
            return sb;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            Game.Penumbra.BeginDraw();
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            SpriteBatch.End();

            if (isDrawFPS)
                DrawFPS();

            if (isDrawCoords)
                DrawPositions();
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