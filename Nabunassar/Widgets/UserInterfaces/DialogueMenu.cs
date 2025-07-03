using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using Myra.Graphics2D.Brushes;
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

        public DialogueMenu(NabunassarGame game, GameObject gameObject) : base(game)
        {
            _gameObject = gameObject;
        }

        protected override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);
            _background = Content.Load<Texture2D>("Assets/Images/Borders/panel-030.png");
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


            return panel;
        }

        private Panel PartyBubble()
        {
            return new Panel();
        }

        private Panel SpeakerBubble()
        {
            return new Panel();
        }

        private Panel DialogueOptionsPanel()
        {
            var panel = new Panel();
            var gray = Color.Gray;
            panel.Background = _background.NinePatchDouble();
            panel.Padding = new Myra.Graphics2D.Thickness(24);
            panel.Width = 1000;
            panel.Height = 150;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Bottom;

            panel.Top = -50;

            var replicsCount = 4;
            var replics = Enumerable.Range(0,4).Select(x=>Guid.NewGuid().ToString()).ToArray();

            var y = 0;

            for (int i = 0; i < replicsCount; i++)
            {
                var btn = new Button();
                btn.Top = y;

                btn.Background = new SolidBrush(Color.Transparent);

                y += 26;

                var label = new Label();
                label.Font = _font.GetFont(25);
                label.Text = $"{i}. "+replics[i];

                btn.Content = label;

                btn.HorizontalAlignment = HorizontalAlignment.Left;

                panel.Widgets.Add(btn);
            }

            return panel;
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = KeyboardExtended.GetState();

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Close();
            }

            base.Update(gameTime);
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
