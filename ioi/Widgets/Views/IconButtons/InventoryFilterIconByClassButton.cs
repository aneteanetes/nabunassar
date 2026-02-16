using Myra.Graphics2D.TextureAtlases;
using ioi.Entities.Data.Items;
using ioi.Entities.Game.Enums;
using ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Components;

namespace ioi.Widgets.Views.IconButtons
{
    internal class InventoryFilterIconByClassButton : InventoryFilterIconButton
    {
        public Archetype Archetype { get; set; }

        public InventoryFilterIconByClassButton(Archetype archetype, string title, TextureRegion icon, ItemsPanel itemPanel, Func<Item, bool> filter=null) : base(title, icon,itemPanel,filter)
        {
            Archetype = archetype;
        }
    }
}