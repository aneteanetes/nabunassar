using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Myra.Graphics2D.UI;
using Nabunassar.Desktops;
using Nabunassar.Desktops.Menu;
using Nabunassar.Desktops.UserInterfaces;
using Nabunassar.Desktops.UserInterfaces.ContextMenus;
using Nabunassar.Resources;
using Nabunassar.Screens.Abstract;

namespace Nabunassar.Screens
{
    internal class MainMenuScreen : BaseGameScreen
    {
        private Texture2D background;
        private Vector2 _position = new Vector2(50, 50);

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
            _position = Vector2.Lerp(_position, Mouse.GetState().Position.ToVector2(), 1f * gameTime.GetElapsedSeconds());
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);

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
