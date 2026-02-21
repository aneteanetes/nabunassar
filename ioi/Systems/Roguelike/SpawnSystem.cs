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
            var entity = new GameEntity(Game.Lua, $"Templates.Classes.{@class}")
            {
                Name = "Странник"
            };

            Console.WriteLine(entity.GetHp());

            return entity;
        }
    }
}