using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using Nabunassar.Struct;

namespace Nabunassar.Components
{
    internal class RenderComponent : MapObject
    {
        public RenderComponent(NabunassarGame game, Sprite sprite, Vector2 position, float rotation=default, MapObject parent=null):base(game,position, ObjectType.Sprite,null,parent:parent)
        {
            Parent = parent;
            Sprite = sprite;
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// Признак, что этот renderable эффект, который можно отключать
        /// </summary>
        public bool IsEffect { get; set; }

        public Effect Effect { get; set; }

        public Color ColorForSwap { get; set; } = Color.White;

        public Vector2 Scale { get; set; } = Vector2.One;

        public Sprite Sprite { get; set; }

        public float Rotation { get; set; }
    }
}