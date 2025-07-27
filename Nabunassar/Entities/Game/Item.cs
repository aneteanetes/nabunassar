using Nabunassar.Entities.Struct.ImageRegions;

namespace Nabunassar.Entities.Game
{
    internal class Item : GameObject, IClonable<Item>
    {
        public Item Clone(Item instance = null)
        {
            var item = new Item();
            this.Clone(item as GameObject);

            item.Icon = Icon;
            item.IconRegion = IconRegion;

            return item;
        }

        public string Icon { get; set; }

        public ImageRegion IconRegion { get; set; }

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