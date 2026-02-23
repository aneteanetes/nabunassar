using FontStashSharp;
using ioi.Widgets.Base;
using Myra.Graphics2D.UI;

namespace ioi.Widgets.UserInterfaces.Roguelike
{
    internal class ControlsWidget : ScreenWidget
    {
        private DynamicSpriteFont consolas;
        private Dictionary<int, ControlbuttonWidget> controls = new();

        public ControlsWidget(GameHost game) : base(game)
        {
        }

        public override void LoadContent()
        {
            consolas = Content.LoadFont(Fonts.Consolas).GetFont(24);
        }

        protected override Widget CreateWidget()
        {
            var grid = new HorizontalStackPanel
            {
                Height = 94,
                Margin = new Myra.Graphics2D.Thickness(1)
            };

            CreateColumn(grid, 287, 1);
            CreateColumn(grid, 294, 2);
            CreateColumn(grid, 279, 3);
            CreateColumn(grid, 278, 4);
            CreateColumn(grid, 270, 5);

            return grid;
        }

        private void CreateColumn(HorizontalStackPanel grid, int width, int idx)
        {
            var vpanel = new VerticalStackPanel
            {
                Width = width,
                Margin=new Myra.Graphics2D.Thickness(0,5,0,0)
            };

            var btn = CreateButton(width, idx);
            var btn1 = CreateButton(width, idx + 5);

            vpanel.Widgets.Add(btn);
            vpanel.Widgets.Add(new Panel() { Height = 10 });
            vpanel.Widgets.Add(btn1);

            grid.Widgets.Add(vpanel);
            grid.Widgets.Add(new Panel() { Width = 9 });
        }

        private ControlbuttonWidget CreateButton(int width, int idx)
        {
            var control = new ControlbuttonWidget(width, consolas);
            controls[idx] = control;
            return control;
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            widget.Left = 12;
            widget.Top = 974;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">from 1 (first row 1st column) to 10 (second row 5th column)</param>
        /// <param name="text"></param>
        /// <param name="onClick"></param>
        public void BindButton(int index, string text, Action onClick=null)
        {
            controls[index].Text = text;
            controls[index].Click=onClick;
        }
    }
}
