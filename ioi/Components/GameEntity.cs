using Geranium.Reflection;
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

        public DynValue Func(string name, params object[] args)
        {
            var func = this[name];

            if (func.IsNil() || func.Type != DataType.Function)
                return DynValue.Nil;

            return _script.Call(func, [Data, .. args]);
        }

        public Color Color(string key)
        {
            var value = this[key];
            if (value.IsNil() || value.Type!= DataType.Table)
                return Microsoft.Xna.Framework.Color.White;

            var table = value.Table;

            var r = Convert.ToByte(table[1]);
            var g = Convert.ToByte(table[2]);
            var b = Convert.ToByte(table[3]);
            var a = Convert.ToByte(table[4]);

            return new Color(r,g,b,a);
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
