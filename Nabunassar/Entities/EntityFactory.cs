using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;
using Nabunassar.Components.Effects;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Map;
using Nabunassar.Monogame.Extended;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.UserInterfaces;
using Penumbra;

namespace Nabunassar.Entities
{
    internal class EntityFactory
    {
        NabunassarGame Game;
        public const float TileSizeMultiplier = 3.99f;
        public const float TileBoundsSizeMultiplier = 3.8f;
        private Texture2DAtlas _cursorAtlas;

        public EntityFactory(NabunassarGame game)
        {
            this.Game = game;
        }

        //private static Vector2 StandartScale => Vector2.One * TileSizeMultiplier;
        private static int PersonBoundsXOffset = 3;
        private static int PersonBoundsYOffset = 15;
        private static Vector2 PersonBoundsSize = new Vector2(10, 8);

        public const int HeroRenderHeight = 24;
        public const int HeroRenderWidth = 16;
        public const int PartyRenderXOffset = 4;
        public const int PartyRenderYOffset = 12;
        public const int HeroRenderWidthOffset = 8;

        public const int PartyBoundRenderOffsetX = 6;

        public Action OnAfterDraw { get; private set; }

        public Entity CreateCursor()
        {
            var entity = CreateEntity(CollisionLayers.Cursor);
            var cursor = Game.GameState.Cursor;

            cursor.FocusedGameObject = null;

            var cursorImg = Game.Content.Load<Texture2D>("Assets/Images/Cursors/tile_0028.png");
            var cursorDefault = MouseCursor.FromTexture2D(cursorImg, 0, 0);

            var cursorEnterTexture = Game.Content.Load<Texture2D>("Assets/Images/Cursors/tile_0035.png");
            var cursorEnter = MouseCursor.FromTexture2D(cursorEnterTexture, 0, 0);

            var cursorInfoTexture = Game.Content.Load<Texture2D>("Assets/Images/Cursors/tile_0033.png");
            var cursorInfo = MouseCursor.FromTexture2D(cursorInfoTexture, 0, 0);

            var cursorSpeakTexture = Game.Content.Load<Texture2D>("Assets/Images/Cursors/tile_0050.png");
            var cursorSpeak = MouseCursor.FromTexture2D(cursorSpeakTexture, 0, 0);

            var cursorHand = MouseCursor.FromTexture2D(Game.Content.Load<Texture2D>("Assets/Images/Cursors/tile_0176.png"), 0, 0);

            cursor.DefineCursor("cursor", cursorDefault);

            cursor.DefineCursor("enter", cursorEnter);
            cursor.DefineCursor("info", cursorInfo);
            cursor.DefineCursor("speak", cursorSpeak);
            cursor.DefineCursor("hand", cursorHand);

            cursor.SetCursor("cursor");


            var pos = Mouse.GetState().Position;

            entity.Attach(new CursorComponent(Game.GameState.Cursor));

            var cursorBounds = new RectangleF(0, 0, 4, 4);
            cursor.Bounds = cursorBounds;

            var gameObj = new MapObject(Game, new Vector2(pos.X,pos.Y), ObjectType.Cursor, entity, cursorBounds, CollisionLayers.Cursor, cursor.OnCollision) { Name = "cursor" };
            gameObj.IsRegisterNoCollision = true;
            gameObj.NoCollision = cursor.OnNoCollistion;
            AddCollistion(gameObj);
            entity.Attach(gameObj);

            var name = "cursorspritesheet";
            var texture = Game.Content.Load<Texture2D>("Assets/Tilesets/cursor_tilemap_packed.png");
            _cursorAtlas = Texture2DAtlas.Create(name+"atlas", texture, 16, 16);
            SpriteSheet spriteSheet = new SpriteSheet(name, _cursorAtlas);

            spriteSheet.DefineAnimation("cursor", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(1, TimeSpan.FromSeconds(0.2))
                    .AddFrame(2, TimeSpan.FromSeconds(0.2));
            });

