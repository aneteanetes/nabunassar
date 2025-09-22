using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Entities.Game;
using Nabunassar.Screens.Abstract;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar.Screens.Game
{
    internal class CombatGameScreen : BaseGameScreen
    {
        private Texture2D _background;

        public CombatGameScreen(NabunassarGame game, GameObject enemy) : base(game)
        {
        }

        public override void LoadContent()
        {
            GameController.SetCameraToWorld();

            _background = Content.LoadTexture("Assets/Images/Backgrounds/Combat/underdead3");

            InitGameUI();

            base.LoadContent();
        }

        public void InitGameUI()
        {
            Game.AddDesktopWidget(new ControlPanel(Game,false));
            Game.AddDesktopWidget(new GameDateTime(Game));
            Game.AddDesktopWidget(new PartyConditions(Game));
        }

        public override void Update(GameTime gameTime)
        {
            GameController.CallGlobalMenu();

            if (!Game.IsGameActive)
                return;

            Game.CombatWorld?.Update(gameTime);
        }

        protected override void DrawInternal(GameTime gameTime)
        {
            var sb = Game.BeginDraw();
            sb.Draw(_background, new Rectangle(Point.Zero, (Game.Resolution.ToVector2() / Game.Camera.Zoom).ToPoint()), Color.White);
            sb.End();

            Game.CombatWorld?.Draw(gameTime);

            Game.SpriteBatch.End();
        }
    }
}