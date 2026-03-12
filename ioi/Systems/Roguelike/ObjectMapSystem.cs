using Geranium.Reflection;
using ioi.Components;
using ioi.Entities.Data.Loot;
using ioi.Entities.Map;
using ioi.Entities.Struct;
using ioi.Scripting;
using ioi.Tiled.Map;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MoonSharp.Interpreter;
using System.Collections;
using System.Diagnostics;

namespace ioi.Systems.Roguelike
{
    internal class ObjectMapSystem : IDisposable
    {
        public GameHost Game { get; private set; }

        public Effect Celshading { get; internal set; }

        public bool IsPaused { get; set; }

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
            Game.GameState.Map = new Entities.Map.RogueMap(map.width, map.height,Game)
            {
                NameToken = map.GetPropertyValue<string>(nameof(RogueMap.NameToken)),
                Color = map.GetPropertyValue<string>(nameof(RogueMap.Color)).AsColor(),
                LootTableName = map.GetPropertyValue<string>(nameof(RogueMap.LootTableName))
            };

            foreach (var tileset in map.Tilesets)
            {
                if (tileset.name == "Hulls")
                    continue;

                var texture = Game.Content.Load<Texture2D>(tileset.image);
                var _atlas = Texture2DAtlas.Create(tileset.name, texture, tileset.tilewidth, tileset.tileheight,spacing:tileset.spacing);
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

                Game.GameState.Tilesets[tileset.name] = Game.GameState.Map.Tilesets[tileset.name] = tileset.TextureAtlas;

                yield return 0;
            }

            map.Layers.SelectMany(layer => layer.Tiles)
                .Where(poly => poly.Gid > 0)
                .ForEach(poly =>
                {
                    //if (poly.Tileset.name == "Consolas1")
                    //    Debugger.Break();
                    var _sprite = poly.Tileset.TextureAtlas.CreateSprite(poly.Tileset.GetAtlasId(poly.Gid));
                    var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
                    var position = poly.Position;

                    var obj = new ObjectMap(Game, $"tile {poly.Coords}")
                    {
                        Sprite = _sprite,
                        Color = GetColorFromTile(poly),
                        IsBounds = poly.GetPropertyValue<bool>(nameof(ObjectMap.IsBounds)),
                        Position = position,
                        Coords = poly.Coords.ToPoint(),
                        Size = Game.CellSize.ToVector2()
                    };
                    obj.Sprite.Color = GetColorFromTile(poly);

                    Game.GameState.Map.Add(obj);
                });

            yield return 0;

            map.Objects.ForEach((poly,i) =>
            {
                if (poly.GetPropertyValue<string>("type") == "region")
                {
                    Game.GameState.Map.Regions.Add(new Area()
                    {
                        Bounds = new RectangleF(((float)poly.x), ((float)poly.y), poly.width, poly.height),
                        NameToken = poly.GetPropertyValue<string>("NameToken") ?? poly.GetPropertyValue<string>("name"),
                        Color = poly.GetPropertyValue<string>(nameof(Area.Color)).AsColor()
                    });

                    return;
                }
                if (poly.GetPropertyValue<string>("type") == "area")
                {
                    Game.GameState.Map.Areas.Add(new Area()
                    {
                        Bounds = new RectangleF(((float)poly.x), ((float)poly.y), poly.width, poly.height),
                        NameToken = poly.GetPropertyValue<string>("NameToken") ?? poly.GetPropertyValue<string>("name"),
                        Color = poly.GetPropertyValue<string>(nameof(Area.Color)).AsColor()
                    });

                    return;
                }

                var _sprite = poly.Tileset.TextureAtlas.CreateSprite(poly.Tileset.GetAtlasId(poly.gid));
                var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
                var position = poly.Position;

                var idobj = poly.GetPropertyValue<string>("idobj");

                var obj = new ObjectMap(Game,$"{idobj}{i}")
                {
                    Sprite = _sprite,
                    Color = GetColorFromTile(poly),
                    IsBounds = poly.GetPropertyValue<bool>(nameof(ObjectMap.IsBounds)),
                    Position = position,
                    Coords = poly.Coords.ToPoint(),
                    Size = Game.CellSize.ToVector2()
                };
                obj.Sprite.Color = GetColorFromTile(poly);

                var type = poly.GetPropertyValue<string>("type");
                if (type == "player")
                {
                    obj.IsIdle = true;
                    obj.IsUpdatable = true;

                    Game.GameState.Player = obj;
                    var playerEntity = Game.World.SpawnSystem.SpawnCharacter("Странник","Human", "Warrior");
                    playerEntity["type"] = DynValue.NewString("player");
                    Game.GameState.Player.BindEntity(playerEntity);

                    var other = Game.World.SpawnSystem.SpawnCharacter("Пуля", "Human", "Warrior");
                    other.Heal(1000);
                    playerEntity.Squad.Add(other);

                    Game.CameraMap.LookAt(obj.Position);
                }

                if (type.IsNotEmpty() && idobj.IsNotEmpty() && type!="player")
                {
                    //var entity = Game.GameWorld.SpawnSystem.CreateEnemy(idobj, $"Animal", $"Bruiser");
                    var entity = Game.World.SpawnSystem.SpawnObject(idobj, type, poly.ToTable(Game.Lua));
                    obj.BindEntity(entity);
                }

                Game.GameState.Map.Add(obj);
            });

