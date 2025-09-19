using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.SpriteBatch;

namespace Nabunassar.Screens.Abstract
{
    internal abstract class BaseScreen : GameScreen
    {
        public new NabunassarGame Game => base.Game.As<NabunassarGame>();

        public new NabunassarContentManager Content => base.Content.As<NabunassarContentManager>();

        public SpriteBatchManager SpriteBatch => Game.SpriteBatch;

        public virtual bool IsLoadingScreen => false;

        protected BaseScreen(NabunassarGame game) : base(game)
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
            var data = new Color[Game.Resolution.Width * Game.Resolution.Height];
            Game.GraphicsDevice.GetBackBufferData<Color>(data);
            DumpedScreen = new RenderTarget2D(Game.GraphicsDevice, Game.Resolution.Width, Game.Resolution.Height);
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
