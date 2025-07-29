using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Screens.Abstract;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.Menu;
using Nabunassar.Widgets.UserInterfaces;
using Nabunassar.Widgets.UserInterfaces.GameWindows;

namespace Nabunassar.Screens.Game
{
    internal class MainGameScreen : BaseGameScreen
    {
        TiledMap _tiledMap;

        public MainGameScreen(NabunassarGame game) : base(game)
        {
        }

        public override void LoadContent()
        {
            Game.Camera.Zoom = 4;
            Game.Camera.Origin = new Vector2(0, 0);
            Game.Camera.Position = new Vector2(0, 0);
            Game.Camera.SetBounds(Vector2.Zero, new Vector2(205,115));

            _tiledMap = Content.Load<TiledMap>("Assets/Maps/learningarea.tmx");
            Game.GameState.LoadedMap = new TiledBase() { Properties = new Dictionary<string, string>(_tiledMap.Properties) };
            Game.EntityFactory.CreateMinimap(_tiledMap);

            foreach (var tileset in _tiledMap.Tilesets)
            {
                if (tileset.name == "Hulls")
                    continue;

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
                var sorted = _layer.Tiles.OrderBy(x => x.GetPropertyValue<GroundType>(nameof(GroundType))).ToList();
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

            InitGameUI();

            _tiledMap.Dispose();
            _tiledMap = null;
        }

        private bool isEsc = false;

        private bool logWindow = false;

        public void InitGameUI()
        {
            Game.AddDesktopWidget(new MinimapWindow(Game) { Position = new Vector2(Game.Resolution.Width, Game.Resolution.Height) });
            Game.AddDesktopWidget(new ChatWindow(Game));
            Game.AddDesktopWidget(new ControlPanel(Game));
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (Game.IsDesktopWidgetExist<MainMenu>())
                {
                    Game.RemoveDesktopWidgets<MainMenu>();
                    Game.ChangeGameActive();
                }
                else
                {
                    Game.AddDesktopWidget(new MainMenu(Game, true));
                    Game.ChangeGameActive();
                }
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
            {
                ControlPanel.OpenCloseMiniMap(Game);
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.L) && keyboardState.IsControlDown())
            {

                logWindow = !logWindow;
            }
        }
    }
}