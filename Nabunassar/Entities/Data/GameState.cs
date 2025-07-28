using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Map;
using Nabunassar.Entities.Struct;
using Nabunassar.Tiled.Map;
using Nabunassar.Widgets.UserInterfaces;

namespace Nabunassar.Entities.Data
{
    internal class GameState
    {
        public Party Party { get; set; }

        public Cursor Cursor { get; set; } = new();

        public Action<string> OnLog { get; set; }

        public Minimap Minimap { get; set; }

        public TiledBase LoadedMap { get; set; }

        public string LoadedMapPostFix => LoadedMap.GetPropertyValue<string>("AreaObjectPostfix");

        public void Log(string message)
        {
            OnLog?.Invoke(message);
        }

        public void AddMessage(DrawText text)
        {
            ChatWindow.AddMessage(text.ToString());
        }

        public void AddRollMessage(DrawText text, RollResult rollResult)
        {
            ChatWindow.AddRollMessage(text.ToString(), rollResult);
        }
    }
}
