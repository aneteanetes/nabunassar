using Geranium.Reflection;
using MonoGame.Extended.Screens;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.SpriteBatch;
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

        public virtual ScreenWidget GetWidget() => null;


        public override void Draw(GameTime gameTime)
        {
            Game.WorldGame.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.Penumbra.Draw(gameTime);

            //Game.World.Draw(gameTime, Game.BeginDraw());
            Game.DesktopContainer.Render();
        }
    }
}