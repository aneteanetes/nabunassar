using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Tiled.Map;

namespace Nabunassar.Tiled.Renderer
{
    internal class TiledObjectRenderer : Renderable
    {
        TiledObject _object;
        Sprite _sprite;

        public TiledObjectRenderer(NabunassarGame game, TiledObject @object) : base(game)
        {
            _object = @object;
        }

        public override void LoadContent()
        {
            var id = _object.Tileset.GetAtlasId(_object.gid);
            _sprite = _object.Tileset.TextureAtlas.CreateSprite(id);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _sprite = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch)
        {
            spriteBatch.Draw(_sprite, _object.Position * TiledMapRenderer.TileSizeMultiplier, 0, Vector2.One * TiledMapRenderer.TileSizeMultiplier);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
