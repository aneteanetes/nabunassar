using MonoGame.Extended.Input;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Entities.Game;
using Nabunassar.Monogame.Interfaces;
using Nabunassar.Resources;
using Nabunassar.Widgets.Views.DescriptionTolltip;

namespace Nabunassar.Widgets.UserInterfaces.Combat
{
    internal abstract class BaseStatWidget : HorizontalStackPanel, IFeatured
    {
        private Creature _creature;
        private Label _label;
        protected NabunassarGame Game;
        private DescriptionTooltip _tooltip;
        private DescriptionPanel _tooltipPanel;
        private DescriptionPanel _descPanel;
        private bool _isAttached;
        private bool _inited;

        public BaseStatWidget(NabunassarGame game, Creature creature, string imageSource, int size = 25, Rectangle rect = default)
        {
            Game=game;
            _creature = creature;

            var texture = game.Content.LoadTexture(imageSource);
            var icon = new Image()
            {
                Renderable = new TextureRegion(texture, rect == default ? new Rectangle(0, 0, texture.Width, texture.Height) : rect),
                Color = GetIconColor(),
                Width = size,
                Height = size,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Myra.Graphics2D.Thickness(3, 0, 0, 0)
            };

            var font = game.Content.LoadFont(Fonts.Retron);
            _label = new Label()
            {
                Text = GetValue(_creature),
                Font = font.GetFont(size),
                TextColor = Globals.BaseColor
            };

            Widgets.Add(icon);
            Widgets.Add(_label);
        }

        public virtual void Init()
        {
            _inited = true;
            var desc = GetDescription(_creature);
            _descPanel = new DescriptionPanel(Game, desc);
        }

        private void UpdateTooltip()
        {
            var mousePos = MouseExtended.GetState().Position;

            var globalLocation = ToGlobal(this.ActualBounds.Location);

            var rect = new Rectangle(globalLocation, ActualBounds.Size);

            if (!rect.Contains(mousePos) && _isAttached)
            {
                Game.MyraDesktop.Widgets.Remove(_descPanel);
                _isAttached = false;
            }
            else if (rect.Contains(mousePos) && !_isAttached)
            {
                _isAttached = true;

                var pos = new Point(globalLocation.X + ActualBounds.Size.X, globalLocation.Y + ActualBounds.Size.Y);

                _descPanel.Left = pos.X;
                _descPanel.Top = pos.Y;

                Game.MyraDesktop.Widgets.Add(_descPanel);
            }
        }

        protected abstract Color GetIconColor();

        protected abstract string GetValue(Creature creature);

        public abstract Description GetDescription(Creature creature);

        public void Update(GameTime gameTime)
        {
            if (!_inited)
                Init();

            _label.Text = GetValue(_creature);
            UpdateTooltip();
        }
    }
}