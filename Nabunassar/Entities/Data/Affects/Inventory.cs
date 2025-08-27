using Nabunassar.Entities.Data.Items;
using System.Collections;

namespace Nabunassar.Entities.Data.Affects
{
    internal class Inventory : IEnumerable<Item>
    {
        public void AddItem(Item item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);

                item.DateTimeRecived = DateTime.UtcNow;

                ItemAdded?.Invoke(this, item);
            }
        }

        internal void RemoveItem(Item item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);

                item.DateTimeRecived = null;

                ItemRemoved?.Invoke(this, item);
            }
        }

        public IEnumerator<Item> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        public List<Item> Items { get; set; } = new();

        public int Weight => Items.Sum(x => x.Weight);

        public event EventHandler<Item> ItemAdded;

        public event EventHandler<Item> ItemRemoved;
    }
}
