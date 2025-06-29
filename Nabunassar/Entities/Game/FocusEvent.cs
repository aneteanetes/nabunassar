namespace Nabunassar.Entities.Game
{
    internal struct FocusEvent
    {
        public GameObject Object { get; set; }

        public bool IsFocused { get; set; }

        public Vector2 Position { get; set; }
    }
}
