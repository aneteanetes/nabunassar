using MonoGame.Extended.Input;
using Myra.Graphics2D.UI;
using Nabunassar.Entities.Game;
using Nabunassar.Extensions.OrthographCameraExtensions;
using Nabunassar.Systems;

namespace Nabunassar.Desktops.UserInterfaces
{
    internal class DialogueMenu : ScreenWidget
    {
        private GameObject _gameObject;

        public DialogueMenu(NabunassarGame game, GameObject gameObject) : base(game)
        {
            _gameObject = gameObject;
        }

        protected override Widget InitWidget()
        {
            Game.DisableSystems(typeof(PlayerControllSystem),typeof(FocusSystem));
            Game.ZoomToPoint(_gameObject.MapObject.Position, 4);

            return new Panel();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = KeyboardExtended.GetState();

            if (keyboard.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Game.ZoomOut(_gameObject.MapObject.Position, 4);
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
