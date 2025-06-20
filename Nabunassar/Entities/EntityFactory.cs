using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;
using Nabunassar.Entities.Data;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using static Assimp.Metadata;

namespace Nabunassar.Entities
{
    internal class EntityFactory
    {
        World _world;
        NabunassarGame game;
        public const float TileSizeMultiplier = 3.99f;
        public const float TileBoundsSizeMultiplier = 3.8f;
        private Texture2DAtlas _cursorAtlas;

        public EntityFactory(NabunassarGame game)
        {
            this.game = game;
            _world = game.World;
        }

        //private static Vector2 StandartScale => Vector2.One * TileSizeMultiplier;
        private static int PersonBoundsXOffset = 3;
        private static int PersonBoundsYOffset = 15;
        private static Vector2 PersonBoundsSize = new Vector2(10, 8);

        public Entity CreateCursor()
        {
            var entity = CreateEntity("cursor");
            var cursor = game.GameState.Cursor;

            cursor.FocusedGameObject = null;

            var cursorImg = game.Content.Load<Texture2D>("Assets/Images/Cursors/tile_0028.png");
            var mouseCursor = MouseCursor.FromTexture2D(cursorImg, 0, 0);
            
            cursor.DefineCursor("cursor",mouseCursor);
            cursor.SetCursor("cursor");


            var pos = Mouse.GetState().Position;

            entity.Attach(new CursorComponent(game.GameState.Cursor));

            var cursorBounds = new RectangleF(0, 0, 4, 4);

            var gameObj = new GameObject(game, new Vector2(pos.X,pos.Y), ObjectType.Cursor, entity, cursorBounds, "cursor", cursor.OnCollision) { Name = "cursor" };
            AddCollistion(gameObj);
            entity.Attach(gameObj);

            var name = "cursorspritesheet";
            var texture = game.Content.Load<Texture2D>("Assets/Images/Cursors/cursor_tilemap_packed.png");
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
            var render = new RenderComponent(game, _sprite, position, 0);
            entity.Attach(render);

            var bounds = new RectangleF(Vector2.Zero, size);

            var gameObject = new GameObject(game, position, ObjectType.Ground, entity, bounds, "ground") { Name = descriptor };
            entity.Attach(gameObject);

            AddCollistion(gameObject);

            var tileComp = new TileComponent(polygon);
            entity.Attach(tileComp);

            return entity;
        }

        public Entity CreateNPC(TiledObject _object)
        {
            var descriptor = "npc " + _object.gid;
            var entity = CreateEntity(descriptor,2);

            entity.Attach(new AnimatedPerson());

            var id = _object.Tileset.GetAtlasId(_object.gid);
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
            position = new Vector2(position.X, position.Y - 9);

            var bounds = new RectangleF(new Vector2(PersonBoundsXOffset, PersonBoundsYOffset), PersonBoundsSize);


            var gameObject = new GameObject(game, position, ObjectType.NPC, entity, bounds, "ground") { Name = descriptor };
            AddCollistion(gameObject);
            entity.Attach(gameObject);

            var render = new RenderComponent(game, animatedSprite, position, 0);
            entity.Attach(render);

            return entity;
        }

        public Entity CreateParty(Party party, Vector2 position)
        {
            var descriptor = "party";
            var partyEntity = CreateEntity(descriptor);
            partyEntity.Attach(party);
            party.Entity = partyEntity;

            var bounds = new RectangleF(Vector2.Zero, new Vector2(18, 8));

            var gameObject = new GameObject(game, position, ObjectType.Player, partyEntity, bounds, onCollistion: party.OnCollision,isMoveable:true) { Name = descriptor };
            partyEntity.Attach(gameObject);
            AddCollistion(gameObject);

            var x = -8;
            var y = -12;
            var i = 1;
            foreach (var hero in party.Reverse())
            {
                CreateHero(hero, new Vector2(x, y), gameObject, i);
                x += 6;
                i++;
            }

            gameObject.OnMoving = party.OnMoving;

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

            var directionRender = new RenderComponent(game, directionSprite, Vector2.Zero, 0);
            directionRender.Scale = Vector2.One * 0.5f;
            directionEntity.Attach(directionRender);

            party.DirectionRender = directionRender;
            gameObject.OnStopMove += () => directionSprite.IsVisible = false;

            //moveComp.DirectionEntity = directionEntity;

            return partyEntity;
        }

