using Microsoft.Xna.Framework;
using Nabunassar.Desktops;
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
            _tiledMapRenderer = new TiledMapRenderer(_tiledMap);
        }

        public override void Update(GameTime gameTime)
        {
            _tiledMapRenderer.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var sb = Game.BeginDraw();
            _tiledMapRenderer.Draw(sb);
            base.Draw(gameTime);
        }
    }
}
