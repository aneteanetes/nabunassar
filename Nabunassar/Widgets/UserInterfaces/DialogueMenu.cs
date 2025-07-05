using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class DialogueMenu : ScreenWidget
    {
        private GameObject _gameObject;
        private FontSystem _font;
        private Texture2D _background;
        private Texture2D _speakBackground;
        private Texture2D _closeTexture;

        public DialogueMenu(NabunassarGame game, GameObject gameObject) : base(game)
        {
            _gameObject = gameObject;
        }

        protected override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);
            _background = Content.Load<Texture2D>("Assets/Images/Borders/panel-030.png");
            _speakBackground = Content.Load<Texture2D>("Assets/Images/Borders/speakerbubble2.png");
            _closeTexture = Content.Load<Texture2D>("Assets/Tilesets/cursor_tilemap_packed.png");
            base.LoadContent();
        }

        protected override Widget InitWidget()
        {
            Game.RemoveDesktopWidgets<TitleWidget>();
            Game.DisableMouseSystems();
            Game.Camera.ZoomToPoint(_gameObject.MapObject.Position, 3);

            Game.Camera.ViewOn(_gameObject.MapObject.Position);

            var panel = new Panel();

            panel.Widgets.Add(DialogueOptionsPanel());
            panel.Widgets.Add(SpeakerBubble());
            panel.Widgets.Add(PartyBubble());

#warning fullfill speaker text
            var generatedText = string.Join(",", Enumerable.Range(0, 10).Select(x => Guid.NewGuid().ToString()));
            SetSpeakerText(generatedText);

            return panel;
        }

        private Panel PartyBubble()
        {
            return new Panel();
        }

        private string _speakerText;

        private int _speakerTextCharIndex;

        private Label _speakerLabel;
        private Panel _speakerPanel;

        private void SetSpeakerText(string txt)
        {
            _speakerText = txt;
            _speakerTextCharIndex = 0;
            _speakerLabel.Text = "";
        }

        private void RevealSoeakerText()
        {
            _speakerLabel.Text = _speakerText;
        }

        private Panel SpeakerBubble()
        {
            var width = 550;
            var height = 300;

            var objScreenPosition = Game.Camera.WorldToScreen(_gameObject.MapObject.Position);

            var panel = _speakerPanel= new Panel();
            var gray = Color.Gray;
            panel.Background = _speakBackground.NinePatchDouble();
            panel.Padding = new Myra.Graphics2D.Thickness(20);
            panel.Width = width;
            panel.Height = height;

            panel.TouchDown += (s, e) => RevealSoeakerText();

            var objWidth = (int)(_gameObject.MapObject.Bounds.BoundingRectangle.Width * Game.Camera.Zoom);

            panel.Top = ((int)objScreenPosition.Y) - height;
            panel.Left = _gameObject.MapObject.ViewDirection == Struct.Direction.Right
                ? ((int)objScreenPosition.X+ objWidth/2) - width
                : ((int)objScreenPosition.X) + objWidth;

            var scroll = new ScrollViewer();
            scroll.Background = new SolidBrush(Color.Transparent);

            var textContainer = new VerticalStackPanel();
            scroll.Content = textContainer;

            panel.Widgets.Add(scroll);

            textContainer.Widgets.Add(_speakerLabel = new Label()
            {
                Background = new SolidBrush(Color.Transparent),
                //Multiline = true,
                Wrap = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                Font = _font.GetFont(24)
            });

            var closeBtn = new Button();
            closeBtn.Width = 24;
            closeBtn.Height = 24;
            closeBtn.Top = (panel.Padding.Top/2) * -1;
            closeBtn.Left = panel.Padding.Right/2;

            var defBackground = closeBtn.Background;

            closeBtn.Background = new SolidBrush(Color.Transparent);
            closeBtn.OverBackground = defBackground;
            closeBtn.PressedBackground = new SolidBrush(Color.Black);
            closeBtn.Content = new Image()
            {
                //Font = _font.GetFont(24),
                //Text = "X",
                Renderable = new TextureRegion(_closeTexture, new Rectangle(272, 0, 16, 16)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            closeBtn.HorizontalAlignment = HorizontalAlignment.Right;
            closeBtn.VerticalAlignment = VerticalAlignment.Top;
            closeBtn.Click += (x, y) => this.Close();

            panel.Widgets.Add(closeBtn);

            return panel;
        }

        private Panel DialogueOptionsPanel()
        {
            var panel = new Panel();
            var gray = Color.Gray;
            panel.Background = _background.NinePatchDouble();
            panel.Padding = new Myra.Graphics2D.Thickness(20);
            panel.Width = 1000;
            panel.Height = 150;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Bottom;

            panel.Top = -50;

            var replicsCount = 8;
            var replics = Enumerable.Range(0, replicsCount).Select(x=>Guid.NewGuid().ToString()).ToArray();

            var y = 0;

            var scrollContainer = new ScrollViewer();
            scrollContainer.Background = new SolidBrush(Color.Transparent);
            //scrollContainer.ShowVerticalScrollBar = true;

            var scroll = new VerticalStackPanel();

            for (int i = 0; i < replicsCount; i++)
            {
                var btn = new Button();
                btn.Top = y;
                btn.Tag = i;

                btn.Background = new SolidBrush(Color.Transparent);
                btn.PressedBackground = new SolidBrush(Color.Black);

                //y += 26;

                var label = new Label();
                label.Font = _font.GetFont(25);
                label.Text = $"{i}. "+replics[i];

                btn.Content = label;

                btn.Click += (s, e) => SelectReplica(s, e, replics[(int)btn.Tag]);

                //btn.HorizontalAlignment = HorizontalAlignment.Left;

                scroll.Widgets.Add(btn);
            }

            scrollContainer.Content = scroll;
            panel.Widgets.Add(scrollContainer);

            var closeBtn = new Button();
            closeBtn.Width = 24;
            closeBtn.Height = 24;
            closeBtn.Top = panel.Padding.Top * -1;
            closeBtn.Left = panel.Padding.Right;
            closeBtn.PressedBackground = new SolidBrush(Color.Black);
            closeBtn.Content = new Image()
            {
                //Font = _font.GetFont(24),
                //Text = "X",
                Renderable = new TextureRegion(_closeTexture,new Rectangle(272,0,16,16)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            closeBtn.HorizontalAlignment = HorizontalAlignment.Right;
            closeBtn.VerticalAlignment = VerticalAlignment.Top;
            closeBtn.Click += (x, y) => this.Close();

            panel.Widgets.Add(closeBtn);

            return panel;
        }

        private void SelectReplica(object sender, EventArgs e, string replica)
        {
            var speakerText = replica + "!!! "+Guid.NewGuid().ToString();
            SetSpeakerText(speakerText);
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = KeyboardExtended.GetState();

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Close();
            }

            if (this.IsUpdateAvailable(gameTime, 40))
            {
                if (_speakerLabel.Text != _speakerText)
                {
                    _speakerLabel.Text += _speakerText[_speakerTextCharIndex];
                    _speakerTextCharIndex++;
                }
            }
        }

        public override void Close()
        {
            Game.Camera.ViewReset();
            Game.Camera.ZoomOut(_gameObject.MapObject.Position, 3);
            Game.EnableSystems();
            base.Close();
        }
    }
}
