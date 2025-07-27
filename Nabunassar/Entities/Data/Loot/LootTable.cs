using Nabunassar.Entities.Game;

namespace Nabunassar.Entities.Data.Loot
{
    internal class LootTable
    {
        public int TableId { get; set; }

        public List<LootTableRow> Rows { get; set; } = new();

        public List<Item> Generate(NabunassarGame game)
        {
            var db = game.DataBase;

            var items=new List<Item>();

            void AddItem(LootTableRow row)
            {
                items.Add(db.GetItem(row.ItemId));
            }

            foreach (var row in Rows)
            {
                if (row.Type == LootChance.Guaranteed)
                {
                    AddItem(row);
                    continue;
                }

                if (row.Type == LootChance.Percent)
                {
                    var isRolled = game.Randoms.Chance(row.Value);
                    if (isRolled)
                        AddItem(row);
                }
            }

            return items;
        }
    }
}