using ioi.Entities.Struct;
using ioi.Widgets.UserInterfaces.Roguelike;

namespace ioi.Systems.Roguelike
{
    internal class LogSystem
    {
        public GameHost Game { get; }

        public LogWidget Widget { get; internal set; }

        public LogSystem(GameHost game)
        {
            Game = game;
        }

        public void Log(DrawText text)
        {
            Widget.SetText(text);
        }
    }
}