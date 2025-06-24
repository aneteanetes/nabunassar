using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;
using Nabunassar.Desktops;
using Nabunassar.Desktops.Menu;
using Nabunassar.Extensions.Texture2DExtensions;
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
                var texture = Content.Load<Texture2D>(tileset.image.Replace("colored-", ""));
                var _atlas = Texture2DAtlas.Create(tileset.name, texture, tileset.tilewidth, tileset.tileheight);
                tileset.TextureAtlas = _atlas;

                // glow
                int glowWidth = 1;
                float intensity = 50;
                float spread = 0;
                float totalGlowMultiplier = 1;
                bool hideTexture = false;
                var glowTexture = GlowEffect.CreateGlow(texture, Color.Yellow, glowWidth, intensity, spread, totalGlowMultiplier, hideTexture);
                var _glowAtlas = Texture2DAtlas.Create(tileset.name + "_glow", glowTexture, tileset.tilewidth, tileset.tileheight,margin: 50);
                tileset.TextureAtlasGlow = _glowAtlas;
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
                Game.EntityFactory.CreateTiledObject(mapObject);
            }

            foreach (var mapObject in _tiledMap.NPCs)
            {
                Game.EntityFactory.CreateNPC(mapObject);
            }
            
            Game.RunGameState();
        }

        private bool isEsc = false;

        private bool logWindow = false;

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (Game.DesktopWidget == default || Game.DesktopWidget.GetType()==typeof(EmptyScreenWidget) || Game.DesktopWidget.GetType()==typeof(MainMenu))
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

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.L) && keyboardState.IsControlDown())
            {
                //if (!logWindow)
                //    Game.SwitchDesktop(new LogWindow(Game));
                //else
                //    Game.SwitchDesktop();

                logWindow = !logWindow;
            }
        }
    }
}