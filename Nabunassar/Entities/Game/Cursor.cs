using Microsoft.Xna.Framework.Input;
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

        public List<SpriteSheetAnimation> Animations { get; set; } = new();

        public static Dictionary<string, MouseCursor> Cursors = new();

        public void SetCursor(string name)
        {
            Mouse.SetCursor(Cursors[name]);
        }

        public void DefineCursor(string name, MouseCursor mouseCursor)
        {
            Cursors[name] = mouseCursor;
        }

        public static Queue<FocusEvent> FocusEvents = new();

        public void OnCollision(CollisionEventArgs collisionInfo, MapObject host, MapObject another)
        {
            var anotherGameObject = another.GameObject;

            var mouse = MouseExtended.GetState();
            var mousePosition = mouse.Position.ToVector2();

            if (FocusedMapObject == default)
            {
                FocusEvents.Enqueue(new FocusEvent()
                {
                    IsFocused = true,
                    Object = anotherGameObject,
                    Position = mousePosition
                });
            }
            else if (FocusedMapObject != anotherGameObject)
            {
                FocusEvents.Enqueue(new FocusEvent()
                {
                    IsFocused = false,
                    Object = FocusedMapObject,
                    Position = mousePosition
                });

                FocusEvents.Enqueue(new FocusEvent()
                {
                    IsFocused = true,
                    Object = anotherGameObject,
                    Position = mousePosition
                });
            }

            FocusedMapObject = anotherGameObject;
        }

        internal void OnNoCollistion()
        {
            if (FocusedMapObject != default)
                FocusEvents.Enqueue(new FocusEvent()
                {
                    IsFocused = false,
                    Object = FocusedMapObject,
                });

            FocusedMapObject = default;
        }
    }
}
