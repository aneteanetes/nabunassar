using Myra.Graphics2D.TextureAtlases;
using ioi.Entities.Data.Items;
using ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Components;

namespace ioi.Widgets.Views.IconButtons
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