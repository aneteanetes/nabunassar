using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;
using Nabunassar.Entities.Data;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;
using SharpFont;

namespace Nabunassar.Entities
{
    internal class EntityFactory
    {
        World _world;
        NabunassarGame _game;
        public const float TileSizeMultiplier = 3.99f;
        public const float TileBoundsSizeMultiplier = 3.8f;

        public EntityFactory(NabunassarGame game)
        {
            _game = game;
            _world = game.World;
        }

        //private static Vector2 StandartScale => Vector2.One * TileSizeMultiplier;
        private static int PersonBoundsXOffset = 3;
        private static int PersonBoundsYOffset = 15;
        private static Vector2 PersonBoundsSize = new Vector2(10, 8);

        public Entity CreateCursor()
        {
            var entity = CreateEntity("cursor");

            _game.GameState.Cursor.Entity = entity;

            var cursorImg = _game.Content.Load<Texture2D>("Assets/Images/Cursors/tile_0028.png");
            var mouseCursor = MouseCursor.FromTexture2D(cursorImg, 0, 0);
            Mouse.SetCursor(mouseCursor);

            var pos = Mouse.GetState().Position;

            entity.Attach(new CursorComponent(_game.GameState.Cursor));

            var collision = CreateCollisionComponent(new RectangleF(pos.X, pos.Y, 4, 4), ObjectType.Cursor, entity,"cursor", _game.GameState.Cursor.OnCollision);
            entity.Attach(collision);

            return entity;
        }

        public Entity CreateTile(TiledPolygon polygon)
        {
            var entity = CreateEntity("tile "+polygon.Gid);

            var id = polygon.Tileset.GetAtlasId(polygon.Gid);
            var _sprite = polygon.Tileset.TextureAtlas.CreateSprite(id);
            var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
            var position = polygon.Position;
            var render = new RenderComponent(_game, _sprite, position, 0);
            entity.Attach(render);

            var bounds = new RectangleF(position, size);
            var collision = CreateCollisionComponent(bounds, ObjectType.Ground, entity, "ground");
            entity.Attach(collision);

            var tileComp = new TileComponent(polygon);
            entity.Attach(tileComp);

            return entity;
        }

        public Entity CreateNPC(TiledObject _object)
        {
            var entity = CreateEntity("npc "+_object.gid);

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


            var _sprite = new AnimatedSprite(spriteSheet, "idle");
            var position = _object.Position;
            position = new Vector2(position.X, position.Y - 9);

            var bounds = new RectangleF(new Vector2(position.X + PersonBoundsXOffset, position.Y + PersonBoundsYOffset), PersonBoundsSize);
            var collision = CreateCollisionComponent(bounds, ObjectType.NPC, entity,"objects");
            entity.Attach(collision);

            var render = new RenderComponent(_game, _sprite, position, 0);
            entity.Attach(render);

            return entity;
        }

        public Entity CreateCharacter(Character character, Vector2 position)
        {
            var entity = CreateEntity("player");
            character.Entity = entity;
            var size = new Vector2(16, 24) * TileSizeMultiplier;

            var boundRender = new BoundRenderPositionComponent();
            boundRender.RenderOffset = new Vector2(PersonBoundsYOffset,PersonBoundsYOffset);
            entity.Attach(boundRender);

            var name = "SpriteSheet_" + character.Name;
            var texture = _game.Content.Load<Texture2D>("Assets/Tilesets/" + character.Tileset);
            var atlas = Texture2DAtlas.Create(name + Guid.NewGuid().ToString(), texture, 16, 24);
            SpriteSheet spriteSheet = new SpriteSheet("SpriteSheet_" + character.Name, atlas);

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
                       .AddFrame(regionIndex: 8, duration: TimeSpan.FromSeconds(0.1))
                       .AddFrame(9, TimeSpan.FromSeconds(0.1))
                       .AddFrame(10, TimeSpan.FromSeconds(0.1))
                       .AddFrame(11, TimeSpan.FromSeconds(0.1));
            });


            var _sprite = new AnimatedSprite(spriteSheet, "idle");
            var render = new RenderComponent(_game, _sprite, position, 0);
            entity.Attach(render);

            var bounds = new RectangleF(new Vector2(position.X + PersonBoundsXOffset, position.Y + PersonBoundsYOffset), PersonBoundsSize);
            var collision = CreateCollisionComponent(bounds, ObjectType.Player, entity,null, character.OnCollision);
            entity.Attach(collision);

            var charComp = new PlayerComponent(this._game, character);
            entity.Attach(charComp);

            var moveComp = new MoveComponent(this._game);
            entity.Attach(moveComp);

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

            var entity = CreateEntity(descriptor);
            var position = _object.Position;
            Vector2 size = Vector2.Zero;

            if (objType != ObjectType.Pathing)
            {

                var id = _object.Tileset.GetAtlasId(_object.gid);
                var _sprite = _object.Tileset.TextureAtlas.CreateSprite(id);
                size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height);

                var render = new RenderComponent(_game,_sprite, position, 0);
                entity.Attach(render);

            }
            else
            {
                size = new Vector2(_object.width, _object.height);
                position.Y += 16;
            }

            var haveBounds = _object.IsHaveBounds();

            if (haveBounds)
            {
                var i = 0;
                foreach (var bound in _object.GetBounds())
                {
                    var boundPosition = new Vector2(position.X + bound.X, position.Y + bound.Y);
                    var boundSize = new SizeF(bound.Width, bound.Height);
                    var bounds = new RectangleF(boundPosition, boundSize);

                    var dummyEntity = CreateEntity($"obj {_object.gid} bound({i})");
                    var collisions = CreateCollisionComponent(bounds, ObjectType.Object, dummyEntity, "objects");
                    dummyEntity.Attach(collisions);
                    i++;
                }
            }
            else
            {
                var bounds = new RectangleF(position, size);
                var collisions = CreateCollisionComponent(bounds, ObjectType.Object, entity, "objects");
                entity.Attach(collisions);
            }

            return entity;
        }

        private BoundsComponent CreateCollisionComponent(RectangleF bounds, ObjectType objectType, Entity host,string layer=null, CollisionEventHandler onCollision = null)
        {
            var component = new BoundsComponent(_game, bounds, objectType, host,layer, onCollision);
            _game.CollisionComponent.Insert(component);

            return component;
        }

        private Entity CreateEntity(string descriptor=null)
        {
            var entity = _world.CreateEntity();

            entity.Attach(new DescriptorComponent(descriptor));

            return entity;
        }
    }
}