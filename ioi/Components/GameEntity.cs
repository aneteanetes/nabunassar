using ioi.Scripting;
using MoonSharp.Interpreter;

namespace ioi.Components
{
    internal class GameEntity
    {
        private LuaScripts _script;
        public Table Data;

        public string Name { get; set; }

        public ObjectMap MapObject { get; set; }

        public GameEntity(LuaScripts script, params string[] templates)
        {
            _script = script;
            Data = script.CreateTable(templates);
        }

        public DynValue this[string key]
        {
            get
            {
                return Data.GetSmart(key);
            }
            set => Data.Set(key, value);
        }
    }
}
