using Nabunassar.Entities.Game;
using Nabunassar.Entities.Map;
using Nabunassar.Tiled.Map;

namespace Nabunassar.Entities.Data
{
    internal class GameState
    {
        public Party Party { get; set; }

        public Cursor Cursor { get; set; } = new();

        public Action<string> OnLog { get; set; }

        public Minimap Minimap { get; set; }

        public TiledMap LoadedMap { get; set; }

        public string LoadedMapPostFix => LoadedMap.GetPropertyValue<string>("AreaObjectPostfix");

        public void Log(string message)
        {
            OnLog?.Invoke(message);
        }
    }
}
