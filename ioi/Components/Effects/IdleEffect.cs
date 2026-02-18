using Microsoft.Xna.Framework.Graphics;

namespace ioi.Components.Effects
{
    internal class IdleEffect : ShaderEffect
    {
        private ObjectMap _obj;

        public IdleEffect(GameHost game, ObjectMap obj) : base(game)
        {
            _obj = obj;
            Effect = game.Content.Load<Effect>("Assets/Shaders/Roguelike/PlayerIdle.fx");
        }

        public override void Update(GameTime gameTime)
        {
            var tileX = _obj.Sprite.TextureRegion.X;
            var tileY = _obj.Sprite.TextureRegion.Y;
            var atlasSize = 1024f;
            var tileSize = 34f;

            this.Effect.Parameters["tileTopLeft"]?.SetValue(new Vector2(tileX / atlasSize, tileY / atlasSize));
            this.Effect.Parameters["tileSizeUV"]?.SetValue(new Vector2(tileSize / atlasSize, tileSize / atlasSize));
            this.Effect.Parameters["time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        }
    }
}
