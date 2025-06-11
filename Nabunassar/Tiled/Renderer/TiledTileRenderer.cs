using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Tiled.Map;

namespace Nabunassar.Tiled.Renderer
{
    internal class TiledTileRenderer : Renderable
    {
        TiledPolygon _tiledPolygon;
        Sprite _sprite;

        public TiledTileRenderer(NabunassarGame game, TiledPolygon tiledPolygon) : base(game)
        {
            _tiledPolygon = tiledPolygon;
        }

        public override void LoadContent()
        {
            var id = _tiledPolygon.Tileset.GetAtlasId(_tiledPolygon.Gid);
            _sprite = _tiledPolygon.Tileset.TextureAtlas.CreateSprite(id);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _sprite = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch)
        {
            spriteBatch.Draw(_sprite, (_tiledPolygon.Position * 16)* TiledMapRenderer.TileSizeMultiplier, 0,Vector2.One* TiledMapRenderer.TileSizeMultiplier);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
