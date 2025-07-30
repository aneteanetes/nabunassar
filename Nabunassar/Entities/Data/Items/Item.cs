using Nabunassar.Entities.Game;
using Nabunassar.Entities.Struct.ImageRegions;

namespace Nabunassar.Entities.Data.Items
{
    internal class Item : GameObject, IClonable<Item>
    {
        public Item Clone(Item instance = null)
        {
            var item = new Item();
            Clone(item as GameObject);

            item.Icon = Icon;
            item.IconRegion = IconRegion;
            item.Cost = Cost;
            item.Weight = Weight;
            item.ItemType = ItemType;

            return item;
        }

        public string Icon { get; set; }

        public ImageRegion IconRegion { get; set; }

        public Money Cost { get; set; }

        public ItemType ItemType { get; set; }

        public DateTime DateTimeRecived { get; set; }

        public int Weight { get; set; }

        public override string GetObjectName()
        {
            var game = NabunassarGame.Game;

            var objectNames = game.Strings["ItemNames"];

            string token = null;

            if (Name != null)
                token = Name;

            if (token == null && ObjectId > 0)
                token = ObjectId.ToString();

            return objectNames[token];
        }
    }
}