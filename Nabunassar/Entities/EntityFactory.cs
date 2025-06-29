using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.UserInterfaces;
using Penumbra;

namespace Nabunassar.Entities
{
    internal class EntityFactory
    {
        NabunassarGame _game;
        public const float TileSizeMultiplier = 3.99f;
        public const float TileBoundsSizeMultiplier = 3.8f;
        private Texture2DAtlas _cursorAtlas;

        public EntityFactory(NabunassarGame game)
        {
            this._game = game;
        }

        //private static Vector2 StandartScale => Vector2.One * TileSizeMultiplier;
        private static int PersonBoundsXOffset = 3;
        private static int PersonBoundsYOffset = 15;
        private static Vector2 PersonBoundsSize = new Vector2(10, 8);

        public Action OnAfterDraw { get; private set; }

        public Entity CreateCursor()
        {
            var entity = CreateEntity("cursor");
            var cursor = _game.GameState.Cursor;

            cursor.FocusedMapObject = null;

            var cursorImg = _game.Content.Load<Texture2D>("Assets/Images/Cursors/tile_0028.png");
            var mouseCursor = MouseCursor.FromTexture2D(cursorImg, 0, 0);
            
            cursor.DefineCursor("cursor",mouseCursor);
            cursor.SetCursor("cursor");


            var pos = Mouse.GetState().Position;

            entity.Attach(new CursorComponent(_game.GameState.Cursor));

            var cursorBounds = new RectangleF(0, 0, 4, 4);

            var gameObj = new MapObject(_game, new Vector2(pos.X,pos.Y), ObjectType.Cursor, entity, cursorBounds, "cursor", cursor.OnCollision) { Name = "cursor" };
            gameObj.IsRegisterNoCollision = true;
            gameObj.NoCollision = cursor.OnNoCollistion;
            AddCollistion(gameObj);
            entity.Attach(gameObj);

            var name = "cursorspritesheet";
            var texture = _game.Content.Load<Texture2D>("Assets/Tilesets/cursor_tilemap_packed.png");
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
            var render = new RenderComponent(_game, _sprite, position, 0);
            entity.Attach(render);

            render.Sprite.Color = GetColorFromTile(polygon);

            var bounds = new RectangleF(Vector2.Zero, size);

            var mapObject = new MapObject(_game, position, ObjectType.Ground, entity, bounds, "ground") { Name = descriptor };
            entity.Attach(mapObject);

            AddCollistion(mapObject);

            var gameObj = _game.DataBase.GetObject(polygon.GetPropopertyValue<ObjectType>(nameof(ObjectType)));
            entity.Attach(gameObj);

            var tileComp = new TileComponent(polygon);
            entity.Attach(tileComp);

            return entity;
        }

        public Entity CreateNPC(TiledObject _object)
        {
            var order = 12;
            var descriptor = "npc " + _object.gid;
            var entity = CreateEntity(descriptor,order);
            var gameObject = _game.DataBase.GetObject(_object.GetPropopertyValue<int>("ObjectId"));

            entity.Attach(gameObject);
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


            var mapObject = new MapObject(_game, position, ObjectType.NPC, entity, bounds, "objects") { Name = descriptor };
            AddCollistion(mapObject);
            entity.Attach(mapObject);
            gameObject.MapObject = mapObject;
            gameObject.Entity = entity;

            var render = new RenderComponent(_game, animatedSprite, position, 0);
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

            var glowEntity = CreateGlowOutline(_object, gameObject, descriptor, entity, position, glowAnimatedSprite,order);
            glowEntity.Attach(glowAnimatedSprite);

            return entity;
        }

        public Entity CreateParty(Party party, Vector2 position)
        {
            var descriptor = "party";
            var partyEntity = CreateEntity(descriptor);
            partyEntity.Attach(party);
            party.Entity = partyEntity;

            var bounds = new RectangleF(new Vector2(6,0), new Vector2(20, 6));

            var mapObject = new MapObject(_game, position, ObjectType.Player, partyEntity, bounds, onCollistion: party.OnCollision,isMoveable:true) { Name = descriptor };
            partyEntity.Attach(mapObject);
            AddCollistion(mapObject);

            party.MapObject= mapObject;

            var x =-4;
            var y = -12;
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

            var directionRender = new RenderComponent(_game, directionSprite, Vector2.Zero, 0);
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
            var entity = CreateEntity(descriptor, 3 + order);
            hero.Entity = entity;
            var size = new Vector2(16, 24) * TileSizeMultiplier;

            entity.Attach(new AnimatedPerson());

            var name = "SpriteSheet_" + hero.Name;
            var texture = _game.Content.Load<Texture2D>("Assets/Tilesets/" + hero.Tileset);
            var atlas = Texture2DAtlas.Create(name + Guid.NewGuid().ToString(), texture, 16, 24);
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

            var gameObject = new MapObject(_game, personalPosition, ObjectType.Hero, entity, bounds, parent: parent,isMoveable:true) { 
                Name = descriptor,
                BoundsColor = Color.Green
            };
            gameObject.RecalculatePosition();
            entity.Attach(gameObject);
            hero.GameObject = gameObject;

            gameObject.IsCustomSpeed = true;
            gameObject.MoveSpeed = .01f;

            var _sprite = new AnimatedSprite(spriteSheet, "idle");
            var render = new RenderComponent(_game, _sprite, Vector2.Zero, 0, gameObject);
            entity.Attach(render);
            entity.Attach(_sprite);

            return entity;
        }

