using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using Nabunassar.Monogame.SpriteBatch;
using System.Diagnostics.Tracing;

namespace Nabunassar.Components.Effects
{
    internal abstract class ShaderEffectComponent : IDisposable
    {
        protected NabunassarGame Game;

        public ShaderEffectComponent(NabunassarGame game)
        {
            Game = game;
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
            Game = null;
        }

        public void Draw(GameTime gameTime, Texture2D texture, Vector2 position, float rotation = 0, Vector2 scale = default)
            => this.Draw(gameTime, new Sprite(texture), position, rotation, scale);

        public virtual void Draw(GameTime gameTime, Sprite sprite, Vector2 position, float rotation = 0, Vector2 scale = default, bool isWithEffect = true)
        {
            if (IsSeparateTexture && SeparateTexture == null)
            {
                var textureRegion = sprite.TextureRegion;
                SeparateTexture = new RenderTarget2D(Game.GraphicsDevice, textureRegion.Width, textureRegion.Height);
            }

            SpriteBatchKnowed sb;

            if (IsSeparateTexture && SeparateSprite == null)
            {
                Game.SetRenderTarget(SeparateTexture);
                Game.ClearRenderTarget(Color.Black);

                sb = Game.BeginDraw(false);
                sb.Draw(sprite, Vector2.Zero, rotation, scale);
                sb.End();

                Game.SetRenderTarget(null);

                SeparateSprite = new Sprite(SeparateTexture);
            }

            if (scale == default)
                scale = Vector2.One;

            sb = Game.BeginDraw(effect: isWithEffect ? Effect : null);

            if (IsSeparateTexture)
            {
                sb.Draw(SeparateSprite, position, rotation, scale);
            }
            else
            {
                sb.Draw(sprite, position, rotation, scale);
            }

            sb.End();
        }
    }
}