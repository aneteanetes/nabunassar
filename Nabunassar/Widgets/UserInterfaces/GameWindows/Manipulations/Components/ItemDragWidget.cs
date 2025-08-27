using MonoGame.Extended.Input;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.Views;

namespace Nabunassar.Widgets.UserInterfaces.GameWindows.Manipulations.Components
{
    internal class ItemDragWidget : ScreenWidget
    {
        private ItemView _itemView;
        private Image _image;

        public ItemDragWidget(NabunassarGame game, ItemView itemView) : base(game)
        {
            _itemView = itemView;
        }

        protected override Widget CreateWidget()
        {
            var size = 56;
            _image = new Image
            {

                Renderable = _itemView.Icon,
                Width = size,
                Height = size,
                BorderThickness = new Myra.Graphics2D.Thickness(1),
                Border = new SolidBrush(Color.White),
                IsModal = true,
                DragDirection = DragDirection.Both
            };

            Position = MouseExtended.GetState().Position.ToVector2();

            return _image;
        }

        public override void OnAfterAddedWidget(Widget widget)
        {
            Game.DisableMouseSystems();
        }

        public override void Dispose()
        {
            Game.EnableSystems();
            base.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = MouseExtended.GetState();
            var pos = mouse.Position;
            _image.Left = pos.X;
            _image.Top = pos.Y;

            if (mouse.WasButtonReleased(MouseButton.Left))
            {
                Game.RemoveDesktopWidget(this);
            }
        }
    }
}
