using MonoGame.Extended;
using Myra.Graphics2D.UI;
using Nabunassar.Components;
using Nabunassar.Monogame;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Desktops
{
    internal abstract class ScreenWidget : IGameComponent, IDrawable, IUpdateable, IDisposable
    {
        protected NabunassarGame Game { get; private set; }

        protected NabunassarContentManager Content => Game.Content;

        private Widget _widget;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public int DrawOrder { get; set; }

        public bool Visible { get; set; } = true;

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

        public MapObject GameObject { get; private set; }

        protected virtual bool IsMouseActiveOnRootWidget => true;

        public Widget Load()
        {
            _widget = InitWidget();

            //if (IsMouseActiveOnRootWidget)
            //{
            //    _widget.MouseEntered += _widget_MouseEntered;
            //    _widget.MouseLeft += _widget_MouseLeft;
            //}

            OnDispose += () =>
            {
                ////_widget.MouseEntered -= _widget_MouseEntered;
                ////_widget.MouseLeft -= _widget_MouseLeft;
                //Game.IsMouseActive = true;
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

        public virtual void Close()
        {
            Dispose();
            Game.SwitchDesktop();
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }
    }
}
