using Microsoft.Xna.Framework;
using Nabunassar.Monogame.SpriteBatch;

namespace Nabunassar.Monogame.Interfaces
{
    internal interface IRenderable : ILoadable, IUpdateable, IDisposable
    {
        void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch);
    }
}
