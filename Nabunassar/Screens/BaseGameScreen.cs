using Geranium.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Tiled;
using Nabunassar.Desktops;
using Nabunassar.Monogame.SpriteBatch;
using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Monogame.Content;

namespace Nabunassar.Screens
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
            Game.Desktop.Render();
        }
    }
}