        public Entity CreateHero(Hero hero, Vector2 personalPosition, GameObject parent, int order)
        {
            var descriptor = "hero" + personalPosition.X;
            var entity = CreateEntity(descriptor, 3 + order);
            hero.Entity = entity;
            var size = new Vector2(16, 24) * TileSizeMultiplier;

            entity.Attach(new AnimatedPerson());

            var name = "SpriteSheet_" + hero.Name;
            var texture = game.Content.Load<Texture2D>("Assets/Tilesets/" + hero.Tileset);
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

            var gameObject = new GameObject(game, personalPosition, ObjectType.Hero, entity, bounds, parent: parent,isMoveable:true) { 
                Name = descriptor,
                BoundsColor = Color.Green
            };
            gameObject.RecalculatePosition();
            entity.Attach(gameObject);
            hero.GameObject = gameObject;

            gameObject.IsCustomSpeed = true;
            gameObject.MoveSpeed = .01f;

            var _sprite = new AnimatedSprite(spriteSheet, "idle");
            var render = new RenderComponent(game, _sprite, Vector2.Zero, 0, gameObject);
            entity.Attach(render);
            entity.Attach(_sprite);

            return entity;
        }

        public Entity CreateObject(TiledObject _object)
        {
            var objType = _object.GetPropopertyValue<ObjectType>("ObjectType");

            var descriptor = "";

            if(objType == ObjectType.Pathing)
            {
                descriptor = $"obj path {_object.x}.{_object.y}.{_object.width}.{_object.height}";
            }
            else
            {
                descriptor = $"obj {_object.gid}";
            }

            var isHalfed = _object.GetPropopertyValue<bool>("IsHalfed");

            var entity = CreateEntity(descriptor, isHalfed ? 10 : 1);
            var position = _object.Position;
            Vector2 size = Vector2.Zero;

            if (objType != ObjectType.Pathing)
            {

                var id = _object.Tileset.GetAtlasId(_object.gid);
                var _sprite = _object.Tileset.TextureAtlas.CreateSprite(id);
                size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);

                _sprite.TextureRegion = new Texture2DRegion(_sprite.TextureRegion.Texture, new Rectangle(_sprite.TextureRegion.X, _sprite.TextureRegion.Y, 16, isHalfed ? 8 : 16));
                var render = new RenderComponent(game,_sprite, position, 0);
                entity.Attach(render);

                if (isHalfed)
                {
                    var entityDownpart = CreateEntity(descriptor + "downpart", 1);
                    var spriteDownPart = new Sprite(_sprite.TextureRegion.Texture)
                    {
                        TextureRegion = new Texture2DRegion(_sprite.TextureRegion.Texture, new Rectangle(_sprite.TextureRegion.X, _sprite.TextureRegion.Y + 8, 16, 8))
                    };
                    var renderDownPart = new RenderComponent(game, spriteDownPart, new Vector2(position.X, position.Y + 8), 0);

                    entityDownpart.Attach(renderDownPart);
                }

            }
            else
            {
                size = new Vector2(_object.width, _object.height);
                position.Y += 16;
            }

            var gameObject = new GameObject(game, position, objType, entity, layer: "objects") { Name = descriptor };
            entity.Attach(gameObject);

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
                    var complexCollision = new GameObject(game, gameObjectPosition, ObjectType.Object,dummyEntity,bounds,"objects") { Name = dummyDiscriptor };
                    dummyEntity.Attach(complexCollision);
                    AddCollistion(complexCollision);
                    i++;
                }
            }
            else
            {
                gameObject.Bounds = new RectangleF(Vector2.Zero, size);
                gameObject.Position = gameObject.Position;
                AddCollistion(gameObject);
            }

            return entity;
        }

        public void AddCollistion(GameObject gameObject)
        {
            game.CollisionComponent.Insert(gameObject);
        }

        public Entity CreateEntity(string descriptor=null, int order=0)
        {
            var entity = _world.CreateEntity();

            entity.Attach(new DescriptorComponent(descriptor));
            entity.Attach(new OrderComponent(order));

            return entity;
        }
    }
}