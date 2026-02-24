using Geranium.Reflection;
using ioi.Scripting;
using MoonSharp.Interpreter;

namespace ioi.Components
{
    internal class GameEntity
    {
        private LuaScripts _script;
        public Table Data;

        public IEnumerable<string> Components
        {
            get
            {
                var comps = Data.Get("_components");
                if (!comps.IsNil())
                {
                    return comps.Table.Values.Select(x => x.String);
                }

                return [];
            }
        }

        public string Name { get; set; }

        public ObjectMap MapObject { get; set; }

        public List<GameEntity> Squad { get; } = new(); 

        public GameEntity(LuaScripts script, params string[] templates)
        {
            _script = script;
            Data = script.CreateTable(templates);
            Squad.Add(this);
        }

        public DynValue Func(string name, params object[] args)
        {
            var func = this[name];

            if (func.IsNil() || func.Type != DataType.Function)
                return DynValue.Nil;

            return _script.Call(func, [Data, .. args]);
        }

        public Color Color(string key, Color? set=default)
        {
            if (set.HasValue)
            {
                var color = set.Value;

                var colorTable = new Table(_script.ScriptHost);
                colorTable.Set("r", DynValue.NewNumber(color.R));
                colorTable.Set("g", DynValue.NewNumber(color.G));
                colorTable.Set("b", DynValue.NewNumber(color.B));
                colorTable.Set("a", DynValue.NewNumber(255));

                this[key] = DynValue.NewTable(colorTable);

                return set.Value;
            }
            else
            {
                var value = this[key];
                if (value.IsNil() || value.Type != DataType.Table)
                    return Microsoft.Xna.Framework.Color.White;

                var table = value.Table;

                var fromstring = Convert.ToByte(table["r"]);
                if(fromstring!=0)
                {
                    var r = Convert.ToByte(table["r"]);
                    var g = Convert.ToByte(table["g"]);
                    var b = Convert.ToByte(table["b"]);
                    var a = Convert.ToByte(table["a"]);

                    return new Color(r, g, b, a);
                }
                else
                {
                    var r = Convert.ToByte(table[1]);
                    var g = Convert.ToByte(table[2]);
                    var b = Convert.ToByte(table[3]);
                    var a = Convert.ToByte(table[4]);

                    if (a == 0)
                        a = 255;

                    return new Color(r, g, b, a);
                }
            }
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
