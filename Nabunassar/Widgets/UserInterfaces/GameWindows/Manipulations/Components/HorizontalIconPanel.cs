using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using Nabunassar.Monogame.Content;
using Nabunassar.Widgets.Views.IconButtons;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class HorizontalIconPanel : HorizontalStackPanel
    {
        private Image _selectedImage;
        private Func<Image, Vector2> _titlePosition;
        private List<Image> _buttons = new();
        private Dictionary<IconButton, Image> _iconButtonImages = new();

        public HorizontalIconPanel(NabunassarContentManager content, List<IconButton> iconButtons, Func<Image,Vector2> titlePosition=null, HorizontalAlignment btnAligment= HorizontalAlignment.Left)
        {
            Background = content.Load<Texture2D>("Assets/Images/Borders/controlpanel_m.png").NinePatch();
            _titlePosition = titlePosition;

            var panel = new HorizontalStackPanel();
            panel.HorizontalAlignment = btnAligment;

            this.Widgets.Add(panel);

            var i = 0;
            foreach (var iconButton in iconButtons)
            {
                var left = i == 0 ? 5 : 0;
                var right = i == iconButtons.Count - 1 ? 5 : 0;

                var btn = AddButton(panel, iconButton, iconButton.OnClick, iconButton.Title, left, right);
                if (iconButton.IsReactOnClick)
                    _buttons.Add(btn);
                i++;
            }
        }

        private Image AddButton(HorizontalStackPanel panel, IconButton iconButton, Action click, string title, int paddingLeft = 0, int paddingRight = 0)
        {
            var img = new Image()
            {
                Renderable = iconButton.Icon,
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
                if (iconButton.IsReactOnClick)
                {
                    if (img == _selected)
                        img.Color = Globals.CommonColorLight;
                    else
                        img.Color = Globals.BaseColor;
                }
                else
                    img.Color = Globals.BaseColor;

                if (_close == iconButton)
                {
                    img.Color = Globals.BaseColor;
                    _close = null;
                    _selected = null;
                }

                NabunassarGame.Game.RemoveDesktopWidgets<TitleWidget>(_selectedImage != img ? 1 : 0);
            };
            img.TouchDown += (s, e) =>
            {
                if (iconButton.IsReactOnClick)
                {
                    ResetButtons();
                    SelectButton(img);
                }
                click();
            };

            panel.Widgets.Add(img);

            if (paddingRight != 0)
                panel.Widgets.Add(new Panel() { Width = paddingRight });

            _iconButtonImages[iconButton] = img;

            return img;
        }

        private Image _selected;

        private void SelectButton(Image btn)
        {
            btn.Color = Globals.CommonColorLight;
            _selected = btn;
        }

        public void Select(IconButton iconButton)
        {
            var img = _iconButtonImages[iconButton];
            SelectButton(img);
        }

        IconButton _close;
        public void Close(IconButton iconButton)
        {
            _close = iconButton;
            _iconButtonImages[iconButton].Color = Globals.BaseColor;
        }

        public void ResetButtons()
        {
            _buttons.ForEach(x =>
            {
                x.Color = Globals.BaseColor;
            });
        }
    }
}
