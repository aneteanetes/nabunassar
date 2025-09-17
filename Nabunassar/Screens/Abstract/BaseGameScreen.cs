using Geranium.Reflection;
using MonoGame.Extended.Screens;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Shaders;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Screens.Abstract
{
    internal abstract class BaseGameScreen : GameScreen
    {
        public new NabunassarGame Game => base.Game.As<NabunassarGame>();

        public new NabunassarContentManager Content => base.Content.As<NabunassarContentManager>();

        public SpriteBatchManager SpriteBatch => Game.SpriteBatch;

        protected BaseGameScreen(NabunassarGame game) : base(game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            if (Game.GameState.InGame)
                Game.WorldGame.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.Penumbra.Draw(gameTime);

            var postProcessShaders = Game.ActivePostProcessShaders.ToArray();

            if (postProcessShaders.Length > 0)
            {
                for (int i = 0; i < postProcessShaders.Length; i++)
                {
                    var postProcessor = postProcessShaders[i];
                    postProcessor.Draw(gameTime, i == postProcessShaders.Length - 1);
                }
            }

            Game.SpriteBatch.End();

            Game.MyraDesktop.Render();
        }
    }
}