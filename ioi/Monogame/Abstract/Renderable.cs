using Microsoft.Xna.Framework;
using ioi.Monogame.Content;
using ioi.Monogame.Interfaces;
using ioi.Monogame.SpriteBatch;

namespace ioi
{
    internal abstract class Renderable : IRenderable
    {
        public GameHost Game { get; private set; }

        public GameContentManager Content => Game.Content;

        public bool Enabled { get; set; } = true;

        public int UpdateOrder { get; set; } 

        public Renderable(GameHost game)
        {
            Game = game;
            EnabledChanged?.Invoke(null,null);
            UpdateOrderChanged?.Invoke(null, null);
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
