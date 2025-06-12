using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;
using Nabunassar.Entities.Data;
using Nabunassar.Struct;
using Nabunassar.Tiled.Map;

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

        private static Vector2 StandartScale => Vector2.One * TileSizeMultiplier;

        public Entity CreateTile(TiledPolygon polygon)
        {
            var entity = _world.CreateEntity();

            var id = polygon.Tileset.GetAtlasId(polygon.Gid);
            var _sprite = polygon.Tileset.TextureAtlas.CreateSprite(id);
            var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height) * TileBoundsSizeMultiplier;
            var position = (polygon.Position * 16) * TileSizeMultiplier;
            var render = new RenderComponent(_sprite, position, 0, StandartScale);
            entity.Attach(render);

            var bounds = new RectangleF(position, size * 0.9f);
            var collision = CreateCollisionComponent(bounds, ObjectType.Ground, entity);
            entity.Attach(collision);

            return entity;
        }

        public Entity CreateNPC(TiledObject _object)
        {
            var entity = _world.CreateEntity();

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
            var position = _object.Position * TileSizeMultiplier;
            position = new Vector2(position.X, position.Y - 9 * TileSizeMultiplier);

            var render = new RenderComponent(_sprite, position, 0, StandartScale);
            entity.Attach(render);

            return entity;
        }

        public Entity CreateCharacter(Character character, Vector2 position)
        {
            var entity = _world.CreateEntity();
            var size = new Vector2(16, 24) * TileSizeMultiplier;


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

            var render = new RenderComponent(_sprite, position, 0, StandartScale);
            entity.Attach(render);

            var bounds = new RectangleF(new Vector2(position.X + 1, position.Y + 32), new Vector2(15 - 2, 15) * TileSizeMultiplier);
            var collision = CreateCollisionComponent(bounds, ObjectType.Player, entity, character.OnCollision);
            entity.Attach(collision);

            var charComp = new PlayerComponent(this._game, character);
            entity.Attach(charComp);

            return entity;
        }

        public Entity CreateObject(TiledObject _object)
        {
            var entity = _world.CreateEntity();

            var id = _object.Tileset.GetAtlasId(_object.gid);
            var _sprite = _object.Tileset.TextureAtlas.CreateSprite(id);
            var position = _object.Position * TileSizeMultiplier;
            var size = new Vector2(_sprite.TextureRegion.Width, _sprite.TextureRegion.Height) * TileBoundsSizeMultiplier;

            var render = new RenderComponent(_sprite, position, 0, StandartScale);
            entity.Attach(render);

            var bounds = new RectangleF(position, size);
            var collisions = CreateCollisionComponent(bounds, ObjectType.Object, entity);
            entity.Attach(collisions);

            return entity;
        }

        private CollisionsComponent CreateCollisionComponent(IShapeF bounds, ObjectType objectType, Entity host, CollisionEventHandler onCollision = null)
        {
            var component = new CollisionsComponent(bounds, objectType, host, onCollision);
            _game.CollisionComponent.Insert(component);

            return component;
        }
    }
}