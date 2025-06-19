using MonoGame.Extended.Graphics;

namespace Nabunassar.Components
{
    internal class RenderComponent : PositionComponent
    {
        public RenderComponent(NabunassarGame game, Sprite sprite, Vector2 position, float rotation, PositionComponent parent=null):base(game)
        {
            Parent = parent;
            Sprite = sprite;
            Position = position;
            Rotation = rotation;
        }

        public Vector2 Scale { get; set; } = Vector2.One;

        public Sprite Sprite { get; set; }

        public float Rotation { get; set; }
    }
}