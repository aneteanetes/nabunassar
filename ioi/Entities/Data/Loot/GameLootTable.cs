using Geranium.Reflection;
using ioi.Components;
using MoonSharp.Interpreter;

namespace ioi.Entities.Data.Loot
{
    internal class GameLootTable
    {
        private GameEntity _lootTable;
        private string _name;

        public GameLootTable(DynValue lootTableVal, string name)
        {
            if (lootTableVal.IsNotNil() && lootTableVal.UserData != null && lootTableVal.UserData.Object is GameEntity lootTable)
            {
                _lootTable = lootTable;
            }
            _name = name;
        }

        public GameLootTable(GameEntity entity, string name)
        {
            if (entity != null)
            {
                _lootTable = entity;
            }
            _name = name;
        }

        public List<GameEntity> Generate(GameEntity player)
        {
            if (_lootTable == null)
                return [];

            var table = _lootTable.Func("generate", _name,player);
            if (table.IsNil())
                return [];

            if (table.Type != MoonSharp.Interpreter.DataType.Table)
                return [];

            List<GameEntity> items = new();

            foreach (var value in table.Table.Values)
            {
                if (value.UserData != default && value.UserData.Object !=null && value.UserData.Object is GameEntity itemEntity)
                {
                    items.Add(value.UserData.Object.As<GameEntity>());
                }
            }

            return items;
        }
    }
}
