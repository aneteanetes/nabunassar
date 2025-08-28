using FontStashSharp;
using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Data.Speaking;
using Nabunassar.Entities.Game;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;
using System.Text;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class DialogueMenu : ScreenWidget
    {
        private GameObject _gameObject;
        private FontSystem _font;
        private Texture2D _background;
        private Texture2D _speakBackground;
        private Texture2D _closeTexture;
        private Dialogue _dialogue;

        public DialogueMenu(NabunassarGame game, GameObject gameObject) : base(game)
        {
            _gameObject = gameObject;
        }

        public override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);
            _background = Content.Load<Texture2D>("Assets/Images/Borders/panel-030.png");
            _speakBackground = Content.Load<Texture2D>("Assets/Images/Borders/speakerbubble2.png");
            _closeTexture = Content.Load<Texture2D>("Assets/Tilesets/cursor_tilemap_packed.png");

            if (_gameObject.Dialogue.IsNotEmpty())
                _dialogue = Content.LoadDialogue(_gameObject.Dialogue);

            base.LoadContent();
        }

        private Panel _globalPanel;

        protected override Widget CreateWidget()
        {
            Game.RemoveDesktopWidgets<TitleWidget>();
            Game.DisableMouseSystems();
            Game.Camera.ZoomToPoint(_gameObject.MapObject.Position, 3);

            Game.Camera.ViewOn(_gameObject.MapObject.Position);

            var panel = _globalPanel = new Panel();

            panel.TouchDown += (s, e) => RevealSpeakerText();

            //panel.Background = new SolidBrush(Color.Red);

            panel.Widgets.Add(DialogueOptionsPanel());
            panel.Widgets.Add(SpeakerBubble());

            if (_dialogue == null)
            {
                var generatedText = string.Join(",", Enumerable.Range(0, 10).Select(x => Guid.NewGuid().ToString()));
                SetSpeakerText(generatedText);
            }
            else
            {
                SetSpeakerText(_dialogue.InitialRound.Text);
            }

            return panel;
        }

        private string _speakerText;

        private int _speakerTextCharIndex;

        private Label _speakerLabel;

        private void SetSpeakerText(string txt)
        {
            ChatWindow.AddMessage(Game.Strings["ObjectNames"][_gameObject.Name] + ": " + txt);

            _speakerText = txt;
            _speakerTextCharIndex = 0;
            _speakerLabel.Text = "";
        }

        private void RevealSpeakerText()
        {
            _speakerLabel.Text = _speakerText;
        }

        private Panel SpeakerBubble()
        {
            var width = 550;
            var height = 300;

            var objScreenPosition = Game.Camera.WorldToScreen(_gameObject.MapObject.Position);

            var panel = new Panel();
            var gray = Color.Gray;
            panel.Background = _speakBackground.NinePatchDouble();
            panel.Padding = new Myra.Graphics2D.Thickness(20);
            panel.Width = width;
            panel.Height = height;

            panel.TouchDown += (s, e) => RevealSpeakerText();

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

        private VerticalStackPanel _replicsPanel;

        private Panel DialogueOptionsPanel()
        {
            var panel = new Panel();
            var gray = Color.Gray;
            panel.Background = _background.NinePatchDouble();
            panel.Padding = new Myra.Graphics2D.Thickness(20);
            panel.Width = 750;
            panel.Height = 150;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Bottom;

            panel.Top = -100; //-100


            var scrollContainer = new ScrollViewer();
            scrollContainer.Background = new SolidBrush(Color.Transparent);

            var scroll = _replicsPanel = new VerticalStackPanel();
            FillReplicaButtons();

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

        private void FillReplicaButtons()
        {
            _replicsPanel.Widgets.Clear();
            var replicsCount = _dialogue.CurrentRound.Replics.Count;
            if (replicsCount > 0)
            {
                var replics = _dialogue.CurrentRound.Replics;
                for (int i = 0; i < replicsCount; i++)
                {
                    Button btn = CreateReplicaButton($"{i+1}. " + replics[i].Text, Color.White);

                    btn.Tag = i;
                    btn.Click += (s, e) => SelectReplica(s, e, replics[(int)btn.Tag]);

                    _replicsPanel.Widgets.Add(btn);
                }
            }
            else
            {
                Button btn = CreateReplicaButton(_dialogue.EndDialogueReplica.Text, Color.Gray);

                btn.Click += (s, e) => this.Close();

                _replicsPanel.Widgets.Add(btn);
            }
        }

        private Button CreateReplicaButton(string text, Color textColor)
        {
            var btn = new Button();

            btn.Background = new SolidBrush(Color.Transparent);
            btn.PressedBackground = new SolidBrush(Color.Black);

            var label = new Label();
            label.Font = _font.GetFont(25);
            label.TextColor = Color.White;
            label.Text = text;

            btn.Content = label;
            return btn;
        }

        private void SelectReplica(object sender, MyraEventArgs e, DialogueReplica replica)
        {
            var round = replica.Select();
            SetSpeakerText(round.Text);

            FillReplicaButtons();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = KeyboardExtended.GetState();

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Close();
            }


            if (new[] { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9 }.Any(keyboard.WasKeyPressed))
            {
                var pressed = keyboard.GetPressedKeys()
                    .Where(keyCode => (int)keyCode >= 48 && (int)keyCode <= 57)
                    .FirstOrDefault();

                if(pressed!=default)
                {
                    var index = int.Parse(pressed.ToString().Replace("D", ""));
                    var replica = _dialogue.CurrentRound.Replics.ElementAtOrDefault(index - 1);
                    if (replica != null)
                        SelectReplica(null, null, replica);
                }
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
