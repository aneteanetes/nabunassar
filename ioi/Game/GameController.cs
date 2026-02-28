using ioi.Entities.Data;
using ioi.Entities.Game;
using ioi.Screens;
using ioi.Screens.Game;
using ioi.Shaders.PostProceessing;
using ioi.Struct;
using ioi.Systems;
using ioi.Systems.Roguelike;
using ioi.Tiled.Map;
using ioi.Widgets.Views.IconButtons;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Shapes;
using System.Collections;

namespace ioi
{
    internal static class GameController
    {
        public static GaussianBlur GlobalBlurShader;

        private static GameHost Game => GameHost.Game;

        public static void StartNewGame(GameHost game)
        {
            game.SwitchScreen<RoguelikeScreen>(NewRoguelike(game));
        }

        public static void StartCombat(GameObject gameObject, Side playerSide)
        {
            Game.SwitchScreen(new CombatGameScreen(Game, gameObject, playerSide), LoadCombat());
        }

        public static void Exit()
        {
            static IEnumerator Exiting()
            {
                Game.Exit();
                yield return 0;
            }

            Game.SwitchScreen<MainGameScreen>(Exiting());
        }

        public static void GlobalMenuWidget()
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                if (!Game.GameState.EscapeSwitch)
                    SettingsIconButton.OpenCloseSettings(Game);
                else
                    Game.GameState.EscapeSwitch = false;
            }
        }

        public static void SetCameraToWorld()
        {
            Game.CameraMain.Zoom = 4;
            Game.CameraMain.Origin = new Vector2(0, 0);
            Game.CameraMain.Position = new Vector2(0, 0);
            //Game.CameraMain.SetBounds(Vector2.Zero, new Vector2(205, 115));
        }

        public static void SetCameraToScreen()
        {
            Game.CameraMain.Zoom = 1;
            Game.CameraMain.Origin = new Vector2(0, 0);
            Game.CameraMain.Position = new Vector2(0, 0);
            //Game.CameraMain.SetBoundsFree();
        }

        private static IEnumerator LoadCombat()
        {
            yield return 0;
        }

        private static IEnumerator NewRoguelike(GameHost gameHost)
        {
            Game.InitGameWorld();
            Game.InitializeGameState();

            Game.World = new GameWorld(Game)
            {
                MapSystem = new ObjectMapSystem(Game),
                SpawnSystem = new SpawnSystem(Game),
                PlayerControlSystem = new PlayerControlSystem(Game),
                LogSystem = new LogSystem(Game),
                BorderLayersSystem = new BorderLayersSystem(Game),
                CombatSystem = new CombatSystem(Game),
                LoadingSystem= new LoadingSystem(Game),
            };

            Game.Lua.Globals["world"] = Game.World;

            yield return Game.World.LoadContent();
            yield return Game.World.MapSystem.LoadMap("Assets/Maps/maraumir3.tmx");

            Game.IsGameActive = true;
            yield return 0;
        }

        private static IEnumerator LoadNewGame(GameHost game)
        {
            Game.InitGameWorld();
            Game.InitializeGameState();

            yield return 0;

            var _tiledMap = Game.Content.Load<TiledMap>("Assets/Maps/learningarea.tmx");
            Game.GameState.Location = new Entities.Data.Locations.Location(Game)
            {
                Region = Entities.Data.Locations.Region.Underdead,
                LoadedMap = new TiledBase() { Properties = new Dictionary<string, string>(_tiledMap.Properties) },
            };
            Game.EntityFactoryMap.CreateMinimap(_tiledMap);

            yield return 0;

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

            yield return 0;

            foreach (var _layer in _tiledMap.Layers)
            {
                var sorted = _layer.Tiles.OrderBy(x => x.GetPropertyValue<GroundType>(nameof(GroundType))).ToList();
                foreach (var tile in sorted) // slower ground put last
                {
                    if (tile.Gid == 0)
                        continue;

                    Game.EntityFactoryMap.CreateTile(tile);
                }
            }

            yield return 0;

            foreach (var mapObject in _tiledMap.Objects)
            {
                Game.EntityFactoryMap.CreateTiledObject(mapObject);
            }

            yield return 0;

            foreach (var mapObject in _tiledMap.NPCs)
            {
                Game.EntityFactoryMap.CreateNPC(mapObject);
            }

            yield return 0;

            foreach (var mapObject in _tiledMap.Creatures)
            {
                Game.EntityFactoryMap.CreateCreature(mapObject);
            }

            yield return 0;

            Game.RunGameState();
            Game.IsGameActive = true;

            yield return 0;
        }

        public static void GlobalBlurShaderReset(bool isEnable=false)
        {
            if (Game == null || Game.Content == null)
                return;

            if (GlobalBlurShader == null)
            {
                GlobalBlurShader = new GaussianBlur(Game, 1.5f);

                if (isEnable)
                    GlobalBlurShader.Enable();
                return;
            }

            var prevEnabled = GlobalBlurShader?.Enabled ?? false;
            Game.DisablePostProcessors();
            GlobalBlurShader = null;

            GlobalBlurShaderReset(prevEnabled);
        }

        public static IEnumerator LoadMainGame()
        {
            if (GlobalBlurShader == null)
                GlobalBlurShader = new GaussianBlur(Game, 1.5f);

            Game.IsGameActive = true; 
            yield return 0;
        }

        internal static IEnumerator UnloadGame()
        {
            Game.RemoveDesktopWidgets(true);
            yield return 0;

            Game.DisposeGameWorld();
            yield return 0;
        }
    }
}
