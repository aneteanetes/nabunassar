using MonoGame.Extended.Graphics;
using Nabunassar.Struct;

namespace Nabunassar.Components
{
    internal class RenderComponent : GameObject
    {
        public RenderComponent(NabunassarGame game, Sprite sprite, Vector2 position, float rotation, GameObject parent=null):base(game,position, ObjectType.Sprite,null,parent:parent)
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