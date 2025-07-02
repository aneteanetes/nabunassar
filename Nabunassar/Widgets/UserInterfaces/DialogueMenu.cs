using FontStashSharp;
using MonoGame.Extended.Input;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Extensions.OrthographCameraExtensions;
using Nabunassar.Resources;
using Nabunassar.Widgets.Base;

namespace Nabunassar.Widgets.UserInterfaces
{
    internal class DialogueMenu : ScreenWidget
    {
        private GameObject _gameObject;
        private FontSystem _font;

        public DialogueMenu(NabunassarGame game, GameObject gameObject) : base(game)
        {
            _gameObject = gameObject;
        }

        protected override void LoadContent()
        {
            _font = Content.LoadFont(Fonts.Retron);
            base.LoadContent();
        }

        protected override Widget InitWidget()
        {
            Game.RemoveDesktopWidgets<TitleWidget>();
            Game.DisableMouseSystems();
            Game.ZoomToPoint(_gameObject.MapObject.Position, 3);

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
            panel.Background = new SolidBrush(Color.Red);
            panel.Width = 1000;
            panel.Height = 150;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Bottom; 

            var replicsCount = 4;
            var replics = Enumerable.Range(0,4).Select(x=>Guid.NewGuid().ToString()).ToArray();

            var y = 0;

            for (int i = 0; i < replicsCount; i++)
            {
                var btn = new Button();
                btn.Top = y;

                y += 25;

                var label = new Label();
                label.Font = _font.GetFont(25);
                label.Text = replics[i];

                btn.Content = label;

                panel.Widgets.Add(btn);
            }

            return panel;
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = KeyboardExtended.GetState();

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Game.ZoomOut(_gameObject.MapObject.Position, 3);
                this.Close();
            }

            base.Update(gameTime);
        }

        public override void Close()
        {
            Game.EnableSystems();
            base.Close();
        }
    }
}
