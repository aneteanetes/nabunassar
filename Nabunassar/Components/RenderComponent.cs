using MonoGame.Extended.Graphics;

namespace Nabunassar.Components
{
    internal class RenderComponent : PositionComponent
    {
        public RenderComponent(NabunassarGame game, Sprite sprite, Vector2 position, float rotation):base(game)
        {
            Sprite = sprite;
            Position = position;
            Rotation = rotation;
            Data = new Color[sprite.TextureRegion.Width * sprite.TextureRegion.Height];

            sprite.TextureRegion.Texture.GetData(0, new Rectangle(sprite.TextureRegion.X, sprite.TextureRegion.Y, sprite.TextureRegion.Width, sprite.TextureRegion.Height), Data, 0, Data.Length);
        }

        public bool IsScaled { get; set; } = true;

        public Vector2 PrevPosition { get; set; }

        public Color[] Data { get; private set; }

        public Sprite Sprite { get; set; }

        public float Rotation { get; set; }
    }
}