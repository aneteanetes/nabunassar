using Microsoft.Xna.Framework.Input;
using MoonSharp.Interpreter;

namespace ioi.Scripting
{
    public static class MoonSharpHelper
    {
        public static DynValue GetSmart(this Table table, string key)
        {
            // 1. Сначала ищем в самом объекте (локальный override)
            var baseObjVal = ResolvePath(table, key);
            //var val = table.Get(key);
            if (!baseObjVal.IsNil()) return baseObjVal;

            // 2. Получаем список путей к шаблонам из _components
            var sources = table.Get("_components").Table;
            if (sources == null) return DynValue.Nil;

            foreach (var sourcePath in sources.Values.Reverse())
            {
                if (sourcePath.Type != DataType.String) continue;

                // 3. Резолвим путь (например, "Templates.Classes.Warrior")
                var currentTable = ResolvePath(table.OwnerScript.Globals, sourcePath.String);
                if (currentTable == null || currentTable.Type != DataType.Table) continue;

                // if key splitted
                var resolvedValue = ResolvePath(currentTable.Table, key);
                //if (resolvedValue != null && resolvedValue.Type == DataType.Table)
                //{
                //    // 4. Ищем ключ в найденной таблице
                //    var sVal = resolvedValue.Table.Get(key);
                //    if (!sVal.IsNil()) return sVal;
                //}
                //else
                if (resolvedValue.IsNotNil())
                {
                    return resolvedValue;
                }
            }

            return DynValue.Nil;
        }

        // Хелпер для быстрого прохода по точкам
        public static DynValue ResolvePath(this Table current, string path)
        {
            DynValue value = DynValue.Nil;
            // Используем Span или просто проход по строке, чтобы не плодить массивы строк
            string[] parts = path.Split('.');

            foreach (var part in parts)
            {
                var next = current.Get(part);
                if (next.Type != DataType.Table) return next;
                current = next.Table;
                value = next;
            }
            return value;
        }

        /// <summary>
        /// инициализация
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DynValue Init(this Table table)
        {
            Stack<DynValue> initList = new();

            //base init in object
            GetInitMethod(table, initList);

            // get components
            var sources = table.Get("_components").Table;
            if (sources != null)
            {
                foreach (var component in sources.Values.Reverse())
                {
                    if (component.Type != DataType.String) continue;

                    // 3. Резолвим путь (например, "Templates.Classes.Warrior")
                    var currentTable = ResolvePath(table.OwnerScript.Globals, component.String);
                    if (currentTable == null || currentTable.Type != DataType.Table) continue;


                    GetInitMethod(currentTable.Table, initList);
                }
            }

            foreach (var init in initList)
            {
                table.OwnerScript.Call(init, table);
            }

            return DynValue.Nil;
        }

        private static void GetInitMethod(Table table, Stack<DynValue> initList)
        {
            var @base = table.Get("init");
            if (!@base.IsNil() && @base.Function != null)
                initList.Push(@base);
        }
    }
}
