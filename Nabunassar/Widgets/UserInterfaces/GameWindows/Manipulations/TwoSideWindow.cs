using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations
{
    internal abstract class TwoSideWindow : ScreenWidgetWindow
    {
        public TwoSideWindow(NabunassarGame game) : base(game)
        {
        }

        protected override Window CreateWindow()
        {
            var window = new Window();

            var grid = new Grid();
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 4.9f));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 0.2f));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 4.9f));

            var left = LeftSide();

            var split = new Button()
            {
                Enabled = false,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 5
            };

            var right = RightSide();

            Grid.SetColumn(left, 0);
            Grid.SetColumn(split, 1);
            Grid.SetColumn(right, 2);

            grid.Widgets.Add(left);
            grid.Widgets.Add(split);
            grid.Widgets.Add(right);

            grid.Width = left.Width + 25 + right.Width;

            window.Content = grid;
            window.CloseKey = Microsoft.Xna.Framework.Input.Keys.Escape;

            return window;
        }

        protected abstract Widget LeftSide();

        protected abstract Widget RightSide();
    }
}
