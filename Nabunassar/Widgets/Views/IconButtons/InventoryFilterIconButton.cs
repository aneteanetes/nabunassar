using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;

namespace Nabunassar.Widgets.Views.IconButtons
{
    internal class InventoryFilterIconButton : IconButton
    {
        private ItemPanel _itemPanel;
        private Func<Item, bool> _filter;


        public InventoryFilterIconButton(string title, TextureRegion icon, ItemPanel itemPanel, Func<Item, bool> filter=null) : base(title, icon)
        {
            _filter = filter;
            _itemPanel = itemPanel;
        }

        public override void OnClick()
        {
            if (_filter == null)
                _itemPanel.ResetFilter();
            else
                _itemPanel.Filter(_filter);
        }
    }
}