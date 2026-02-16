using ioi.Components;
using ioi.Entities.Base;
using ioi.Tiled.Map;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended.Graphics;
using System.Collections;

namespace ioi.Systems.Roguelike
{
    internal class ObjectMapSystem
    {
        private Texture2D normalAtlas;
        private Texture2D colorAtlas;
        private Effect lightingEffect;

        public GameHost Game { get; }

        public ObjectMapSystem(GameHost game)
        {
            Game = game;
        }

        public IEnumerator LoadMap(string assetName)
        {
            normalAtlas = Game.Content.LoadTexture("Assets/Tilesets/Fonts/NormalMap1.png"); // карта нормалей
            colorAtlas = Game.Content.LoadTexture("Assets/Tilesets/Fonts/Consolas_0.png");
            // Загружаем эффект (файл .fx должен быть скомпилирован в .xnb)
            lightingEffect = Game.Content.Load<Effect>("Assets/Shaders/Celshading.fx");

            var tiledMap = Game.Content.Load<TiledMap>(assetName);
            return LoadMap(tiledMap);
        }

        public IEnumerator LoadMap(TiledMap map)
        {
            Game.GameState.Map = new Entities.Map.RogueMap();

            foreach (var tileset in map.Tilesets)
            {
                if (tileset.name == "Hulls")
                    continue;

                var texture = Game.Content.Load<Texture2D>(tileset.image);
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

                yield return 0;
            }

            Game.GameState.Map.ObjectMap =
                map.Layers.SelectMany(layer => layer.Tiles)
                .Where(poly => poly.Gid > 0)
                .Select(poly =>
                {
                    var _sprite = poly.Tileset.TextureAtlas.CreateSprite(poly.Gid - 1);
                    var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
                    var position = poly.Position;

                    return new ObjectMap
                    {
                        Sprite = _sprite,
                        Color = GetColorFromTile(poly),
                        IsBounds = poly.GetPropertyValue<bool>(nameof(ObjectMap.IsBounds)),
                        Position = position,
                        Coords = poly.Coords,
                        Size = Game.CellSize
                    };
                }).ToDictionary(obj => obj.KeyCoords(), obj => new List<ObjectMap>() { obj });

            yield return 0;

            map.Objects.ForEach(poly =>
            {
                var _sprite = poly.Tileset.TextureAtlas.CreateSprite(poly.gid - 1);
                var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
                var position = poly.Position;

                var obj = new ObjectMap
                {
                    Sprite = _sprite,
                    Color = GetColorFromTile(poly),
                    IsBounds = poly.GetPropertyValue<bool>(nameof(ObjectMap.IsBounds)),
                    Position = position,
                    Coords = poly.Coords,
                    Size = Game.CellSize
                };

                if (poly.GetPropertyValue<string>("id") == "player")
                {
                    Game.GameState.Player = obj;
                }

                Game.GameState.Map.Add(obj);
            });

            yield return 0;
        }

        private static Color GetColorFromTile(Propertied _object)
        {
            var color = Color.White;

            var hexColor = _object.GetPropertyValue<string>(nameof(Color));
            if (hexColor != default)
            {
                color = hexColor.AsColor();
            }

            return color;
        }

        public void Update(GameTime gameTime)
        {
            Game.CameraMap.Position = Game.GameState.Player.Position - new Vector2(Game.mainViewport.Width / 2f, Game.mainViewport.Height / 2f);
            //Game.CameraMap.LookAt(Game.GameState.Player.Position);
        }

        public void Draw(GameTime gameTime)
        {
            var mainViewport = Game.GraphicsDevice.Viewport;

            Game.GraphicsDevice.Viewport = Game.mapViewport;

            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            Vector3 lightDir = new Vector3(
                (float)Math.Sin(0.5f),
                (float)Math.Cos(0.5f),
                0.5f);

            lightDir.Normalize();

            var sb = Game.BeginDraw(samplerState: SamplerState.LinearWrap, camera: Game.CameraMap, effect: lightingEffect);

            //sb.Draw(colorAtlas, Vector2.Zero, Color.White);
            foreach (var objMap in Game.GameState.Map.Objects)
            {
                if (Game.CameraMap.Contains(objMap.Position + objMap.Size) == ContainmentType.Disjoint)
                {
                    continue;
                }

                if (objMap.Sprite.Color != objMap.Color)
                    objMap.Sprite.Color = objMap.Color;

                objMap.Sprite.Draw(sb, objMap.Position, 0, Vector2.One);
            }

            sb.End();

            Game.GraphicsDevice.Viewport = mainViewport;
        }
    }
}
