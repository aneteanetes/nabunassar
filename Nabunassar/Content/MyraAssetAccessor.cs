using AssetManagementBase;
using System.Reflection;

namespace Nabunassar.Content
{
    internal class MyraAssetAccessor : IAssetAccessor
    {
        public string Name => nameof(MyraAssetAccessor);

        private ResourceLoader _resourceLoader;
        public MyraAssetAccessor(ResourceLoader resourceLoader)
        {
            this._resourceLoader= resourceLoader;
        }

        public bool Exists(string path) =>_resourceLoader.IsExists(path);

        public Stream Open(string path)
        {
            path = path.Replace(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),"Data") + "/", "");
            return _resourceLoader.GetStream(path);
        }
    }
}
