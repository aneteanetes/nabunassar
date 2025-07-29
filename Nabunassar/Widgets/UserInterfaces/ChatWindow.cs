using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;
using Nabunassar.Widgets.UserInterfaces.GameWindows.Informations;
using System.Reflection;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class ChatWindow : ScreenWidgetWindow
    {
        private static FontSystem _font;
        private Texture2D chatborder;
        private Texture2D chatborderBlack;
        private TextureRegion resizeImage;
        private Texture2D pixel;
        private Texture2D sliderknob;
        private static ChatWindow ChatWindowWidget;

        public ChatWindow(NabunassarGame game) : base(game)
        {
            ChatWindowWidget = this;
        }

        protected override void LoadContent()
        {
            _font = Game.Content.LoadFont(Fonts.Retron);
            chatborder = Game.Content.Load<Texture2D>("Assets/Images/Borders/panel-border-007sm.png");
            chatborderBlack = Game.Content.Load<Texture2D>("Assets/Images/Borders/panel-border-007sm_b.png");
            resizeImage = new TextureRegion(Game.Content.Load<Texture2D>("Assets/Tilesets/cursor_tilemap_packed.png"), new Rectangle(64, 96, 16, 16));
            pixel = Game.Content.Load<Texture2D>("Assets/Images/pixel.png");
            sliderknob = Game.Content.Load<Texture2D>("Assets/Images/Borders/sliderknob.png");
            base.LoadContent();

            _verticalScrollbarThumbAccessor = typeof(ScrollViewer).GetField("_verticalScrollbarThumb", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            ThumbPositionAccessor = typeof(ScrollViewer).GetProperty("ThumbPosition", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public override bool IsCloseable => false;

        protected override bool IsMouseMovementAvailableWithThisActivedWidget => true;

        public override bool IsRemovable => false;

        private static ScrollViewer scrollcontainer;
        private static VerticalStackPanel messageBox;

        protected override Window CreateWindow()
        {
            var window = new Window();

            scrollcontainer = new ScrollViewer();
            //scrollcontainer.VerticalScrollBackground = new TextureRegion(pixel);
            //scrollcontainer.VerticalScrollKnob = new TextureRegion(sliderknob);

            scrollcontainer.TouchDown += Scrollcontainer_TouchDown;
            scrollcontainer.TouchUp += Scrollcontainer_TouchUp;

            messageBox = new VerticalStackPanel();
            messageBox.MinHeight = minimalHeight;
            messageBox.Width = 550;

            //messageBox = new VerticalStackPanel();

            scrollcontainer.Height = minimalHeight;
            scrollcontainer.Content = messageBox;

            //content.Widgets.Add(messageBox);

            window.Content = scrollcontainer;

            window.DragDirection = DragDirection.None;
            window.TitlePanel.TouchDown += TitlePanel_TouchDown;
            window.TitlePanel.TouchDoubleClick += TitlePanel_TouchDoubleClick;

            window.Left = 0;
            window.Top = minimalTop;

            var resizeimg = new Image()
            {
                Renderable = resizeImage,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 12,
                Height = 12
            };

            StackPanel.SetProportionType(resizeimg, ProportionType.Fill);

            window.TitlePanel.Widgets.Clear();
            window.TitlePanel.Widgets.Add(resizeimg);

            return window;
        }

        private void Scrollcontainer_TouchUp(object sender, EventArgs e)
        {
            var scroll = sender.As<ScrollViewer>();
            var r = _verticalScrollbarThumbAccessor.GetValue(scroll).As<Rectangle>();
            var thumbPosition = ThumbPositionAccessor.GetValue(scroll).As<Point>();
            r.Y += thumbPosition.Y;

            if (Game.Desktop.TouchPosition != null)
            {
                var touchPosition = scroll.ToLocal(Game.Desktop.TouchPosition.Value);

                if (!r.Contains(touchPosition))
                {
                    _widget_MouseLeft(sender, e);
                }
            }
        }

        private void Scrollcontainer_TouchDown(object sender, EventArgs e)
        {
            var scroll = sender.As<ScrollViewer>();
            var r = _verticalScrollbarThumbAccessor.GetValue(scroll).As<Rectangle>();
            var thumbPosition = ThumbPositionAccessor.GetValue(scroll).As<Point>();
            r.Y += thumbPosition.Y;

            var touchPosition = scroll.ToLocal(Game.Desktop.TouchPosition.Value);

            if (r.Contains(touchPosition))
            {
                _widget_MouseEntered(sender, e);
            }
        }

        private static int trashhold = 20;

        private static Label AddMessageLabel(string message)
        {
            var label = new Label()
            {
                Wrap = true,
                Text = message,
                Padding = new Myra.Graphics2D.Thickness(0, 5, 0, 0),
            };

            if (messageBox.Widgets.Count > trashhold)
            {
                ChatWindowWidget.ClearMessageBox(1);
            }

            messageBox.Widgets.Add(label);
            messageBox.InvalidateMeasure();
            SetScrollDown();

            return label;
        }

        public void ClearMessageBox(int? count = null)
        {
            List<Widget> widgets = new List<Widget>();
            for (int i = 0; i < (count ?? messageBox.Widgets.Count); i++)
            {
                var widget = messageBox.Widgets[i + 1];
                widget.MouseEntered -= Label_MouseEntered;
                widget.MouseLeft += Label_MouseLeft;
                ChatWindowWidget.UnBindWidgetBlockMouse(widget);
                widgets.Add(widget);
            }

            foreach (var widget in widgets)
            {
                messageBox.Widgets.Remove(widget);
            }
        }

        public static void AddMessage(string message)
            => AddMessageLabel(message);

        public static void AddRollMessage(string message, RollResult rollResult)
        {
            var label = AddMessageLabel(message);
            label.TouchDown += (s, e) =>
            {
                var window = new DiceResultWindow(ChatWindowWidget.Game, new Entities.Game.GameObject()
                {
                    RollResult = rollResult,
                    ObjectType = Struct.ObjectType.RollResult
                });
                Open(window);
            };
            label.MouseEntered += Label_MouseEntered;
            label.MouseLeft += Label_MouseLeft;
            ChatWindowWidget.BindWidgetBlockMouse(label, false);
        }

        private static void Label_MouseLeft(object sender, EventArgs e)
        {
            NabunassarGame.Game.GameState.Cursor.SetCursor();
        }

        private static void Label_MouseEntered(object sender, EventArgs e)
        {
            NabunassarGame.Game.GameState.Cursor.SetCursor("info");
        }

        private void TitlePanel_TouchDoubleClick(object sender, EventArgs e)
        {
            SetWindowPosition(minimalTop);
            SetScrollDown();
        }

        protected override void InitWindow(Window window)
        {
            Position = new Vector2(0, minimalTop);

            window.Background = chatborderBlack.NinePatch();
            window.OverBackground = chatborder.NinePatch();

            BindWidgetBlockMouse(window.TitlePanel);

            window.CloseKey = Microsoft.Xna.Framework.Input.Keys.None;
            window.TitlePanel.Opacity = 1;
        }

        private bool isResizing = false;


        private void TitlePanel_TouchDown(object sender, EventArgs e)
        {
            isResizing = true;
        }

        private int minimalHeight = 150;
        private static int minimalTop = 895;
        private FieldInfo _verticalScrollbarThumbAccessor;

        public PropertyInfo ThumbPositionAccessor { get; private set; }

        public override void Update(GameTime gameTime)
        {

            var mouse = MouseExtended.GetState();
            if (mouse.WasButtonReleased(MouseButton.Left))
                isResizing = false;

            if (isResizing)
            {
                if (mouse.PositionChanged && mouse.Position.Y >= 0)
                {
                    var pos = mouse.Position.ToVector2();
                    SetWindowPosition((int)pos.Y);
                    SetScrollDown();
                }

            }

            base.Update(gameTime);
        }

        private static void SetScrollDown()
        {
            scrollcontainer.ScrollPosition = new Point(0, messageBox.ActualBounds.Size.Y);
        }

        private void SetWindowPosition(int y)
        {
            Window.Top = y;

            scrollcontainer.Height = minimalTop + minimalHeight - y;


            if (scrollcontainer.Height < minimalHeight)
                scrollcontainer.Height = minimalHeight;

            if (Window.Top > minimalTop)
                Window.Top = minimalTop;

            if (Window.Top < 0)
                Window.Top = 0;
        }
    }
}
