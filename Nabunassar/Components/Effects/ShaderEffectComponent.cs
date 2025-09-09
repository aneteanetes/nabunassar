using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Nabunassar.Components.Effects
{
    internal abstract class ShaderEffectComponent : IDisposable
    {
        public ShaderEffectComponent(NabunassarGame game)
        {
        }

        public abstract void Update(GameTime gameTime);

        public virtual bool IsSeparateTexture => false;

        public RenderTarget2D SeparateTexture { get; set; }

        public Sprite SeparateSprite { get; set; }

        public Effect Effect { get; set; }

        public virtual void Dispose()
        {
            SeparateTexture?.Dispose();
            SeparateSprite = null;
            //Effect.Dispose();
        }
    }
}
