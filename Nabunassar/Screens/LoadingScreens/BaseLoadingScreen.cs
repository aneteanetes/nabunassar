using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using Nabunassar.Screens.Abstract;
using System.Collections;

namespace Nabunassar.Screens.LoadingScreens
{
    internal class BaseLoadingScreen : BaseScreen
    {
        private Sprite _iconSprite;
        private Vector2 _iconPos;
        private float _rotation;
        private string _loadText;
        private bool _isAllCompleted;
        private bool _isLoaded;
        private bool _isTransitionEnded;

        public override bool IsLoadingScreen => true;

        public BaseLoadingScreen(NabunassarGame game) : base(game)
        {
        }

        public BaseScreen NextScreen { get; set; }

        public IEnumerator LoadingCorutine { get; set; }

        public override void LoadContent()
        {
            GameController.SetCameraToScreen();

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
            UpdateScreen(gameTime);

            if (_isAllCompleted)
                return;

            if (_isTransitionEnded)
            {
                if (!LoadingCorutine.MoveNext())
                    _isLoaded = true;
            }

            if (_isLoaded && _isTransitionEnded)
            {
                Game.SwitchScreenInternal(NextScreen);
                _isAllCompleted = true;
            }
        }

        protected virtual void UpdateScreen(GameTime gameTime)
        {
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
        }

        public void TransitionCompleted()
        {
            _isTransitionEnded = true;
        }
    }
}