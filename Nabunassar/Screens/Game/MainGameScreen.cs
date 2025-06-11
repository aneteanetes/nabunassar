using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Desktops;
using Nabunassar.Desktops.Menu;
using Nabunassar.Resources;
using Nabunassar.Screens.Abstract;
using Nabunassar.Tiled.Map;
using Nabunassar.Tiled.Renderer;

namespace Nabunassar.Screens.Game
{
    internal class MainGameScreen : BaseGameScreen
    {
        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;

        public MainGameScreen(NabunassarGame game) : base(game)
        {
        }

        public override ScreenWidget GetWidget()
        {
            return new EmptyScreenWidget(Game);
        }

        public override void LoadContent()
        {
            _tiledMap = Content.Load<TiledMap>("Assets/Maps/learningarea.tmx");
            _tiledMapRenderer = new TiledMapRenderer(Game, _tiledMap);
            _tiledMapRenderer.LoadContent();

            Game.InitializeGameState();
        }

        public override void UnloadContent()
        {
            _tiledMapRenderer.UnloadContent();
        }

        private bool isEsc = false;

        public override void Update(GameTime gameTime)
        {
            _tiledMapRenderer.Update(gameTime);

            var state = KeyboardExtended.GetState();

            if(state.WasKeyPressed( Microsoft.Xna.Framework.Input.Keys.Escape ))
            {
                if (!isEsc)
                {
                    isEsc = true;
                    Game.SwitchDesktop(new MainMenu(Game));
                }
                else
                {
                    isEsc = false;
                    Game.SwitchDesktop();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var sb = Game.BeginDraw();
            _tiledMapRenderer.Draw(gameTime, sb);
            sb.End();

            sb = Game.BeginDraw(false);

            sb.DrawText(Fonts.Retron,30, Game.FrameCounter.ToString(), new Vector2(1, 1), Color.Yellow);

            sb.End();


            base.Draw(gameTime);
        }
    }
}
