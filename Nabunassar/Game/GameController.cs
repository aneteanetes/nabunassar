using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;
using Nabunassar.Entities.Data;
using Nabunassar.Screens.Game;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using System.Collections;

namespace Nabunassar
{
    internal static class GameController
    {
        private static NabunassarGame Game => NabunassarGame.Game;

        public static void StartNewGame(NabunassarGame game)
        {
            game.SwitchScreen<MainGameScreen>(LoadNewGame(game));
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

        private static IEnumerator LoadNewGame(NabunassarGame game)
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
            Game.MapEntityFactory.CreateMinimap(_tiledMap);

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

                    Game.MapEntityFactory.CreateTile(tile);
                }
            }

            yield return 0;

            foreach (var mapObject in _tiledMap.Objects)
            {
                Game.MapEntityFactory.CreateTiledObject(mapObject);
            }

            yield return 0;

            foreach (var mapObject in _tiledMap.NPCs)
            {
                Game.MapEntityFactory.CreateNPC(mapObject);
            }

            yield return 0;

            foreach (var mapObject in _tiledMap.Creatures)
            {
                Game.MapEntityFactory.CreateCreature(mapObject);
            }

            yield return 0;

            Game.RunGameState();
            Game.IsGameActive = true;

            yield return 0;
        }

        public static IEnumerator LoadMainGame()
        {
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
