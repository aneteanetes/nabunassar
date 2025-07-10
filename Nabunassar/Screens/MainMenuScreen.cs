using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Widgets.Menu;
using Nabunassar.Resources;
using Nabunassar.Screens.Abstract;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Screens
{
    internal class MainMenuScreen : BaseGameScreen
    {
        private Texture2D background;
        private Effect effect;

        public MainMenuScreen(NabunassarGame game) : base(game) { }

        public override void LoadContent()
        {
            base.LoadContent();
            Game.Content.LoadFont(Fonts.Retron);
            background = Game.Content.Load<Texture2D>("Assets/Images/Backgrounds/logo.png");
        }

        public override ScreenWidget GetWidget()
        {
            return new MainMenu(Game);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            //Game.GraphicsDevice.Clear(Color.Black);

            var sb = Game.BeginDraw();
            sb.Draw(background, Game.Resolution, new Rectangle(0, 0, 2560, 1440), Color.White);

            sb.DrawText(Fonts.Retron, 35, nameof(MainMenuScreen), new Vector2(10, 10), Color.LightCyan);
            sb.End();

            base.Draw(gameTime);
        }

        public override void UnloadContent()
        {
            Game.Content.UnloadAsset("Assets/Images/Backgrounds/logo.png");
        }
    }
}
