using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Monogame.SpriteBatch;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public SpriteBatchKnowed BeginDraw(SamplerState samplerState = null, bool alphaBlend = false, bool isTransformMatrix = true)
        {
            this.SpriteBatch.Begin();
            var sb = this.SpriteBatch.GetSpriteBatch(samplerState, alphaBlend, isTransformMatrix);
            return sb;
        }
    }
}
