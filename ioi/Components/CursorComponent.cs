using MonoGame.Extended.Graphics;
using ioi.Entities.Game;

namespace ioi.Components
{
    internal class CursorComponent
    {
        public Cursor Cursor { get; private set; }

        public AnimatedSprite Sprite { get; set; }

        public CursorComponent(Cursor cursor)
        {
            Cursor = cursor;
        }
    }
}
