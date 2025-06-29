using Myra.Graphics2D.UI;

namespace Nabunassar.Widgets.Base
{
    internal class EmptyScreenWidget : ScreenWidget
    {
        public EmptyScreenWidget(NabunassarGame game) : base(game)
        {
        }

        protected override Widget InitWidget()
        {
            return new Panel() { Width = 1, Height = 1 };
        }
    }
}
