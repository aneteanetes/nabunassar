using Microsoft.Xna.Framework;
using ioi.Monogame.SpriteBatch;

namespace ioi.Monogame.Interfaces
{
    internal interface IRenderable : ILoadable, IUpdateable, IDisposable
    {
        void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch);
    }
}
