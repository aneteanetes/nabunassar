namespace Nabunassar.Entities.Data.Loot
{
    internal class LootTableRow
    {
        public Guid ItemId { get; set; }

        public LootChance Type { get; set; }

        public float Value { get; set; }
    }
}
