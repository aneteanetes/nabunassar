using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Screens.Abstract;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Screens.LoadingScreens
{
    internal class BaseLoadingScreen : BaseGameScreen
    {
        Texture2D background;


        public BaseLoadingScreen(NabunassarGame game):base(game)
        {
            background = Game.Content.Load<Texture2D>("Assets/Images/Backgrounds/logo.png");
        }

        public override void Draw(GameTime gameTime)
        {
            using var sb = Game.BeginDraw();
            sb.Draw(background, Game.Resolution, new Rectangle(0, 0, 2560, 1440), Color.White);

            base.Draw(gameTime);
        }

        public override ScreenWidget GetWidget()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
