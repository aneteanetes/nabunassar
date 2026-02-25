using ioi.Screens.Abstract;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using System.Collections;

namespace ioi.Screens.LoadingScreens
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


        private Stack<IEnumerator> loading = new();
        private LoadingComponent loader;

        public override bool IsLoadingScreen => true;

        public BaseLoadingScreen(GameHost game, BaseScreen next, IEnumerator loadingEnty = null) : base(game)
        {
            if (loadingEnty != default)
                loading.Push(loadingEnty);
            NextScreen = next;
        }

        public BaseScreen NextScreen { get; set; }

        public override void LoadContent()
        {
            GameController.SetCameraToScreen();

            loader = new LoadingComponent(Game);
            loader.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.End();

            loader.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            loader.X = Game.MainViewport.X;
            loader.Y = Game.MainViewport.Y;
            loader.Width = Game.MainViewport.Width;
            loader.Height = Game.MainViewport.Height;
            loader.Update(gameTime);
            //UpdateScreen(gameTime);

            if (_isAllCompleted)
                return;

            if (_isTransitionEnded)
            {
                IEnumerator current = loading.Peek();

                // if it's nested IEnumerator - add to loadings
                if (current.MoveNext()) //here loading
                {
                    if (current.Current is IEnumerator nested)
                    {
                        loading.Push(nested);
                    }
                }
                else
                {
                    // return to parent IEnumerator
                    loading.Pop();
                }

                if (loading.Count==0)
                    _isLoaded = true;
            }

            if (_isLoaded && _isTransitionEnded)
            {
                Game.SwitchScreenInternal(NextScreen);
                _isAllCompleted = true;
            }
        }

        public void TransitionCompleted()
        {
            _isTransitionEnded = true;
        }
    }
}