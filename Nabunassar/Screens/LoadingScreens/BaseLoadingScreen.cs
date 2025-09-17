using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Screens.Transitions;
using Nabunassar.Screens.Abstract;
using System.Runtime.CompilerServices;

namespace Nabunassar.Screens.LoadingScreens
{
    internal class BaseLoadingScreen : BaseGameScreen
    {
        private Sprite _iconSprite;
        private Vector2 _iconPos;
        private float _rotation;
        private string _loadText;
        private bool _isLoadingStarted;
        private bool _isLoaded;
        private bool _isTransitionEnded;
        private bool _isAllCompleted;

        public BaseLoadingScreen(NabunassarGame game) : base(game)
        {
        }

        public BaseGameScreen NextScreen { get; set; }

        public Func<Task> Loading { get; set; }

        public override void LoadContent()
        {
            _iconSprite = new Sprite(Content.LoadTexture("Assets/Images/Backgrounds/star.png"));

            var posOffset = 100;
            _iconPos = new Vector2(posOffset, Game.Resolution.Height - posOffset);
            _rotation = 0f;
            _iconSprite.Origin = new Vector2(_iconSprite.TextureRegion.Width / 2, _iconSprite.TextureRegion.Height / 2);

            _loadText = Game.Strings["UI"]["Loading"];
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.End();
            var sb = Game.BeginDraw();

            sb.Draw(_iconSprite, _iconPos, _rotation, new Vector2(.25f));

            sb.DrawText(Fonts.FritzQuadranta, 50, _loadText, _iconPos + new Vector2(50, -20), Globals.BaseColor);

            Game.SpriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (!_isLoadingStarted)
            {
                _isLoadingStarted = true;
                RunLoading();
            }

            if (this.CanUpdate(gameTime, TimeSpan.FromMilliseconds(.5)))
            {
                if (_rotation == 1)
                    _rotation = 0;
                _rotation += 0.001f;
            }

            if (_loadText.CanUpdate(gameTime, TimeSpan.FromMilliseconds(250)))
            {
                _loadText += ".";

                if (_loadText.Contains("...."))
                    _loadText = _loadText.Replace("....", "");
            }

            if (_isLoaded && _isTransitionEnded && !_isAllCompleted)
            {
                Game.SwitchScreenInternal(NextScreen);
                _isAllCompleted = true;
            }
        }

        public void TransitionCompleted()
        {
            _isTransitionEnded = true;
        }

        private void RunLoading()
        {
            if (Loading != null)
            {
                Task.Run(Loading).ContinueWith(t =>
                {
                    _isLoaded = true;
                });
            }
            else
            {
                _isLoaded = true;
            }
        }
    }
}