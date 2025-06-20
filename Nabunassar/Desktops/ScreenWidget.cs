using MonoGame.Extended;
using Myra.Graphics2D.UI;
using Nabunassar.Monogame;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Desktops
{
    internal abstract class ScreenWidget : ILoadable
    {
        protected NabunassarGame Game { get; private set; }
        private Widget _widget;

        public ScreenWidget(NabunassarGame game)
        {
            Game = game;
        }

        protected abstract Widget InitWidget();

        public GameObject GameObject { get; private set; }

        public Widget Load()
        {
            _widget = InitWidget();

            _widget.MouseEntered += _widget_MouseEntered;
            _widget.MouseLeft += _widget_MouseLeft;

            //var entity = Game.EntityFactory.CreateEntity();
            //var position = new Vector2(_widget.Left, _widget.Top);
            //GameObject = new GameObject(Game, position, Struct.ObjectType.Interface, entity, new RectangleF(Vector2.Zero, new SizeF(_widget.Width ?? 0f, _widget.Height ?? 0f)), "objects");
            //entity.Attach(GameObject);

            ////Game.EntityFactory.AddCollistion(GameObject);

            OnDispose += () =>
            {
                _widget.MouseEntered -= _widget_MouseEntered;
                _widget.MouseLeft -= _widget_MouseLeft;
                Game.IsCanMoveByMouse = true;
            };

            return _widget;
        }

        private void _widget_MouseLeft(object sender, EventArgs e)
        {
            Game.IsCanMoveByMouse = true;
        }

        private void _widget_MouseEntered(object sender, EventArgs e)
        {
            Game.IsCanMoveByMouse = false;
        }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public Action OnDispose { get; set; }

        public void Dispose()
        {
            UnloadContent();
            Game.Desktop.Root = null;
            OnDispose?.Invoke();
            GameObject = null;
        }
    }
}
