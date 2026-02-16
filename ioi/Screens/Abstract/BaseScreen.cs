using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using ioi.Monogame.Content;
using ioi.Monogame.SpriteBatch;

namespace ioi.Screens.Abstract
{
    internal abstract class BaseScreen : GameScreen
    {
        public new GameHost Game => base.Game.As<GameHost>();

        public new GameContentManager Content => base.Content.As<GameContentManager>();

        public SpriteBatchManager SpriteBatch => Game.SpriteBatch;

        public virtual bool IsLoadingScreen => false;

        protected BaseScreen(GameHost game) : base(game)
        {
        }

        public RenderTarget2D DumpedScreen { get; set; }

        public bool IsDumping { get; private set; } = false;

        public void Dump()
        {
            IsDumping = true;
        }

        private void InternalDump()
        {
            var w = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var h = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            var data = new Color[w*h];
            Game.GraphicsDevice.GetBackBufferData<Color>(data);
            DumpedScreen = new RenderTarget2D(Game.GraphicsDevice, w,h);
            DumpedScreen.SetData(data);
            Game.DisablePostProcessors();
        }

        public override void Draw(GameTime gameTime)
        {
            Game.MyraDesktop.Render();

            if (IsDumping)
            {
                InternalDump();
                IsDumping = false;
            }
        }

        public override void Dispose()
        {
            DumpedScreen?.Dispose();
            DumpedScreen = null;
        }
    }
}
