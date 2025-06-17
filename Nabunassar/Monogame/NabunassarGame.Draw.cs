using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Monogame.SpriteBatch;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public bool IsDrawBounds { get; internal set; }

        public SpriteBatchKnowed BeginDraw(bool isCameraDependant = true, SamplerState samplerState = null, bool alphaBlend = false, bool isTransformMatrix = true)
        {
            var transformMatrix = Camera.GetViewMatrix();
            this.SpriteBatch.Begin(isCameraDependant ? transformMatrix : null);
            var sb = this.SpriteBatch.GetSpriteBatch(samplerState, alphaBlend, isTransformMatrix);
            return sb;
        }
    }
}
