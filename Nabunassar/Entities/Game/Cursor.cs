using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using Nabunassar.Components;

namespace Nabunassar.Entities.Game
{
    internal class Cursor
    {
        public GameObject FocusedMapObject { get; set; }

        public static Action<GameObject> OnObjectFocused { get; set; } = x => { };

        public static Action<GameObject> OnObjectUnfocused { get; set; } = x => { };

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

        public void OnCollision(CollisionEventArgs collisionInfo, MapObject host, MapObject another)
        {
            var anotherGameObject = another.GameObject;
            if (FocusedMapObject == default)
            {
                OnObjectFocused?.Invoke(anotherGameObject);
            }
            else

            if (FocusedMapObject != anotherGameObject)
            {
                OnObjectUnfocused?.Invoke(FocusedMapObject);
                OnObjectFocused?.Invoke(anotherGameObject);
            }

            FocusedMapObject = anotherGameObject;
        }

        internal void OnNoCollistion()
        {
            if (FocusedMapObject != default)
                OnObjectUnfocused?.Invoke(FocusedMapObject);
            FocusedMapObject = default;
        }
    }
}
