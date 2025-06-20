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

            //var entity = Game.EntityFactory.CreateEntity();
            //var position = new Vector2(_widget.Left, _widget.Top);
            //GameObject = new GameObject(Game, position, Struct.ObjectType.Interface, entity, new RectangleF(Vector2.Zero, new SizeF(_widget.Width ?? 0f, _widget.Height ?? 0f)), "objects");
            //entity.Attach(GameObject);

            ////Game.EntityFactory.AddCollistion(GameObject);

            //OnDispose += () =>
            //{
            //    Game.World.DestroyEntity(entity);
            //};

            return _widget;
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
