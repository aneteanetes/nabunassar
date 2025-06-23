using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using Nabunassar.Struct;

namespace Nabunassar.Components
{
    internal class RenderComponent : MapObject
    {
        public RenderComponent(NabunassarGame game, Sprite sprite, Vector2 position, float rotation, MapObject parent=null):base(game,position, ObjectType.Sprite,null,parent:parent)
        {
            Parent = parent;
            Sprite = sprite;
            Position = position;
            Rotation = rotation;
        }

        public Effect ColorSwapEffect { get; set; }

        public Color ColorForSwap { get; set; } = Color.White;

        public Vector2 Scale { get; set; } = Vector2.One;

        public Sprite Sprite { get; set; }

        public float Rotation { get; set; }
    }
}