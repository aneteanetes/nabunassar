using Nabunassar.Entities.Game;

namespace Nabunassar.Components
{
    internal class CursorComponent
    {
        public Cursor Cursor { get; private set; }
        public CursorComponent(Cursor cursor)
        {
            Cursor = cursor;
        }   
    }
}
