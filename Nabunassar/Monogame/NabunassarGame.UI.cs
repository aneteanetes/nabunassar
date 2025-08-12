using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Styles;
using Nabunassar.Resources;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public void ApplyMyraCustomStyle()
        {
            var tabControlStyle = Stylesheet.Current.TabControlStyle;


            tabControlStyle.TabItemStyle.Background = new SolidBrush(Color.Transparent);
            tabControlStyle.TabItemStyle.PressedBackground = new SolidBrush(Globals.CommonColor);
            tabControlStyle.ContentStyle.Background = new SolidBrush(Color.Transparent);
			tabControlStyle.TabItemStyle.LabelStyle.Font = Content.LoadFont(Fonts.Retron).GetFont(18);

            var retronFont = Content.LoadFont(Fonts.Retron);
            var tooltipBackground = Content.Load<Texture2D>("Assets/Images/Borders/panel-transparent-center-022_colored_white.png");

            var tooltipStyle = Stylesheet.Current.TooltipStyle;

            tooltipStyle.Background = tooltipBackground.NinePatch();
            tooltipStyle.Font = retronFont.GetFont(18);
            tooltipStyle.Padding = new Myra.Graphics2D.Thickness(9);
        }
    }
}