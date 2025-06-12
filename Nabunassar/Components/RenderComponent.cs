using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;

namespace Nabunassar.Components
{
    internal class RenderComponent
    {
        public RenderComponent(Sprite sprite, Vector2 position, float rotation, Vector2 scale)
        {
            Sprite = sprite;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Sprite Sprite { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public Vector2 Scale { get; set; }
    }
}
