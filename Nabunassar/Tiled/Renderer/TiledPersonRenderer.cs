using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using Nabunassar.Monogame.SpriteBatch;
using Nabunassar.Tiled.Map;

namespace Nabunassar.Tiled.Renderer
{
    internal class TiledPersonRenderer : Renderable
    {
        TiledObject _object;
        AnimatedSprite _sprite;

        public TiledPersonRenderer(NabunassarGame game, TiledObject tiledObject) : base(game)
        {
            _object = tiledObject;
        }

        public override void LoadContent()
        {
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
                       .AddFrame(regionIndex: 9, duration: TimeSpan.FromSeconds(0.2))
                       .AddFrame(10, TimeSpan.FromSeconds(0.2))
                       .AddFrame(11, TimeSpan.FromSeconds(0.2))
                       .AddFrame(12, TimeSpan.FromSeconds(0.2));
            });


            _sprite = new AnimatedSprite(spriteSheet,"idle");

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _sprite = null;
        }

        public override void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch)
        {
            var pos = _object.Position * TiledMapRenderer.TileSizeMultiplier;
            
            spriteBatch.Draw(_sprite, new Vector2(pos.X,pos.Y-9*TiledMapRenderer.TileSizeMultiplier) , 0, Vector2.One * TiledMapRenderer.TileSizeMultiplier);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
