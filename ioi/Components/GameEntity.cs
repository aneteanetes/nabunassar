using FontStashSharp.Interfaces;
using FontStashSharp.RichText;
using Geranium.Reflection;
using ioi.Scripting;
using MonoGame.Extended.ECS;
using MoonSharp.Interpreter;
using SharpFont;

namespace ioi.Components
{
    public class GameEntity
    {
        private LuaScripts _script;
        public Table Data;

        public string Name { get; set; }

        public GameEntitySquad Squad { get; internal set; }

        internal ObjectMap MapObject { get; set; }

        public IEnumerable<GameEntity> Abilities
        {
            get
            {
                var abils = this.Func("getAbilities");
                if (abils.IsNil())
                    return [];

                return abils.Table.Values.Select(x => x.UserData.Object.As<GameEntity>());
            }
        }

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

        public GameEntity(LuaScripts script,Table initProps=null, params string[] templates)
        {
            _script = script;
            Data = script.CreateTable(initProps, templates);
            Data["Destroy"] = (Action)Destroy;
            Data["entity"] = this;
            Squad = new GameEntitySquad(this);
        }

        /// <summary>
        /// First argument self
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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
                return ColorFromTable(table);
            }
        }
        public static Color ColorFromTable(Table table)
        {
            var isrgb = table.Keys.Any(x => x.String == "r");
            if (isrgb)
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

        public static string ColorFromTableToHex(Table table)
        {
            return ColorFromTable(table).ToHexString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx">From 1</param>
        /// <returns></returns>
        public GameEntity GetAbility(int idx)
        {
            var abilValue = this.Func("getAbility", idx);

            if (abilValue.IsNotNil())
            {
                return abilValue.UserData.Object.As<GameEntity>();
            }

            return null;
        }

        internal string GetAbilityName(int idx)
        {
            var abilValue = this.Func("getAbility", idx);

            if (abilValue.IsNotNil())
            {
                var entity = abilValue.UserData.Object.As<GameEntity>();
                var name = entity.GetName();// abtable.Get("name").String;
                var rescolor = this.Color("rescolor").ToHexString();
                var cost = entity["cost"].Number;

                var costtext = $" /c[{rescolor}][{cost}]";

                if (entity["mode"].String == "passive")
                    costtext = string.Empty;

                return $"{name}{costtext}";
            }

            return "...";
        }

        public DynValue this[string key]
        {
            get
            {
                return Data.GetSmart(key);
            }
            set => Data.Set(key, value);
        }

        public string GetName()
        {
            if(this.Name.IsNotEmpty())
                return this.Name;

            var nameValue = this["name"];
            if (nameValue.IsNil())
                return "#";

            var nameToken = nameValue.String;

            return _script.Game.Strings["Roguelike"][nameToken];
        }

        public string GetNameColored()
        {
            return Func("coloredName").String;
        }

        public void Heal(int heal, GameEntity healer = null)
        {
            this.Func("applyheal", heal, healer, null);
        }

        public void Destroy()
        {
            Func("destroy");

            if (this.MapObject != null)
                _script.Game.World.MapSystem.Remove(this.MapObject);

            Squad?.Remove(this);

            MapObject = null;
            Data = null;
            _script = null;
            Squad = null;
        }

        /// <summary>
        /// Based on negative hp entity gets some condition: unconscious, dead, destroying
        /// </summary>
        internal void Unconscious()
        {
            IsUnconscious = true;
        }

        /// <summary>
        /// Is entity is unconscious, than player can't control it
        /// </summary>
        public bool IsUnconscious { get; set; }
    }
}
