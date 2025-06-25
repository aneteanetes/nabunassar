using Geranium.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using Nabunassar.Desktops;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.SpriteBatch;

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

        public abstract ScreenWidget GetWidget();

        public override void Draw(GameTime gameTime)
        {
            Game.WorldGame.Draw(gameTime);

            Game.SpriteBatch.End();
            //Game.World.Draw(gameTime, Game.BeginDraw());
            Game.Desktop.Render();
        }
    }
}