using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Monogame.Content;
using Nabunassar.Widgets.Views.IconButtons;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class HorizontalIconPanel : HorizontalStackPanel
    {
        private Image _selectedImage;
        private Func<Image, Vector2> _titlePosition;

        public HorizontalIconPanel(NabunassarContentManager content, List<IconButton> iconButtons, Func<Image,Vector2> titlePosition=null)
        {
            Background = content.Load<Texture2D>("Assets/Images/Borders/controlpanel_m.png").NinePatch();
            _titlePosition = titlePosition;

            var i = 0;
            foreach (var iconButton in iconButtons)
            {
                var left = i == 0 ? 5 : 0;
                var right = i == iconButtons.Count - 1 ? 5 : 0;

                AddButton(this, iconButton.Icon, iconButton.OnClick, iconButton.Title, left, right);
                i++;
            }
        }

        private void AddButton(HorizontalStackPanel panel, TextureRegion image, Action click, string title, int paddingLeft = 0, int paddingRight = 0)
        {
            var img = new Image()
            {
                Renderable = image,
                Color = Globals.BaseColor,
                Width = 48,
                Height = 48,
                Left = paddingLeft,
                Padding = new Myra.Graphics2D.Thickness(5)
            };
            img.MouseEntered += (s, e) =>
            {
                _selectedImage = img;
                img.Color = Globals.CommonColor;

                var titlePos = _titlePosition(img);

                var left = img.ToGlobal(Point.Zero).X;

                NabunassarGame.Game.AddDesktopWidget(new TitleWidget(NabunassarGame.Game, title, titlePos, Color.White));
            };
            img.MouseLeft += (s, e) =>
            {
                img.Color = Globals.BaseColor;

                NabunassarGame.Game.RemoveDesktopWidgets<TitleWidget>(_selectedImage != img ? 1 : 0);
            };
            img.TouchDown += (s, e) => click();

            panel.Widgets.Add(img);

            if (paddingRight != 0)
                panel.Widgets.Add(new Panel() { Width = paddingRight });
        }
    }
}
