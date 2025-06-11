namespace Nabunassar.Monogame.Interfaces
{
    internal interface ILoadable : IDisposable
    {
        void LoadContent();

        void UnloadContent();
    }
}
