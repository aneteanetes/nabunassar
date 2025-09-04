using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.Views.DescriptionTolltip
{
    internal class DescriptionTooltip : ScreenWidget
    {
        private Description _description;
        private Point _position;
        private DescriptionPanel _panel;

        public DescriptionTooltip(NabunassarGame game, Description description, Point position) : base(game)
        {
            _description = description;
            _position = position;
        }

        protected override Widget CreateWidget()
        {
            _panel = new DescriptionPanel(Game, _description);
            _panel.Left = _position.X;
            _panel.Top = _position.Y;

            return _panel;
        }

        public override void Update(GameTime gameTime)
        {
            var panelBox = _panel.Left + (_panel.Width.HasValue ? _panel.Width.Value : _panel.ActualBounds.Width);
            if (panelBox > Game.Resolution.Width)
            {
                _panel.Left -= panelBox - Game.Resolution.Width + 5;
            }
            base.Update(gameTime);
        }
    }
}