            spriteSheet.DefineAnimation("busy", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(189, TimeSpan.FromSeconds(0.2))
                    .AddFrame(190, TimeSpan.FromSeconds(0.2))
                    .AddFrame(191, TimeSpan.FromSeconds(0.2))
                    .AddFrame(192, TimeSpan.FromSeconds(0.2))
                    .AddFrame(196, TimeSpan.FromSeconds(0.2))
                    .AddFrame(195, TimeSpan.FromSeconds(0.2))
                    .AddFrame(193, TimeSpan.FromSeconds(0.2))
                    .AddFrame(194, TimeSpan.FromSeconds(0.2));
            });

            cursor.SpriteSheet = spriteSheet;

            cursor.Animations.Add(spriteSheet.GetAnimation("busy"));

            var _sprite = new AnimatedSprite(spriteSheet, "cursor");
            cursor.AnimatedSprite = _sprite;

            return entity;
        }

        public Entity CreateTile(TiledPolygon polygon)
        {
            var descriptor = "tile " + polygon.Gid;
            var entity = CreateEntity(descriptor);

            var id = polygon.Tileset.GetAtlasId(polygon.Gid);
            var _sprite = polygon.Tileset.TextureAtlas.CreateSprite(id);
            var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
            var position = polygon.Position;
            var render = new RenderComponent(Game, _sprite, position, 0);
            entity.Attach(render);


            render.Sprite.Color = GetColorFromTile(polygon);

            var bounds = new RectangleF(Vector2.Zero, size);

            var mapObject = new MapObject(Game, position, ObjectType.Ground, entity, bounds, CollisionLayers.Ground) { Name = descriptor };
            entity.Attach(mapObject);

            AddCollistion(mapObject);

            var objectType = polygon.GetPropertyValue<ObjectType>(nameof(ObjectType));
            var groundType = polygon.GetPropertyValue<GroundType>(nameof(GroundType));

            var gameObj = objectType == ObjectType.Ground
                ? Game.DataBase.GetObjectGround(groundType)
                : Game.DataBase.GetObject(objectType);

            gameObj.MergeProperties(polygon);
            gameObj.MapObject = mapObject;
            entity.Attach(gameObj);

            gameObj.Entity = entity;

            if (objectType == ObjectType.Ground)
            {
                AddOnMinimap(entity.Id,position, ObjectType.Ground,null, groundType);
            }

            var tileComp = new TileComponent(polygon.CopyBase());
            entity.Attach(tileComp);

            return entity;
        }

        public Entity CreateNPC(TiledObject _object)
        {
            var order = 12;
            var descriptor = "npc " + _object.gid;
            var entity = CreateEntity(descriptor,order);
            var gameObject = Game.DataBase.GetObject(_object.GetPropertyValue<int>("ObjectId"));

            AddOnMinimap(entity.Id, _object.Position, ObjectType.NPC, gameObject.Name);

            gameObject.MergeProperties(_object);

            entity.Attach(gameObject);
            entity.Attach(_object.As<TiledBase>());
            entity.Attach(new AnimatedPerson());

            SpriteSheet spriteSheet = new SpriteSheet("SpriteSheet_" + _object.Tileset.name, _object.Tileset.TextureAtlas);

            spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                       .AddFrame(regionIndex: 4, duration: TimeSpan.FromSeconds(0.2))
                       .AddFrame(5, TimeSpan.FromSeconds(0.2))
                       .AddFrame(6, TimeSpan.FromSeconds(0.2))
                       .AddFrame(7, TimeSpan.FromSeconds(0.2));
            });


            spriteSheet.DefineAnimation("run", builder =>
            {
                builder.IsLooping(true)
                       .AddFrame(regionIndex: 8, duration: TimeSpan.FromSeconds(0.2))
                       .AddFrame(9, TimeSpan.FromSeconds(0.2))
                       .AddFrame(10, TimeSpan.FromSeconds(0.2))
                       .AddFrame(11, TimeSpan.FromSeconds(0.2));
            });


            var animatedSprite = new AnimatedSprite(spriteSheet, "idle");
            entity.Attach(animatedSprite);

            var position = _object.Position;
            position = new Vector2(position.X, position.Y - 8);

            var bounds = new RectangleF(new Vector2(PersonBoundsXOffset, PersonBoundsYOffset), PersonBoundsSize);


            var mapObject = new MapObject(Game, position, ObjectType.NPC, entity, bounds, CollisionLayers.Objects) { Name = descriptor };
            AddCollistion(mapObject);
            entity.Attach(mapObject);
            gameObject.MapObject = mapObject;
            gameObject.Entity = entity;

            var render = new RenderComponent(Game, animatedSprite, position, 0);
            entity.Attach(render);

            //glow 

            SpriteSheet glowSpriteSheet = new SpriteSheet("SpriteSheet_" + _object.Tileset.name+"glow", _object.Tileset.TextureAtlasGlow);

            glowSpriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                       .AddFrame(regionIndex: 4, duration: TimeSpan.FromSeconds(0.2))
                       .AddFrame(5, TimeSpan.FromSeconds(0.2))
                       .AddFrame(6, TimeSpan.FromSeconds(0.2))
                       .AddFrame(7, TimeSpan.FromSeconds(0.2));
            });


            glowSpriteSheet.DefineAnimation("run", builder =>
            {
                builder.IsLooping(true)
                       .AddFrame(regionIndex: 8, duration: TimeSpan.FromSeconds(0.2))
                       .AddFrame(9, TimeSpan.FromSeconds(0.2))
                       .AddFrame(10, TimeSpan.FromSeconds(0.2))
                       .AddFrame(11, TimeSpan.FromSeconds(0.2));
            });


            var glowAnimatedSprite = new AnimatedSprite(glowSpriteSheet, "idle");

            var title = new FocusWidgetComponent(gameObject, focusEvent => new TitleWidget(Game, focusEvent.Object.GetObjectNameTitle(), focusEvent.Position));
            entity.Attach(title);

            var glowEntity = CreateGlowOutline(_object, gameObject, descriptor, entity, position, glowAnimatedSprite,order, title);
            glowEntity.Attach(glowAnimatedSprite);


            return entity;
        }

        public static int HeroInPartyOffset = 8;

        public Entity CreateParty(Party party, Vector2 position)
        {
            var descriptor = "party";
            var partyEntity = CreateEntity(descriptor);
            partyEntity.Attach(party);

            party.Entity = partyEntity;

            AddOnMinimap(partyEntity.Id, position, ObjectType.Player, Game.Strings["UI"]["You"]);

            var bounds = new RectangleF(new Vector2(PartyBoundRenderOffsetX, 0), new Vector2(20, 6));

            var mapObject = new MapObject(Game, position, ObjectType.Player, partyEntity, bounds,CollisionLayers.Player, onCollistion: party.OnCollision,isMoveable:true) { Name = descriptor };
            partyEntity.Attach(mapObject);
            AddCollistion(mapObject);

            mapObject.OnStopMove += party.OnStopMoving;

            party.MapObject= mapObject;

            var x = PartyRenderXOffset * -1;
            var y = PartyRenderYOffset * -1;
            var i = 1;
            foreach (var hero in party.Reverse())
            {
                CreateHero(hero, new Vector2(x, y), mapObject, i);
                x += 8;
                i++;
            }

            var directionEntity = CreateEntity("direction entity",20);
            var directionMoveComponent = new DirectionMoveComponent();
            directionEntity.Attach(directionMoveComponent);

            var directionMoveCompSpriteSheet = new SpriteSheet(nameof(directionMoveComponent), _cursorAtlas);
            directionMoveCompSpriteSheet.DefineAnimation("moving", builder =>
            {
                builder.IsLooping(true)
                    .AddFrame(57 - 2, TimeSpan.FromSeconds(0.2))
                    .AddFrame(56 - 2, TimeSpan.FromSeconds(0.2));
            });
            var directionSprite = new AnimatedSprite(directionMoveCompSpriteSheet, "moving");
            directionSprite.IsVisible = false;

            var directionRender = new RenderComponent(Game, directionSprite, Vector2.Zero, 0);
            directionRender.Scale = Vector2.One * 0.5f;
            directionEntity.Attach(directionRender);

            party.DirectionRender = directionRender;
            mapObject.OnStopMove += () => directionSprite.IsVisible = false;

            var light = new LightComponent(PartyLight = new PointLight
            {
                Scale = new Vector2(250f), // Range of the light source (how far the light will travel)
                ShadowType = ShadowType.Illuminated, // Will not lit hulls themselves
                Radius = 80,
            });
            partyEntity.Attach(light);

            //moveComp.DirectionEntity = directionEntity;

            return partyEntity;
        }

        public PointLight PartyLight { get; set; }

        public Entity CreateHero(Hero hero, Vector2 personalPosition, MapObject parent, int order)
        {
            var descriptor = "hero" + personalPosition.X;
            var entity = CreateEntity(descriptor, 3 + order); // from 4 to 8
            hero.Entity = entity;
            var size = new Vector2(16, 24) * TileSizeMultiplier;

            entity.Attach(new AnimatedPerson());

            var name = "SpriteSheet_" + hero.Name;
            var texture = Game.Content.Load<Texture2D>("Assets/Tilesets/" + hero.Tileset);
            var atlas = Texture2DAtlas.Create(name + Guid.NewGuid().ToString(), texture, HeroRenderWidth, HeroRenderHeight);
            var spriteSheet = new SpriteSheet("SpriteSheet_" + hero.Name, atlas);

            spriteSheet.DefineAnimation("idle", builder =>
            {
                builder.IsLooping(true)
                       .AddFrame(7, TimeSpan.FromSeconds(0.2))
                       .AddFrame(8, TimeSpan.FromSeconds(0.2))
                       .AddFrame(9, TimeSpan.FromSeconds(0.2))
                       .AddFrame(10, TimeSpan.FromSeconds(0.2));
            });


            spriteSheet.DefineAnimation("run", builder =>
            {
                builder.IsLooping(true)
                       .AddFrame(14, TimeSpan.FromSeconds(0.1))
                       .AddFrame(15, TimeSpan.FromSeconds(0.1))
                       .AddFrame(16, TimeSpan.FromSeconds(0.1))
                       .AddFrame(17, TimeSpan.FromSeconds(0.1));
            });

            var bounds = new RectangleF(4, 18, 8, 6);

            var gameObject = new MapObject(Game, personalPosition, ObjectType.Hero, entity, bounds, parent: parent,isMoveable:true) { 
                Name = descriptor,
                BoundsColor = Color.Green
            };
            gameObject.RecalculatePosition();
            entity.Attach(gameObject);
            hero.GameObject = gameObject;

            gameObject.IsCustomSpeed = true;
            gameObject.MoveSpeed = .01f;

            var _sprite = new AnimatedSprite(spriteSheet, "idle");
            hero.Sprite = _sprite;
            var render = new RenderComponent(Game, _sprite, Vector2.Zero, 0, gameObject);
            entity.Attach(render);
            entity.Attach(_sprite);

            return entity;
        }

        public Entity CreateTiledObject(TiledObject _object)
        {
            var objId = _object.GetPropertyValue<long>(nameof(GameObject.ObjectId));
            var objType = _object.GetPropertyValue<ObjectType>(nameof(GameObject.ObjectType));

            GameObject gameObj = null;

            if (objId != default)
                gameObj = this.Game.DataBase.GetObject(objId);
            else
                gameObj = this.Game.DataBase.GetObject(objType);

            gameObj.MergeProperties(_object);

            objType = gameObj.ObjectType;

            var descriptor = "";

            if (objType == ObjectType.Pathing)
            {
                descriptor = $"obj path {_object.x}.{_object.y}.{_object.width}.{_object.height}";
            }
            else
            {
                descriptor = $"obj {_object.gid}";
            }

#warning ishalfed disabled
            var isHalfed = false;// _object.GetPropertyValue<bool>("IsHalfed");

            var order = isHalfed ? 10 : 1;

            var entity = CreateEntity(descriptor, order);
            entity.Attach(gameObj);
            entity.Attach(_object.As<TiledBase>());

            var position = _object.Position;
            Vector2 size = Vector2.Zero;

            // mapObj

            bool isVisible = gameObj.RevealComplexity == null;

            var mapObject = new MapObject(Game, position, objType, entity, layer: isVisible ? CollisionLayers.Objects : CollisionLayers.Hidden)
            {
                Name = descriptor,
                IsVisible = isVisible
            };
            entity.Attach(mapObject);

            // minimap
            mapObject.MinimapPoint = AddOnMinimap(entity.Id, position, objType.IsInteractive() ? ObjectType.Object : ObjectType.Border, isVisible: isVisible);

            Color color = GetColorFromTile(_object);

            var id = _object.Tileset.GetAtlasId(_object.gid);
            var _sprite = _object.Tileset.TextureAtlas.CreateSprite(id);
            size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);

            _sprite.TextureRegion = new Texture2DRegion(_sprite.TextureRegion.Texture, new Rectangle(_sprite.TextureRegion.X, _sprite.TextureRegion.Y, 16, isHalfed ? 8 : 16));
            _sprite.Color = color;

            if (objType != ObjectType.Pathing)
            {
                var render = new RenderComponent(Game, _sprite, position, 0);

                entity.Attach(render);

                if (isHalfed)
                {
                    var entityDownpart = CreateEntity(descriptor + "downpart", 1);
                    var spriteDownPart = new Sprite(_sprite.TextureRegion.Texture)
                    {
                        TextureRegion = new Texture2DRegion(_sprite.TextureRegion.Texture, new Rectangle(_sprite.TextureRegion.X, _sprite.TextureRegion.Y + 8, 16, 8))
                    };
                    spriteDownPart.Color = color;
                    var renderDownPart = new RenderComponent(Game, spriteDownPart, new Vector2(position.X, position.Y + 8), 0);

                    entityDownpart.Attach(renderDownPart);

                    var downMapObj = new MapObject(Game, Vector2.Zero, ObjectType.None, entityDownpart);
                    downMapObj.Parent = mapObject;
                    mapObject.Dependant.Add(downMapObj);
                    downMapObj.OnDestroy += () => entityDownpart.Destroy();
                }
            }
            else
            {
                size = new Vector2(_object.width, _object.height);
                position.Y += 16;
            }

            // end glow

            if (gameObj != default)
            {
                gameObj.MapObject = mapObject;
                gameObj.Entity = entity;
            }

            var isHaveBounds = _object.IsHaveBounds();
            if (isHaveBounds)
            {
                var i = 0;
                foreach (var bound in _object.GetBounds())
                {
                    var gameObjectPosition = new Vector2(position.X + bound.X, position.Y + bound.Y);
                    var boundPosition = new Vector2(bound.X, bound.Y);
                    var boundSize = new SizeF(bound.Width, bound.Height);
                    var bounds = new RectangleF(Vector2.Zero, boundSize);

                    var dummyDiscriptor = $"obj {_object.gid} bound({i})";
                    var dummyEntity = CreateEntity(dummyDiscriptor);
                    var complexCollision = new MapObject(Game, gameObjectPosition, ObjectType.Object, entity, bounds, isVisible ? CollisionLayers.Objects : CollisionLayers.Hidden)
                    {
                        Name = dummyDiscriptor
                    };
                    dummyEntity.Attach(complexCollision);

                    //if (isVisible)
                        AddCollistion(complexCollision);
                    i++;

                    complexCollision.OnDestroy = () => Game.DestoryEntity(dummyEntity);
                    complexCollision.Parent = mapObject;
                    mapObject.Dependant.Add(complexCollision);
                }
                mapObject.Bounds = new RectangleF(position, size);
            }
            else
            {
                mapObject.Bounds = new RectangleF(Vector2.Zero, size);
                mapObject.Position = mapObject.Position;

                //if (isVisible)
                    AddCollistion(mapObject);
            }

            var isHavePolygons = _object.IsHavePolygons();
            if (isHavePolygons)
            {
                List<Hull> hulls = [];
                var hullPolugons = _object.GetPolygons().Where(x => x.GetPropertyValue<ObjectType>("ObjectType") == ObjectType.Hull);
                foreach (var hullPolygon in hullPolugons)
                {
                    hulls.Add(new Hull(hullPolygon.Vertices)
                    {
                        Position = hullPolygon.Position
                    });
                }

                entity.Attach(new HullComponent(hulls.ToArray()));
            }

            // FOCUS

            if (gameObj.ObjectType.IsInteractive())
            {
                var focusWidgetComp = new FocusWidgetComponent(gameObj, focusEvent => new TitleWidget(Game, focusEvent.Object.GetObjectNameTitle(), focusEvent.Position));
                CreateGlowOutline(_object, gameObj, descriptor, entity, position, _object.Tileset.TextureAtlasGlow.CreateSprite(id), 1, focusWidgetComp);
                entity.Attach(focusWidgetComp);
            }

            return entity;
        }

        private Entity CreateGlowOutline(TiledObject _object, GameObject gameObj, string descriptor, Entity entity, Vector2 position, Sprite glowSprite, int order, FocusWidgetComponent focusWidgetComponent)
        {
            var glowEntity = CreateEntity(descriptor + " glow", order-1);
            var glowRender = new RenderComponent(Game, glowSprite, position, 0);
            glowRender.IsEffect = true;
            glowSprite.IsVisible = false;
            glowEntity.Attach(glowRender);

            focusWidgetComponent.OnFocus = mapObj =>
                {
                    if (mapObj == gameObj && Game.IsMouseMoveAvailable)
                    {
                        glowSprite.IsVisible = true;
                    }
                };
            focusWidgetComponent.OnUnfocus = mapObj =>
                {
                    if (mapObj == gameObj)
                    {
                        glowSprite.IsVisible = false;
                    }
                };

            gameObj.Dependant.Add(new GameObject()
            {
                Entity = glowEntity
            });

            //Cursor.OnObjectFocused
            //    Cursor.OnObjectUnfocused

            //if (false) //glow flickering is disabled
            //{
            //    glowSprite.Alpha = .5f;

            //    var glowFlicker = new FlickeringComponent(1, 2, 100, 0.5, 1);
            //    glowFlicker.GameObject = gameObj;
            //    glowFlicker.OnChange = value =>
            //    {
            //        glowSprite.Alpha = (float)value;
            //    };
            //    entity.Attach(glowFlicker);

            //    Cursor.OnObjectFocused += mapObj =>
            //    {
            //        if (mapObj == gameObj)
            //        {
            //            glowFlicker.IsActive = true;
            //            glowFlicker.CurrentStep = 0;
            //        }
            //    };

            //    Cursor.OnObjectUnfocused += mapObj =>
            //    {
            //        if (mapObj == gameObj)
            //        {
            //            glowFlicker.IsActive = false;
            //        }
            //    };
            //}

            return glowEntity;
        }

        private static Color DefaultColor = "#cfc6b8".AsColor();

        private static Color GetColorFromTile(TiledBase _object)
        {
            var color = DefaultColor;

            var hexColor = _object.GetPropertyValue<string>(nameof(Color));
            if (hexColor != default)
            {
                color = hexColor.AsColor();
            }

            return color;
        }

        public void AddCollistion(MapObject gameObject)
        {
            Game.CollisionComponent.Insert(gameObject);
        }

        public void RemoveCollistion(MapObject gameObject)
        {
            Game.CollisionComponent.Remove(gameObject);
        }

        public MinimapPoint AddOnMinimap(int entityId, Vector2 gamePosition, ObjectType objectType, string toolTip = default, GroundType groundType = GroundType.Dirt, bool isVisible=true)
        {
            var minimap = Game.GameState.Minimap;

            var point = new MinimapPoint()
            {
                Position = minimap.TransformPosition(gamePosition),
                Name = toolTip ?? Guid.NewGuid().ToString(),
                ObjectType = objectType,
                EntityId = entityId,
                GroundType = groundType,
                IsVisible = isVisible
            };
            minimap.Add(point);

            return point;
        }

        internal void CreateMinimap(TiledMap _tiledMap)
        {

            var mapSize = new Vector2(_tiledMap.width * _tiledMap.tilewidth, _tiledMap.height * _tiledMap.tileheight);
            var miniMapSize = new Vector2(_tiledMap.width, _tiledMap.height);

            var minimap = new Minimap(mapSize,miniMapSize);
            minimap.AreaName =  Game.Strings["AreaNames"][_tiledMap.GetPropertyValue<string>("AreaName")];

            var entity = CreateEntity("minimap", 100);
            entity.Attach(minimap);

            minimap.Texture = new RenderTarget2D(Game.GraphicsDevice, ((int)minimap.MapSize.X), ((int)minimap.MapSize.Y));

            Game.GameState.Minimap = minimap;
        }

        internal void AttachEffect<T>(Entity entity, T effect)
            where T : ShaderEffectComponent
        {
            entity.Attach(effect as ShaderEffectComponent);
        }

        public Entity CreateEntity(string descriptor=null, int order=0)
        {
            var entity = Game.WorldGame.CreateEntity();

            entity.Attach(new DescriptorComponent(descriptor));
            entity.Attach(new OrderComponent(order));

            return entity;
        }
    }
}