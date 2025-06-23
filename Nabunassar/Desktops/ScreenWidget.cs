using MonoGame.Extended;
using Myra.Graphics2D.UI;
using Nabunassar.Monogame;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Desktops
{
    internal abstract class ScreenWidget : IGameComponent, IUpdateable, IDisposable
    {
        protected NabunassarGame Game { get; private set; }

        protected NabunassarContentManager Content => Game.Content;

        private Widget _widget;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public bool Enabled { get; set; } = true;

        public int UpdateOrder { get; set; } = 0;

        public ScreenWidget(NabunassarGame game)
        {
            Game = game;
        }

        protected virtual void LoadContent() { }

        protected virtual void UnloadContent() { }

        public virtual void Initialize()
        {
            LoadContent();
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
                Game.IsMouseActive = true;
            };

            return _widget;
        }

        private void _widget_MouseLeft(object sender, EventArgs e)
        {
            Game.IsMouseActive = true;
        }

        private void _widget_MouseEntered(object sender, EventArgs e)
        {
            Game.IsMouseActive = false;
        }

        public Action OnDispose { get; set; }

        public virtual void Dispose()
        {
            UnloadContent();
            Game.Components.Remove(this);
            Game.Desktop.Root = null;
            OnDispose?.Invoke();
            GameObject = null;
        }

        public virtual void Update(GameTime gameTime) { }
    }
}
