namespace Nabunassar
{
    internal interface IScreenTransition : IDisposable
    {
        event EventHandler StateChanged;

        event EventHandler Completed;

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);
    }
}
