﻿using Geranium.Reflection;
using Myra.Graphics2D.UI;
using Nabunassar.Components;
using Nabunassar.Monogame.Content;
using System.Diagnostics.Tracing;

namespace Nabunassar.Widgets.Base
{
    internal abstract class ScreenWidget : IGameComponent, IDrawable, IUpdateable, IDisposable
    {
        public NabunassarGame Game { get; private set; }

        internal static bool NOLOOSEBLOCK = false;

        protected NabunassarContentManager Content => Game.Content;

        protected Widget UIWidget;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public virtual bool IsModal => false;

        public virtual bool IsRemovable => true;

        public int DrawOrder { get; set; }

        public bool Visible { get; set; } = true;

        public bool Enabled { get; set; } = true;

        public int UpdateOrder { get; set; } = 0;

        public Vector2 Position { get; set; }

        public ScreenWidget(NabunassarGame game)
        {
            Game = game;
            EnabledChanged?.Invoke(null, null);
            UpdateOrderChanged?.Invoke(null, null);
            DrawOrderChanged?.Invoke(null, null);
            VisibleChanged?.Invoke(null, null);
        }

        public Widget GetWidgetReference() => UIWidget;

        public virtual void LoadContent() { }

        protected virtual void UnloadContent() { }

        public virtual void Initialize() { }

        protected abstract Widget CreateWidget();

        public virtual void OnAfterAddedWidget(Widget widget) { }

        public MapObject MapObject { get; protected set; }

        protected virtual bool IsMouseMovementAvailableWithThisActivedWidget => false;

        private HashSet<Widget> bindedWidgets = new();

        public Widget Load()
        {
            LoadContent();
            UIWidget = CreateWidget();
            UIWidget.IsModal = IsModal;
            UIWidget.Tag = this.GetType().Name;

            if (!IsMouseMovementAvailableWithThisActivedWidget && !bindedWidgets.Contains(UIWidget))
            {
                BindWidgetBlockMouse(UIWidget);
            }

            return UIWidget;
        }

        public virtual void BindWidgetBlockMouse(Widget widget, bool withDispose = true, bool twoSideBlock = false)
        {
            widget.MouseEntered += _widget_MouseEntered;
            widget.MouseLeft += twoSideBlock ? _widget_MouseEntered : _widget_MouseLeft;

            if (withDispose)
                OnDispose += () =>
                {
                    widget.MouseEntered -= _widget_MouseEntered;
                    widget.MouseLeft -= _widget_MouseLeft;
                };
        }

        public void UnBindWidgetBlockMouse(Widget widget)
        {
            widget.MouseEntered -= _widget_MouseEntered;
            widget.MouseLeft -= _widget_MouseLeft;
        }

        protected void _widget_MouseLeft(object sender, EventArgs e)
        {
            Game.IsMouseMoveAvailable = true;
#if DEBUG
            Console.WriteLine($"{sender} mouse active");
#endif
            if (NOLOOSEBLOCK)
                NabunassarGame.Game.IsMouseMoveAvailable = false;
            NOLOOSEBLOCK = false;
#if DEBUG
            Console.WriteLine("Mouse block restored.");
#endif
        }

        protected void _widget_MouseEntered(object sender, EventArgs e)
        {
            Game.IsMouseMoveAvailable = false;
#if DEBUG
            Console.WriteLine($"{sender} mouse disabled");
#endif
        }

        public Action OnDispose { get; set; }

        public bool IsRemoved { get; internal set; }

        public virtual void Dispose()
        {
            UnloadContent();
            Game.Components.Remove(this);
            OnDispose?.Invoke();
            MapObject = null;
            Game.IsMouseMoveAvailable = true;

            if (!IsRemoved)
                Game.RemoveDesktopWidget(this);
        }

        public virtual void Close()
        {
            Dispose();
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }
    }
}
