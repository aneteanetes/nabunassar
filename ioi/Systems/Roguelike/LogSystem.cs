using ioi.Entities.Struct;
using ioi.Widgets.UserInterfaces.Roguelike;
using MoonSharp.Interpreter;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    internal class LogSystem : IDisposable
    {
        public GameHost Game { get; private set; }

        public LogWidget Widget { get; internal set; }

        public LogSystem(GameHost game)
        {
            Game = game;
        }

        public void Log(DrawText text)
        {
            Widget.SetText(text);
        }

        public void Dispose()
        {
            Widget?.Dispose();
            Game = null;
        }
    }
}