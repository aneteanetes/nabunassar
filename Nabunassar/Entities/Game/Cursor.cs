using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Nabunassar.Components;

namespace Nabunassar.Entities.Game
{
    internal class Cursor
    {
        public GameObject FocusedMapObject { get; set; }

        public SpriteSheet SpriteSheet { get; set; }

        public AnimatedSprite AnimatedSprite { get; set; }

        public RectangleF Bounds { get; set; }

        public List<SpriteSheetAnimation> Animations { get; set; } = new();

        public static Dictionary<string, MouseCursor> Cursors = new();

        public void SetCursor(string name=null)
        {
            Mouse.SetCursor(Cursors[name ?? "cursor"]);
        }

        public void DefineCursor(string name, MouseCursor mouseCursor)
        {
            Cursors[name] = mouseCursor;
        }

        public void OnCollision(CollisionEventArgs collisionInfo, MapObject host, MapObject another)
        {
            var anotherGameObject = another.GameObject;

            var mouse = MouseExtended.GetState();
            var mousePosition = mouse.Position.ToVector2();

            FocusedMapObject = anotherGameObject;
        }

        internal void OnNoCollistion()
        {

            FocusedMapObject = default;
        }
    }
}
