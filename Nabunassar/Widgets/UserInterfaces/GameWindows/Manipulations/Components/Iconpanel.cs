using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class Iconpanel : Panel
    {
        private Grid _description;

        public Grid Content => _description;

        public Iconpanel(IImage iconImage)
        {
            var size = 52;

            BorderThickness = new Thickness(0, 0, 0, 1);
            Border = new SolidBrush(Color.White);
            OverBackground = ScreenWidgetWindow.WindowBackground.NinePatch();
            HorizontalAlignment = HorizontalAlignment.Stretch;

            var grid = new Grid()
            {
                Padding = new Thickness(6)
            };
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2f));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 8f));

            var icon = new Image()
            {
                Renderable = iconImage,
                Width = size,
                Height = size,
                VerticalAlignment = VerticalAlignment.Top,
                Border = new SolidBrush(Color.White),
                BorderThickness=new Thickness(1),
                Padding=new Thickness(5)
            };
            grid.Widgets.Add(icon);
            Grid.SetColumn(icon, 0);

            _description = new Grid();

            grid.Widgets.Add(_description);
            Grid.SetColumn(_description, 1);

            Widgets.Add(grid);
        }

        public void Add(Widget widget)
        {
            _description.Widgets.Add(widget);
        }

        public void Remove(Widget widget)
        {
            _description.Widgets.Remove(widget);
        }
    }
}
