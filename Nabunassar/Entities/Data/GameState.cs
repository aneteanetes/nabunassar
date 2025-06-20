using Nabunassar.Entities.Game;

namespace Nabunassar.Entities.Data
{
    internal class GameState
    {
        public Party Party { get; set; }

        public Cursor Cursor { get; set; } = new();

        public Action<string> OnLog { get; set; }

        public void Log(string message)
        {
            OnLog?.Invoke(message);
        }
    }
}
