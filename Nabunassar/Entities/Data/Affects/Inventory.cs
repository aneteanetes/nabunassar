using Microsoft.Xna.Framework.Input;
using Myra.Events;
using Nabunassar.Entities.Data.Items;

namespace Nabunassar.Entities.Data.Affects
{
    internal class Inventory
    {
        public bool AddItem(Item item)
        {
            if(Items.Contains(item)) 
                return false;

            Items.Add(item);

            item.DateTimeRecived = DateTime.UtcNow;

            ItemAdded.Invoke(this, item);

            return true;
        }

        public List<Item> Items { get; set; } = new();

        public int Weight => Items.Sum(x => x.Weight);

        public event EventHandler<Item> ItemAdded;
    }
}
