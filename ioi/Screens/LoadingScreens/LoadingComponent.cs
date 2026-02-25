using MonoGame.Extended.Graphics;
using MoonSharp.Interpreter;
using System.Collections;
using System.Runtime.CompilerServices;

namespace ioi.Screens.LoadingScreens
{
    [MoonSharpUserData]
    internal class LoadingComponent
    {
        private Sprite _iconSprite;
        private Vector2 _iconPos;
        private float _rotation;
        private string _loadText;

        private Stack<IEnumerator> loading = new();
        IEnumerator _afterLoad = null;

        public GameHost Game { get; }
        private float privateOffset;

        public LoadingComponent(GameHost game)
        {
            Game = game;
            privateOffset = (float)new Random().NextDouble() * 10f;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        private TimeSpan _waiting;

        public bool IsLoaded { get; private set; } = true;

        private bool _isLoadedCorutine = true;

        public void LoadContent()
        {
            _iconSprite = new Sprite(Game.Content.LoadTexture("Assets/Images/Backgrounds/star.png"));

            _rotation = 0f;
            _iconSprite.Origin = new Vector2(_iconSprite.TextureRegion.Width / 2, _iconSprite.TextureRegion.Height / 2);

            _loadText = Game.Strings["UI"]["Loading"];
        }

        public void Update(GameTime gameTime)
        {
            UpdateDraw(gameTime);

            if (_isLoadedCorutine && this.CanUpdate(gameTime, _waiting, privateOffset))
            {
                IsLoaded = true;

                if (_afterLoad != default)
                {
                    while (_afterLoad.MoveNext()) { }
                }
            }
            else
            {
                if (loading.Count > 0)
                    _isLoadedCorutine = Loading();
            }
        }

        private void UpdateDraw(GameTime gameTime)
        {
            var posOffset = ((int)Math.Ceiling(Width * 0.05208));
            _iconPos = new Vector2(posOffset, Height - posOffset);

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

        public void Draw(GameTime gameTime)
        {
            var sb = Game.BeginDraw();

            Game.PixelTexture.SetData([Color.Black]);
            sb.Draw(Game.PixelTexture, new Rectangle(X, Y, Width, Height), Color.Black);
            sb.Draw(_iconSprite, _iconPos, _rotation, new Vector2(.25f));
            sb.DrawText(Fonts.FritzQuadranta, 50, _loadText, _iconPos + new Vector2(50, -20), Globals.BaseColor);

            Game.SpriteBatch.End();
        }

        public void Load(IEnumerator corutine, TimeSpan waiting, IEnumerator afterLoad=null)
        {
            _waiting = waiting;
            IsLoaded = false;
            _afterLoad = afterLoad;
            loading.Push(corutine);
        }

        private bool Loading()
        {
            IEnumerator current = loading.Peek();

            if (current.MoveNext()) //here loading
            {
                if (current.Current is IEnumerator nested)
                {
                    loading.Push(nested);
                }
            }
            else
            {
                loading.Pop();
            }

            if (loading.Count == 0)
                return true;

            return false;
        }
    }
}
