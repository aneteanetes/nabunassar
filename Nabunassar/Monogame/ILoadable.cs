namespace Nabunassar.Monogame
{
    internal interface ILoadable : IDisposable
    {
        void LoadContent();

        void UnloadContent();
    }
}
