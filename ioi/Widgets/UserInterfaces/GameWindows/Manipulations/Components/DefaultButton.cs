using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using ioi.Widgets.Base;

namespace ioi.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class DefaultButton : Button
    {
        public DefaultButton(string text)
        {
            var font = GameHost.Game.Content.LoadFont(Fonts.Retron);

            Height = 25;
            Background = ScreenWidgetWindow.WindowBackground.NinePatch();
            HorizontalAlignment = HorizontalAlignment.Stretch;

            var btnText = new Label()
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = font.GetFont(24),
            };

            Content = btnText;
            PressedBackground = new SolidBrush(Color.Black);
        }
    }
}
