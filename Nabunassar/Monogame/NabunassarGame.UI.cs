using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Styles;
using Nabunassar.Resources;

namespace Nabunassar
{
    internal partial class NabunassarGame
    {
        public void ApplyMyraCustomStyle()
        {
			Stylesheet.Current.TabControlStyle.TabItemStyle.Background = new SolidBrush(Color.Transparent);
            Stylesheet.Current.TabControlStyle.TabItemStyle.PressedBackground = new SolidBrush(Globals.CommonColor);
            Stylesheet.Current.TabControlStyle.ContentStyle.Background = new SolidBrush(Color.Transparent);
			Stylesheet.Current.TabControlStyle.TabItemStyle.LabelStyle.Font = Content.LoadFont(Fonts.Retron).GetFont(18);

            // Stylesheet.Current.LabelStyle.TextColor = Color.Green;
        }
    }
}


/*<TabControlStyles>
		<TabControlStyle ButtonSpacing="0">
			<TabItemStyle OverBackground="button-over" Padding="5, 0" PressedBackground="list-selection">
				<LabelStyle Font="default-font" TextColor="white" DisabledTextColor="gray" />
			</TabItemStyle>
			<ContentStyle Background="window" Padding="5" />
			<CloseButtonStyle OverBackground="button-over" Padding="5, 0" PressedBackground="button-red">
				<ImageStyle Image="icon-close" />
			</CloseButtonStyle>
		</TabControlStyle>
	</TabControlStyles>*/