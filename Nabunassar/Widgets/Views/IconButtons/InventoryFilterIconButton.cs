using Myra.Graphics2D.TextureAtlases;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components;

namespace Nabunassar.Widgets.Views.IconButtons
{
    internal class InventoryFilterIconButton : IconButton
    {
        private ItemsPanel _itemPanel;
        private Func<Item, bool> _filter;


        public InventoryFilterIconButton(string title, TextureRegion icon, ItemsPanel itemPanel, Func<Item, bool> filter=null) : base(title, icon)
        {
            _filter = filter;
            _itemPanel = itemPanel;
        }

        public override void OnClick()
        {
            if (_filter == null)
                _itemPanel.ResetFilter();
            _itemPanel.Filter(_filter);
        }
    }
}