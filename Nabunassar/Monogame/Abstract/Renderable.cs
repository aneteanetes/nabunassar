using Microsoft.Xna.Framework;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Interfaces;
using Nabunassar.Monogame.SpriteBatch;

namespace Nabunassar
{
    internal abstract class Renderable : IRenderable
    {
        public NabunassarGame Game { get; private set; }

        public NabunassarContentManager Content => Game.Content;

        public bool Enabled { get; set; } = true;

        public int UpdateOrder { get; set; } 

        public Renderable(NabunassarGame game)
        {
            Game = game;
        }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public virtual void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch) { }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Dispose() => UnloadContent();
    }
}
