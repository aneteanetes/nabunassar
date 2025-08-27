using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Game;
using Nabunassar.Entities.Game.Enums;
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
            item.ItemSubtype = ItemSubtype;
            item.Archetype = Archetype;
            item.IsCombat = IsCombat;
            item.AbilityName = AbilityName;

            return item;
        }

        public string Icon { get; set; }

        public ImageRegion IconRegion { get; set; }

        public Money Cost { get; set; }

        public ItemType ItemType { get; set; }

        public ItemSubtype ItemSubtype { get; set; }

        public Archetype Archetype { get; set; }

        public DateTime? DateTimeRecived { get; set; }

        public bool IsCombat { get; set; }

        public string AbilityName { get; set; }

        public int Weight { get; set; }

        public bool IsStack { get; set; } = false;

        public int Count { get; set; }

        public int GetCount()
        {
            if (IsStack)
                return Count;

            return 1;
        }

        public bool TryGetAbility(out AbilityModel ability)
        {
            if (AbilityName.IsNotEmpty())
            {
                var game = NabunassarGame.Game;

                var model = game.DataBase.GetAbility(AbilityName);
                ability = model.Load(game);

                return true;
            }

            ability = null;
            return false;
        }

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