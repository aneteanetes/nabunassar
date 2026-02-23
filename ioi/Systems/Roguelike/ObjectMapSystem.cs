using ioi.Components;
using ioi.Entities.Base;
using ioi.Entities.Map;
using ioi.Entities.Struct;
using ioi.Tiled.Map;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using System.Collections;

namespace ioi.Systems.Roguelike
{
    internal class ObjectMapSystem
    {
        private Effect lightingEffect;

        public GameHost Game { get; }

        public ObjectMapSystem(GameHost game)
        {
            Game = game;
        }

        public IEnumerator LoadMap(string assetName)
        {
            var tiledMap = Game.Content.Load<TiledMap>(assetName);

            return LoadMap(tiledMap);
        }

        public IEnumerator LoadMap(TiledMap map)
        {
            Game.GameState.Map = new Entities.Map.RogueMap()
            {
                NameToken = map.GetPropertyValue<string>(nameof(RogueMap.NameToken))
            };

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

                    var obj = new ObjectMap
                    {
                        Sprite = _sprite,
                        Color = GetColorFromTile(poly),
                        IsBounds = poly.GetPropertyValue<bool>(nameof(ObjectMap.IsBounds)),
                        Position = position,
                        Coords = poly.Coords,
                        Size = Game.CellSize
                    };
                    obj.Sprite.Color = GetColorFromTile(poly);
                    return obj;
                }).ToDictionary(obj => obj.KeyCoords(), obj => new List<ObjectMap>() { obj });

            yield return 0;

            map.Objects.ForEach(poly =>
            {
                if (poly.gid == 0)
                {
                    Game.GameState.Map.Areas.Add(new Entities.Map.Area()
                    {
                        Bounds = new RectangleF(((float)poly.x), ((float)poly.y), poly.width, poly.height),
                        NameToken = poly.GetPropertyValue<string>("NameToken")
                    });

                    return;
                }

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
                obj.Sprite.Color = GetColorFromTile(poly);

                if (poly.GetPropertyValue<string>("id") == "player")
                {
                    obj.IsIdle = true;
                    Game.GameState.Player.MapObject = obj;
                    //Game.CameraMap.Origin = new Vector2(Game.mapViewport.Width / 2f, Game.mapViewport.Height / 2f);
                    //Game.CameraMap.LookAt(obj.Position);
                }

                Game.GameState.Map.Add(obj);
            });


            Game.PathfindSystem = new PathfindSystem(Game.GameState.Map);

            UpdateArea();

            yield return 0;
        }

        [Obsolete("Potential performance hit")]
        public void UpdateArea()
        {
            Game.GameState.Map.CurrentArea = Game.GameState.Map.Areas.FirstOrDefault(x => x.Bounds.Contains(Game.GameState.Player.MapObject.Position));
        }

        public void LogArea()
        {
            var str = Game.Strings["roguelike"];
            var map = Game.GameState.Map;

            var txt = DrawText.Create(str["comingtolocation"], Color.DarkGray)
                .AppendSpace()
                .Append(str[map.NameToken]);

            if (map.CurrentArea != null)
                txt.Append(" - "+str[map.CurrentArea.NameToken]);

            Game.LogSystem.Log(txt.Append("."));
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
            Game.GameState.Map.Objects.Where(x => x.IsUpdatable)
                .ForEach(x =>
                {
                    x.Update(gameTime);
                });
            UpdateArea();
        }

        public void Draw(GameTime gameTime, Effect effect=null)
        {
            var mainViewport = Game.GraphicsDevice.Viewport;

            Game.GraphicsDevice.Viewport = Game.MapViewport;

            var defaultObjs = Game.GameState.Map.Objects.Where(x => x.Effect == default);

            var sb = Game.BeginDraw(samplerState: SamplerState.LinearWrap, camera: Game.CameraMap, effect: effect);

            DrawMapObjects(sb, defaultObjs);

            sb.End();

            var affectedObjs = Game.GameState.Map.Objects.Except(defaultObjs);
            foreach (var affectedObj in affectedObjs)
            {
                affectedObj.Effect.Draw(gameTime, affectedObj.Sprite, affectedObj.Position);
            }

            Game.GraphicsDevice.Viewport = mainViewport;
        }

        private void DrawMapObjects(Monogame.SpriteBatch.SpriteBatchKnowed sb, IEnumerable<ObjectMap> obs)
        {
            foreach (var objMap in obs)
            {
                if (Game.CameraMap.Contains(objMap.VisualBounds) == ContainmentType.Disjoint)
                {
                    continue;
                }
                //objMap.Sprite.Effect = objMap.Side == Struct.Side.Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                objMap.Sprite.Draw(sb, objMap.DrawPosition.HasValue ? objMap.DrawPosition.Value : objMap.Position, 0, Vector2.One);
            }
        }
    }
}