using ioi.Components;
using ioi.Scripting;
using MoonSharp.Interpreter;

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
                Name = "Странник",
            };

            entity["icon"] = DynValue.NewString("@");
            entity.Color("color", Color.Red);

            Lua.Call(entity["refresh"],entity.Data);

            return entity;
        }

        public GameEntity CreateEnemy(string id, string race, string @class)
        {
            var entity = new GameEntity(Game.Lua,
                "Templates.Base.Object",
                $"Templates.Races.{race}", 
                $"Templates.Classes.{@class}",
                $"Templates.Base.Moveable",
                $"Templates.Enemies.{id}");

            Lua.Call(entity["refresh"], entity.Data);

            return entity;
        }
    }
}