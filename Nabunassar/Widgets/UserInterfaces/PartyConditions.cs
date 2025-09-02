using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.Views.StatusEffects;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class PartyConditions : ScreenWidget
    {
        private HorizontalStackPanel _panel;
        private List<StatusEffectWidget> _effectWidgets = new();

        public override bool IsRemovable => false;

        public PartyConditions(NabunassarGame game) : base(game)
        {
        }

        protected override Widget CreateWidget()
        {
            _panel = new HorizontalStackPanel();
            _panel.Margin = new Myra.Graphics2D.Thickness(5, 0);

            _panel.VerticalAlignment = VerticalAlignment.Top;
            _panel.HorizontalAlignment = HorizontalAlignment.Right;

            return _panel;
        }

        public override void Update(GameTime gameTime)
        {
            var gameTimeWidget = Game.GetDesktopWidget<GameDateTime>().UIWidget;
            _panel.Left = (gameTimeWidget.ActualBounds.Size.X+ gameTimeWidget.MBPWidth) * -1;

            if (Game.GameState.PartyEffects.IsChanged)
            {
                _effectWidgets.Clear();
                _panel.Widgets.Clear();

                foreach (var partyEffect in Game.GameState.PartyEffects)
                {
                    var effectWidget = new StatusEffectWidget(Game, partyEffect);
                    _panel.Widgets.Add(effectWidget);
                    _effectWidgets.Add(effectWidget);
                }
            }

            foreach (var effectWidget in _effectWidgets)
            {
                effectWidget.Update(gameTime);
            }
        }
    }
}