            Game.GameState.Map.Init();

            Game.World.PathfindSystem = new PathfindSystem(Game.GameState.Map);

            UpdateArea();

            yield return 0;
        }

        [Obsolete("Potential performance hit")]
        public void UpdateArea()
        {
            var region = Game.GameState.Map.CurrentRegion;
            var area = Game.GameState.Map.CurrentArea;

            Game.GameState.Map.CurrentRegion = Game.GameState.Map.Regions.FirstOrDefault(x => x.Bounds.Contains(Game.GameState.Player.Position));
            Game.GameState.Map.CurrentArea = Game.GameState.Map.Areas.FirstOrDefault(x => x.Bounds.Contains(Game.GameState.Player.Position));

            if (region != Game.GameState.Map.CurrentRegion || area != Game.GameState.Map.CurrentArea)
            {
                if (Game.GameState.Map.CurrentArea == null && region == Game.GameState.Map.CurrentRegion)
                    return;

                LogArea(false);
            }
        }

        public void LogArea(bool isLogMap=true)
        {
            var str = Game.Strings["roguelike"];
            var map = Game.GameState.Map;
            var regionExists = false;

            DrawText txt = DrawText.Create(str["comingtolocation"], Color.DarkGray)
                    .AppendSpace();

            if (isLogMap)
            {
                txt.Color(map.Color)
                    .Append(str[map.NameToken])
                    .ResetColor();
            }

            if (map.CurrentRegion != null)
            {
                regionExists = true;
                if (isLogMap)
                {
                    txt.Append(" - ");
                }
                txt.Color(map.CurrentRegion.Color)
                    .Append(str[map.CurrentRegion.NameToken])
                    .ResetColor();
            }

            if (map.CurrentArea != null)
            {
                if (regionExists)
                {
                    txt.Append(" - ");
                }
                txt.Color(map.CurrentArea.Color)
                    .Append(str[map.CurrentArea.NameToken])
                    .ResetColor();
            }

            Game.World.LogSystem.Log(txt.Append("."));
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
            if (IsPaused)
                return;

            foreach (var updatable in Game.GameState.Map.Updatable)
            {
                if (Game.CameraMap.Contains(updatable.VisualBounds) == ContainmentType.Disjoint)
                {
                    if (!updatable.IsUpdateOutOfCamera)
                        continue;
                }
                updatable.Update(gameTime);
            }

            UpdateArea();
        }

