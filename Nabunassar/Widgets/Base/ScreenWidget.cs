using MonoGame.Extended;
using Myra.Graphics2D.UI;
using Nabunassar.Components;
using Nabunassar.Monogame;
using Nabunassar.Monogame.Content;
using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Widgets.Base
{
    internal abstract class ScreenWidget : IGameComponent, IDrawable, IUpdateable, IDisposable
    {
        public NabunassarGame Game { get; private set; }

        protected NabunassarContentManager Content => Game.Content;

        protected Widget UIWidget;

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
            EnabledChanged?.Invoke(null, null);
            UpdateOrderChanged?.Invoke(null, null);
            DrawOrderChanged?.Invoke(null, null);
            VisibleChanged?.Invoke(null, null);
        }

        public Widget GetWidgetReference() => UIWidget;

        protected virtual void LoadContent() { }

        protected virtual void UnloadContent() { }

        public virtual void Initialize()
        {
            LoadContent();
        }

        protected abstract Widget InitWidget();

        public MapObject MapObject { get; private set; }

        protected virtual bool IsMouseActiveOnRootWidget => true;

        public Widget Load()
        {
            UIWidget = InitWidget();

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

            return UIWidget;
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
            Game.Desktop.Widgets.Remove(UIWidget);
            OnDispose?.Invoke();
            MapObject = null;
        }

        public virtual void Close()
        {
            Dispose();
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }
    }
}
