using FontStashSharp;
using Myra.Graphics2D.UI;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class WeightBar : Panel
    {
        private NabunassarGame _game;
        private HorizontalProgressBar _weightBar;
        private Label _weightLabel;

        public WeightBar(NabunassarGame game, FontSystem font)
        {
            _game = game;
            _weightBar = new HorizontalProgressBar();
            _weightBar.Filler = new SolidBrush(Globals.BaseColor);

            _weightLabel = new Label()
            {
                Font = font.GetFont(16),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            RecalculateWeightView();

            Widgets.Add(_weightBar);
            Widgets.Add(_weightLabel);
        }

        public void RecalculateWeightView()
        {
            _weightBar.Maximum = _game.GameState.Party.Weight;
            _weightBar.Value = _game.GameState.Party.Inventory.Weight;
            _weightLabel.Text = $"{_weightBar.Value}/{_weightBar.Maximum}";
        }
    }
}
