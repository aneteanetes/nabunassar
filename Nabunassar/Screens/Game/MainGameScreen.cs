using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Screens.Abstract;
using Nabunassar.Shaders;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.UserInterfaces;
using Nabunassar.Widgets.UserInterfaces.GameWindows;
using Nabunassar.Widgets.Views.IconButtons;

namespace Nabunassar.Screens.Game
{
    internal class MainGameScreen : BaseGameScreen
    {
        public static GaussianBlur GlobalBlurShader;

        private bool logWindow = false;
        private bool _isNewGame;

        public MainGameScreen(NabunassarGame game, bool isNewGame=true) : base(game)
        {
            _isNewGame = isNewGame;
        }

        public override void LoadContent()
        {
            if (GlobalBlurShader == null)
                GlobalBlurShader = new GaussianBlur(base.Game, 1.5f);

            Game.Camera.Zoom = 4;
            Game.Camera.Origin = new Vector2(0, 0);
            Game.Camera.Position = new Vector2(0, 0);
            Game.Camera.SetBounds(Vector2.Zero, new Vector2(205, 115));

            if (_isNewGame)
                InitNewGame();

            InitGameUI();
        }

        private void InitNewGame()
        {
            var _tiledMap = Game.Content.Load<TiledMap>("Assets/Maps/learningarea.tmx");
            Game.GameState.Location = new Entities.Data.Locations.Location(Game)
            {
                Region = Entities.Data.Locations.Region.Underdead,
                LoadedMap = new TiledBase() { Properties = new Dictionary<string, string>(_tiledMap.Properties) },
            };
            Game.EntityFactory.CreateMinimap(_tiledMap);

            foreach (var tileset in _tiledMap.Tilesets)
            {
                if (tileset.name == "Hulls")
                    continue;

                var texture = Game.Content.Load<Texture2D>(tileset.image.Replace("colored-", ""));
                var _atlas = Texture2DAtlas.Create(tileset.name, texture, tileset.tilewidth, tileset.tileheight);
                tileset.TextureAtlas = _atlas;

                // glow
                int glowWidth = 1;
                float intensity = 50;
                float spread = 0;
                float totalGlowMultiplier = 1;
                bool hideTexture = false;
                var glowTexture = GlowEffect.CreateGlow(texture, Color.Yellow, glowWidth, intensity, spread, totalGlowMultiplier, hideTexture);
                var _glowAtlas = Texture2DAtlas.Create(tileset.name + "_glow", glowTexture, tileset.tilewidth, tileset.tileheight, margin: 50);
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
        }

        public void InitGameUI()
        {
            Game.AddDesktopWidget(new MinimapWindow(Game) { Position = new Vector2(Game.Resolution.Width, Game.Resolution.Height) });
            Game.AddDesktopWidget(new ChatWindow(Game));
            Game.AddDesktopWidget(new ControlPanel(Game));
            Game.AddDesktopWidget(new GameDateTime(Game));
            Game.AddDesktopWidget(new PartyConditions(Game));
            Game.AddDesktopWidget(new PartyPlates(Game));
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (!Game.GameState.EscapeSwitch)
                    SettingsIconButton.OpenCloseSettings(Game);
                else
                    Game.GameState.EscapeSwitch = false;
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                Console.WriteLine("XNA_space");
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.PrintScreen))
            {
                Console.WriteLine("XNA_printscreen");
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
            {
                MinimapIconButton.OpenCloseMiniMap(Game);
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.I))
            {
                InventoryIconButton.OpenCloseInventory(Game);
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
            {
                AbilityIconButton.OpenCloseAbilities(Game);
            }

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.L) && keyboardState.IsControlDown())
            {

                logWindow = !logWindow;
            }
        }

        public override void UnloadContent()
        {
            GlobalBlurShader.Disable();
            Game.DisposeGameWorld();
            Game.RemoveDesktopWidgets(true);

            var game = Game;
            game.Camera.Zoom = 1;
            game.Camera.Origin = new Vector2(0, 0);
            game.Camera.Position = new Vector2(0, 0);
        }
    }
}