using Myra.Graphics2D.UI;

namespace ioi.Widgets.Base
{
    internal class EmptyScreenWidget : ScreenWidget
    {
        public EmptyScreenWidget(GameHost game) : base(game)
        {
        }

        protected override Widget CreateWidget()
        {
            return new Panel() { Width = 1, Height = 1 };
        }
    }
}
