using ioi.Components;
using ioi.Scripting;

namespace ioi.Systems.Roguelike
{
    internal class SpawnSystem
    {
        public GameHost Game { get; }

        public LuaScripts Lua => Game.Lua;

        public SpawnSystem(GameHost game)
        {
            Game = game;
        }

        public GameEntity CreateCharacter(string race, string @class)
        {
            var entity = new GameEntity(Game.Lua, "Templates.Base.Object", $"Templates.Races.{race}", $"Templates.Classes.{@class}")
            {
                Name = "Странник"
            };

            var result = Lua.Call(entity["refresh"],entity.Data);

            var z = entity["stats.def"];

            return entity;
        }
    }
}