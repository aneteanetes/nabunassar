using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.Debug
{
    internal class LogWindow : ScreenWidget
    {
        public LogWindow(NabunassarGame game) : base(game)
        {
        }

        protected override Widget InitWidget()
        {
            var window = new Window();
            window.Title = "debug log";
            window.Width = 350;
            window.Height = 400;

            var panel = new Panel();

            var scrollbox = new ScrollViewer();
            scrollbox.ShowVerticalScrollBar = true;
            scrollbox.ShowHorizontalScrollBar = false;
            var text = new Label();
            text.Text = "";

            Game.GameState.OnLog += t =>
            {
                text.Text = t + Environment.NewLine + text.Text;
                scrollbox.ResetScroll();
            };

            scrollbox.Content = text;
            panel.Widgets.Add(scrollbox);

            window.Content = panel;

            return window;
        }
    }
}
