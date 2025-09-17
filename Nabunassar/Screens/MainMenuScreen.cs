using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Screens.Abstract;
using Nabunassar.Widgets.Menu;

namespace Nabunassar.Screens
{
    internal class MainMenuScreen : BaseGameScreen
    {
        private Texture2D background;

        public MainMenuScreen(NabunassarGame game) : base(game) { }

        public override void LoadContent()
        {
            Game.Content.LoadFont(Fonts.Retron);
            background = Game.Content.Load<Texture2D>("Assets/Images/Backgrounds/logo.png");
            Game.AddDesktopWidget(new MainMenu(Game));
            Game.InitGameWorld();
            Game.InitializeGameState();
            Game.IsGameActive = true;
        }

        public override void Draw(GameTime gameTime)
        {
            var sb = Game.BeginDraw();
            sb.Draw(background, Game.Resolution, new Rectangle(0, 0, 2560, 1440), Color.White);
            sb.End();

            base.Draw(gameTime);
        }

        public override void UnloadContent()
        {
            Game.RemoveDesktopWidgets(true);
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}