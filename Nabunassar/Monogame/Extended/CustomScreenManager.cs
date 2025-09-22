using MonoGame.Extended;
using Nabunassar.Screens.Abstract;

namespace Nabunassar.Monogame.Extended
{
    internal class CustomScreenManager : SimpleDrawableGameComponent
    {
        public BaseScreen ActiveScreen => _activeScreen;
        private BaseScreen _activeScreen;
        private bool _disabledActive;
        private IScreenTransition _activeTransition;
        private NabunassarGame _game;

        public CustomScreenManager(NabunassarGame game)
        {
            _game = game;
        }

        public void LoadScreen(BaseScreen screen, IScreenTransition transition)
        {
            if (_activeTransition == null)
            {
                _activeScreen?.Dump();
                _disabledActive = true;

                _activeTransition = transition;
                _activeTransition.StateChanged += delegate
                {
                    LoadScreen(screen);
                    _disabledActive = false;
                };
                _activeTransition.Completed += delegate
                {
                    _activeTransition.Dispose();
                    _activeTransition = null;
                };
            }
        }

        public void LoadScreen(BaseScreen screen)
        {
            if (_activeScreen != null)
            {
                _activeScreen?.UnloadContent();
                _activeScreen?.Dispose();
            }

            screen.Initialize();
            screen.LoadContent();
            _activeScreen = screen;
        }

        public override void Initialize()
        {
            base.Initialize();
            _activeScreen?.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _activeScreen?.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _activeScreen?.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!_disabledActive || (_activeScreen?.IsLoadingScreen ?? false))
                _activeScreen?.Update(gameTime);

            _activeTransition?.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_activeScreen != null)
            {
                if (!_disabledActive || _activeScreen.IsLoadingScreen || _activeScreen.IsDumping)
                {
                    _activeScreen?.Draw(gameTime);
                }
                else
                {
                    if (_activeScreen.DumpedScreen != null)
                    {
                        var sb = _game.BeginDraw(false);
                        sb.Draw(_activeScreen.DumpedScreen, Vector2.Zero, Color.White);
                        sb.End();
                    }
                }
            }

            _activeTransition?.Draw(gameTime);
        }
    }
}
