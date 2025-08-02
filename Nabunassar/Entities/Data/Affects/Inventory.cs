using Microsoft.Xna.Framework.Input;
using Myra.Events;
using Nabunassar.Entities.Data.Items;

namespace Nabunassar.Entities.Data.Affects
{
    internal class Inventory
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

        public List<Item> Items { get; set; } = new();

        public int Weight => Items.Sum(x => x.Weight);

        public event EventHandler<Item> ItemAdded;

        public event EventHandler<Item> ItemRemoved;
    }
}
