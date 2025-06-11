using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Tiled.Map;

namespace Nabunassar.Tiled.Renderer
{
    internal class TiledMapRenderer : Renderable
    {
        TiledMap _map;
        Texture2DAtlas _atlas;
        List<TiledLayerRenderer> _layers = new();
        List<TiledObjectRenderer> _objects = new();
        List<TiledPersonRenderer> _npcs = new();
        public const float TileSizeMultiplier = 3.99f;

        public TiledMapRenderer(NabunassarGame game, TiledMap map):base(game)
        {
            _map = map;
        }

        public override void LoadContent()
        {
            foreach (var tileset in _map.Tilesets)
            {
                var texture = Content.Load<Texture2D>(tileset.image);
                _atlas = Texture2DAtlas.Create(tileset.name, texture,tileset.tilewidth,tileset.tileheight);
                tileset.TextureAtlas = _atlas;
            }

            foreach (var mapLayer in _map.Layers)
            {
                var layer = new TiledLayerRenderer(Game, mapLayer);
                layer.LoadContent();
                _layers.Add(layer);
            }

            foreach (var mapObject in _map.Objects)
            {
                var obj = new TiledObjectRenderer(Game, mapObject);
                obj.LoadContent();
                _objects.Add(obj);
            }

            foreach (var mapObject in _map.NPCs)
            {
                var obj = new TiledPersonRenderer(Game, mapObject);
                obj.LoadContent();
                _npcs.Add(obj);
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var layer in _layers)
            {
                layer.Update(gameTime);
            }

            foreach (var obj in _objects)
            {
                obj.Update(gameTime);
            }

            foreach (var npc in _npcs)
            {
                npc.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch)
        {
            foreach (var layer in _layers)
            {
                layer.Draw(gameTime, spriteBatch);
            }

            foreach (var obj in _objects)
            {
                obj.Draw(gameTime, spriteBatch);
            }

            foreach (var npc in _npcs)
            {
                npc.Draw(gameTime, spriteBatch);
            }
        }
    }
}
