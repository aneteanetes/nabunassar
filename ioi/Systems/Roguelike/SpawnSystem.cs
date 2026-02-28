using ioi.Components;
using ioi.Scripting;
using MoonSharp.Interpreter;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    internal class SpawnSystem
    {
        public GameHost Game { get; }

        public LuaScripts Lua => Game.Lua;

        public SpawnSystem(GameHost game)
        {
            Game = game;
        }

        public GameEntity SpawnCharacter(string name, string race, string @class)
        {
            var entity = new GameEntity(Game.Lua,null,
                "Templates.Base.Object",
                "Templates.Base.Player",
                $"Templates.Races.{race}", 
                $"Templates.Classes.{@class}")
            {
                Name = name,
            };

            entity["icon"] = DynValue.NewString("@");
            entity["type"] = DynValue.NewString("player");
            entity.Color("color", Color.Cyan);
            entity["namevalue"] = DynValue.NewString(entity.Name);
            entity.Func("refresh");

            return entity;
        }

        public GameEntity SpawnObject(string id, string type, Table props)
        {
            var entity = new GameEntity(Game.Lua, props, "Templates.Base.Object", $"Templates.{type}.{id}");

            return entity;
        }

        public ObjectMap SpawnObjectMap(string type, string id, Table props, int x, int y, string tileset, int tileId)
        {
            var entity = SpawnObject(id, type, props);

            var obj = new ObjectMap(Game, $"{type}.{id}.{Guid.NewGuid().ToString().Substring(0,5)}")
            {
                Sprite = Game.GameState.Map.Tilesets[tileset].CreateSprite(tileId),
                Color = entity.Color("color"),
                IsBounds = entity["isBounds"].Boolean,
                Coords = new Point(x,y),
                Size = Game.CellSize.ToVector2()
            };

            obj.Position = obj.GetPositionFromCoords();
            obj.BindEntity(entity);

            Game.GameState.Map.Add(obj);

            return obj;
        }
    }
}