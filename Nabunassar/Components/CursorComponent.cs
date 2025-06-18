using MonoGame.Extended.Graphics;
using Nabunassar.Entities.Game;

namespace Nabunassar.Components
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
