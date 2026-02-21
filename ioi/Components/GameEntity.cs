using ioi.Scripting;
using MonoGame.Extended.ECS;
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

        public double GetHp()
        {
            var func = _script.Executor.LoadString("local t = ...; return t.hp");

            DynValue result = _script.Call(func, Data);

            return result.Number;
        }

        public DynValue this[string key]
        {
            get => Data.GetSmart(key);
            set => Data.Set(key, value);
        }
    }

    public static class MoonSharpHelper
    {
        public static DynValue GetSmart(this Table table, string key)
        {
            // 1. Сначала ищем в самом объекте (локальный override)
            var val = table.Get(key);
            if (!val.IsNil()) return val;

            // 2. Получаем список путей к шаблонам из _sources
            var sources = table.Get("_sources").Table;
            if (sources == null) return DynValue.Nil;

            foreach (var sourcePath in sources.Values)
            {
                if (sourcePath.Type != DataType.String) continue;

                // 3. Резолвим путь (например, "Templates.Classes.Warrior")
                Table currentTable = ResolvePath(table.OwnerScript, sourcePath.String);
                if (currentTable == null) continue;

                // 4. Ищем ключ в найденной таблице
                var sVal = currentTable.Get(key);
                if (!sVal.IsNil()) return sVal;
            }

            return DynValue.Nil;
        }

        // Хелпер для быстрого прохода по точкам
        private static Table ResolvePath(Script script, string path)
        {
            Table current = script.Globals;
            // Используем Span или просто проход по строке, чтобы не плодить массивы строк
            string[] parts = path.Split('.');

            foreach (var part in parts)
            {
                var next = current.Get(part);
                if (next.Type != DataType.Table) return null;
                current = next.Table;
            }
            return current;
        }
    }
}
