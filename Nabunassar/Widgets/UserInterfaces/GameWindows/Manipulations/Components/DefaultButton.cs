using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class DefaultButton : Button
    {
        public DefaultButton(string text)
        {
            var font = NabunassarGame.Game.Content.LoadFont(Fonts.Retron);

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
