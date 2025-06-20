using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Components.Inactive;
using Nabunassar.Desktops;
using Nabunassar.Desktops.Menu;
using Nabunassar.Screens.Abstract;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;

namespace Nabunassar.Screens.Game
{
    internal class MainGameScreen : BaseGameScreen
    {
        TiledMap _tiledMap;

        public MainGameScreen(NabunassarGame game) : base(game)
        {
        }

        public override ScreenWidget GetWidget()
        {
            return new EmptyScreenWidget(Game);
        }

        public override void LoadContent()
        {
            Game.Camera.Zoom = 4;
            Game.Camera.Origin = new Vector2(0, 0);
            Game.Camera.Position = new Vector2(0, 0);

            _tiledMap = Content.Load<TiledMap>("Assets/Maps/learningarea.tmx");

            foreach (var tileset in _tiledMap.Tilesets)
            {
                var texture = Content.Load<Texture2D>(tileset.image);
                var _atlas = Texture2DAtlas.Create(tileset.name, texture, tileset.tilewidth, tileset.tileheight);
                tileset.TextureAtlas = _atlas;
            }

            foreach (var _layer in _tiledMap.Layers)
            {
                var sorted = _layer.Tiles.OrderBy(x => x.GetPropopertyValue<GroundType>(nameof(GroundType))).ToList();
                foreach (var tile in sorted) // slower ground put last
                {
                    if (tile.Gid == 0)
                        continue;

                    Game.EntityFactory.CreateTile(tile);
                }
            }

            foreach (var mapObject in _tiledMap.Objects)
            {
                Game.EntityFactory.CreateObject(mapObject);
            }

            foreach (var mapObject in _tiledMap.NPCs)
            {
                Game.EntityFactory.CreateNPC(mapObject);
            }
            
            Game.RunGameState();
        }

        private bool isEsc = false;

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (!isEsc)
                {
                    isEsc = true;
                    Game.SwitchDesktop(new MainMenu(Game));
                    Game.ChangeGameActive();
                }
                else
                {
                    isEsc = false;
                    Game.SwitchDesktop();
                    Game.ChangeGameActive();
                }
            }
        }
    }
}