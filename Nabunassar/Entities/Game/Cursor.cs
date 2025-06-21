using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;

namespace Nabunassar.Entities.Game
{
    internal class Cursor
    {
        public GameObject FocusedGameObject { get; set; }

        public SpriteSheet SpriteSheet { get; set; }

        public AnimatedSprite AnimatedSprite { get; set; }

        public List<SpriteSheetAnimation> Animations { get; set; } = new();

        public void OnCollision(CollisionEventArgs collisionInfo, GameObject host, GameObject another)
        {
            FocusedGameObject = another;
        }

        public static Dictionary<string, MouseCursor> Cursors = new();

        public void SetCursor(string name)
        {
            Mouse.SetCursor(Cursors[name]);
        }

        public void DefineCursor(string name, MouseCursor mouseCursor)
        {
            Cursors[name] = mouseCursor;
        }

        internal void OnNoCollistion()
        {
            FocusedGameObject = default;
        }
    }
}
