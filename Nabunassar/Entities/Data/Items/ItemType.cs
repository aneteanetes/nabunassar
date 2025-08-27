using Nabunassar.Entities.Struct.ImageRegions;

namespace Nabunassar.Entities.Data.Items
{
    internal enum ItemType
    {
        Weapon,
        Armor,
        Accessories,
        Consumables,
        Resource,
        Quest
    }

    internal static class ItemTypeAttributes
    {
        public static (ImageRegion, string) GetInfo(this ItemType itemType, NabunassarGame game)
        {
            string Text(ItemType type) => game.Strings["UI"][type.ToString()];

            var texture = "Assets/Tilesets/transparent_packed.png";

            switch (itemType)
            {
                case ItemType.Weapon: return (new ImageRegion(608, 128, 16, 16, texture), Text(itemType));
                case ItemType.Armor: return (new ImageRegion(560, 16, 16, 16, texture), Text(itemType));
                case ItemType.Accessories: return (new ImageRegion(720, 128, 16, 16, texture), Text(itemType));
                case ItemType.Consumables: return (new ImageRegion(544, 256, 16, 16, texture), Text(itemType));
                //case ItemType.Potions: return (new ImageRegion(624, 176, 16, 16, texture), Text(itemType));
                case ItemType.Resource: return (new ImageRegion(528, 320, 16, 16, texture), Text(itemType));
                case ItemType.Quest: return (new ImageRegion(544, 160, 16, 16, texture), Text(itemType));
                default: return (default, null);
            }
        }
    }
}
