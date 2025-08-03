using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;

namespace Nabunassar.Widgets.Views.IconButtons
{
    internal class InventoryFilterIconByClassButton : InventoryFilterIconButton
    {
        public Archetype Archetype { get; set; }

        public InventoryFilterIconByClassButton(Archetype archetype, string title, TextureRegion icon, ItemPanel itemPanel, Func<Item, bool> filter=null) : base(title, icon,itemPanel,filter)
        {
            Archetype = archetype;
        }
    }
}