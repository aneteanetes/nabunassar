using ioi.Screens.LoadingScreens;
using MoonSharp.Interpreter;
using System.Collections;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    internal class LoadingSystem : IDisposable
    {
        private LoadingComponent loading;

        public GameHost Game { get; private set; }

        public LoadingSystem(GameHost game)
        {
            Game = game;
        }

        public void LoadContent()
        {
            loading = new LoadingComponent(Game);
            loading.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (!loading.IsLoaded)
                loading.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (!loading.IsLoaded)
                loading.Draw(gameTime);
        }

        public event Action OnLoaded;

        public void LoadCenter(IEnumerator loadingProcess,int ms, IEnumerator afterLoad=default)
        {
            if (ms == 0)
                ms = 100;

            loading.X = Game.MapViewport.X;
            loading.Y = Game.MapViewport.Y;
            loading.Width= Game.MapViewport.Width;
            loading.Height= Game.MapViewport.Height;
            loading.Load(loadingProcess,TimeSpan.FromMilliseconds(ms), afterLoad);
        }

        public void Dispose()
        {
            loading = null;
        }
    }
}