        public Entity CreateTiledObject(TiledObject _object)
        {
            var objId = _object.GetPropopertyValue<long>(nameof(GameObject.ObjectId));
            var objType = _object.GetPropopertyValue<ObjectType>(nameof(GameObject.ObjectType));

            GameObject gameObj = null;

            if (objId != default)
                gameObj = this._game.DataBase.GetObject(objId);
            else
                gameObj = this._game.DataBase.GetObject(objType);

            var descriptor = "";

            if (objType == ObjectType.Pathing)
            {
                descriptor = $"obj path {_object.x}.{_object.y}.{_object.width}.{_object.height}";
            }
            else
            {
                descriptor = $"obj {_object.gid}";
            }

            var isHalfed = _object.GetPropopertyValue<bool>("IsHalfed");

            var order = isHalfed ? 10 : 1;

            var entity = CreateEntity(descriptor, order);
            entity.Attach(gameObj);

            entity.Attach(_object);

            var position = _object.Position;
            Vector2 size = Vector2.Zero;

            Color color = GetColorFromTile(_object);

            var id = _object.Tileset.GetAtlasId(_object.gid);
            var _sprite = _object.Tileset.TextureAtlas.CreateSprite(id);
            size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);

            _sprite.TextureRegion = new Texture2DRegion(_sprite.TextureRegion.Texture, new Rectangle(_sprite.TextureRegion.X, _sprite.TextureRegion.Y, 16, isHalfed ? 8 : 16));
            _sprite.Color = color;

            if (objType != ObjectType.Pathing)
            {
                var render = new RenderComponent(_game, _sprite, position, 0);

                entity.Attach(render);

                if (isHalfed)
                {
                    var entityDownpart = CreateEntity(descriptor + "downpart", order);
                    var spriteDownPart = new Sprite(_sprite.TextureRegion.Texture)
                    {
                        TextureRegion = new Texture2DRegion(_sprite.TextureRegion.Texture, new Rectangle(_sprite.TextureRegion.X, _sprite.TextureRegion.Y + 8, 16, 8))
                    };
                    spriteDownPart.Color = color;
                    var renderDownPart = new RenderComponent(_game, spriteDownPart, new Vector2(position.X, position.Y + 8), 0);

                    entityDownpart.Attach(renderDownPart);
                }
            }
            else
            {
                size = new Vector2(_object.width, _object.height);
                position.Y += 16;
            }

            if (objType.IsInteractive())
            {
                CreateGlowOutline(_object, gameObj, descriptor, entity, position, _object.Tileset.TextureAtlasGlow.CreateSprite(id), order);
            }

            // end glow

            var mapObject = new MapObject(_game, position, objType, entity, layer: "objects")
            {
                Name = descriptor
            };
            entity.Attach(mapObject);

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
                    var complexCollision = new MapObject(_game, gameObjectPosition, ObjectType.Object, entity, bounds, "objects")
                    {
                        Name = dummyDiscriptor
                    };
                    dummyEntity.Attach(complexCollision);
                    AddCollistion(complexCollision);
                    i++;
                }
            }
            else
            {
                mapObject.Bounds = new RectangleF(Vector2.Zero, size);
                mapObject.Position = mapObject.Position;
                AddCollistion(mapObject);
            }

            var isHavePolygons = _object.IsHavePolygons();
            if (isHavePolygons)
            {
                List<Hull> hulls = [];
                var hullPolugons = _object.GetPolygons().Where(x => x.GetPropopertyValue<ObjectType>("ObjectType") == ObjectType.Hull);
                foreach (var hullPolygon in hullPolugons)
                {
                    hulls.Add(new Hull(hullPolygon.Vertices)
                    {
                        Position = hullPolygon.Position
                    });
                }

                entity.Attach(new HullComponent(hulls.ToArray()));
            }

            var title = new FocusWidgetComponent(gameObj, focusEvent => new TitleWidget(_game, focusEvent.Object, focusEvent.Position));
            entity.Attach(title);

            return entity;
        }

        private Entity CreateGlowOutline(TiledObject _object, GameObject gameObj, string descriptor, Entity entity, Vector2 position, Sprite glowSprite, int order)
        {
            var glowEntity = CreateEntity(descriptor + " glow", order-1);
            var glowRender = new RenderComponent(_game, glowSprite, position, 0);
            glowSprite.IsVisible = false;
            glowEntity.Attach(glowRender);
            glowEntity.Attach(new FocusComponent(
                mapObj =>
                {
                    if (mapObj == gameObj && _game.IsMouseActive)
                    {
                        glowSprite.IsVisible = true;
                    }
                },
                mapObj =>
                {
                    if (mapObj == gameObj)
                    {
                        glowSprite.IsVisible = false;
                    }
                }));

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

            var hexColor = _object.GetPropopertyValue<string>(nameof(Color));
            if (hexColor != default)
            {
                color = hexColor.AsColor();
            }

            return color;
        }

        public void AddCollistion(MapObject gameObject)
        {
            _game.CollisionComponent.Insert(gameObject);
        }

        public Entity CreateEntity(string descriptor=null, int order=0)
        {
            var entity = _game.WorldGame.CreateEntity();

            entity.Attach(new DescriptorComponent(descriptor));
            entity.Attach(new OrderComponent(order));

            return entity;
        }
    }
}