        public void Draw(GameTime gameTime)
        {
            if (IsPaused)
                return;

            var mainViewport = Game.GraphicsDevice.Viewport;

            if (Game.IsBackBufferActive())
            {
                /// if backbuffer is active, then we need recalculate offset, cuz main viewport have letterboxing:
                /// 1. main viewport offset must be ignored
                /// 2. map viewport offset is 'logical', so we need keep it
                /// 3. for implement it we need to substract main viewport offset from map viewport offset (cuz main offset affected by scale)

                var mapViewport = Game.MapViewport;
                var currentOffset = new Point(mapViewport.X,mapViewport.Y);
                currentOffset -= new Point(Game.MainViewport.X, Game.MainViewport.Y);
                Game.GraphicsDevice.Viewport = new Viewport(currentOffset.X, currentOffset.Y, mapViewport.Width, mapViewport.Height);
            }
            else
            {
                Game.GraphicsDevice.Viewport = Game.MapViewport;
            }

            var defaultObjs = Game.GameState.Map.Drawable.Where(x => x.Effect == default);

            var sb = Game.BeginDraw(samplerState: SamplerState.LinearWrap, camera: Game.CameraMap, effect: Celshading);

            DrawMapObjects(sb, defaultObjs);

            sb.End();

            var affectedObjs = Game.GameState.Map.Drawable.Except(defaultObjs);
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

                var region = objMap.Sprite.TextureRegion;

                objMap.Sprite.Color = objMap.Color;
                //objMap.Sprite.Effect = objMap.Side == Struct.Side.Right ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                objMap.Sprite.Draw(sb, objMap.DrawPosition.HasValue ? objMap.DrawPosition.Value : objMap.Position, 0, Vector2.One);
            }
        }

        public void Dispose()
        {
            Game.GameState.Map = null;
            Game = null;
        }

        internal void Pause()
        {
            Game.GameState.Temp.ClickPosition = null;
            Game.World.BorderSystem["Map"] = false;
            IsPaused = true;
        }

        internal void Resume()
        {
            Game.World.BorderSystem["Map"] = true;
            IsPaused = false;
        }

        internal void Remove(ObjectMap mapObject)
        {
            Game.GameState.Map.Remove(mapObject);
        }

        internal IEnumerable<GameEntity> CollectNearest(GameEntity enemy)
        {
            var radiusValue = enemy["assemblySquadRadius"];
            if (radiusValue.IsNil() || radiusValue.Type != DataType.Number)
                return [];

            var radius = (int)Math.Round(radiusValue.Number);

            var map = Game.GameState.Map;
            var cell = map.Updatable.FirstOrDefault(x => x.Entity == enemy);
            var coords = cell.Coords;

            var xStart = Math.Clamp(coords.X - radius, 0, map.Width-1);
            var xEnd = Math.Clamp(coords.X + radius, 0, map.Width-1);

            var yStart = Math.Clamp(coords.Y - radius, 0, map.Height - 1);
            var yEnd = Math.Clamp(coords.Y + radius, 0, map.Height - 1);

            List<GameEntity> collected = new();

            for (int x = xStart; x <= xEnd; x++)
            {
                for (int y = yStart; y <= yEnd; y++)
                {
                    if (x == coords.X && y == coords.Y)
                        continue; //center

                    var inRadiusCell = map.ObjectMap[x, y];

                    foreach (var obj in inRadiusCell.Objects)
                    {
                        if (obj.Entity == null)
                            continue;

                        var typeValue = obj.Entity["type"];
                        if (typeValue.IsNil())
                            continue;

                        var type = typeValue.String;
                        if (type != "enemy")
                            continue;

                        collected.Add(obj.Entity);
                    }
                }
            }

            return collected;
        }

        public void AddObjectMap(GameEntity entity, Point coords)
        {
            var tileset = entity["tileset"].String;
            var tileId = ((int)entity["tileid"].Number);

            var obj = new ObjectMap(Game, $"{Guid.NewGuid().ToString().Substring(0, 5)}")
            {
                Sprite = Game.GameState.Map.Tilesets[tileset].CreateSprite(tileId),
                Color = entity.Color("color"),
                IsBounds = entity["isBounds"].Boolean,
                Coords = coords,
                Size = Game.CellSize.ToVector2()
            };

            obj.Position = obj.GetPositionFromCoords();
            obj.BindEntity(entity);

            Game.GameState.Map.Add(obj);
        }

        internal void AddLoot(GameEntity entity)
        {
            var player = Game.GameState.Player.Entity;
            var coords = entity.MapObject.Coords;

            var lootTableVal = entity["loottable"];

            List<GameEntity> loots = new();

            if (lootTableVal.IsNotNil())
            {
                var lootTable = new GameLootTable(lootTableVal, lootTableVal.Table.Get("loottablename").String);
                loots = lootTable.Generate(player);
            }
            else
            {
                var mapLootTable = Game.GameState.Map.LootTable;
                if (mapLootTable != default)
                {
                    loots = mapLootTable.Generate(player);
                }
            }


            foreach (var loot in loots)
            {
                AddObjectMap(loot, coords);
            }
        }

        public RogueMapCell GetCell(int x, int y)
        {
            return Game.GameState.Map.ObjectMap[x, y];
        }
    }
}