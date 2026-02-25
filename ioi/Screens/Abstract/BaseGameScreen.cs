using MonoGame.Extended.Collisions;

namespace ioi.Screens.Abstract
{
    internal abstract class BaseGameScreen : BaseScreen
    {
        protected BaseGameScreen(GameHost game) : base(game)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Game.GameState.InGame = true;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            DrawInternal(gameTime);

            Game.MyraDesktopIngame.Render();

            Game.GameWorld?.LoadingSystem?.Draw(gameTime);

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

            base.Draw(gameTime);
        }

        protected virtual void DrawInternal(GameTime gameTime) { }
    }
}