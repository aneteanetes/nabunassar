using LiteDB;
using Microsoft.Xna.Framework.Content;

namespace Nabunassar.Content
{
    internal class ResourceLoader : IDisposable
    {
        private NabunassarGame game;

        public ResourceLoader(NabunassarGame game, string moduleName = null)
        {
            this.game = game;

            if (moduleName != null)
                ModuleName = moduleName;
        }

        private readonly string ModuleName = "BaseGame";

        private LiteDatabase _dataBase;
        private LiteDatabase Database
        {
            get
            {
                try
                {
                    if (_dataBase == default)
                    {
                        var dataPath = game.Settings.PathData;
                        if (!Directory.Exists(dataPath))
                        {
                            Directory.CreateDirectory(dataPath);
                        }

                        var datafilePath = Path.Combine(dataPath, ModuleName + Resource.DataFileExtension);

                        _dataBase = new LiteDatabase(datafilePath);
                    }
                    return _dataBase;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }

        public bool IsExists(string assetName)
        {
            var db = Database?.GetCollection<Resource>();

            if (db == null)
                throw new ContentLoadException("Database file or collection is not found!");

            return db.Exists(x => x.Path == assetName);
        }

        public MemoryStream GetStream(string assetName)
        {
            try
            {
                Resource res = null;
                var db = Database?.GetCollection<Resource>();
                if (db != null)
                {
                    try
                    {
                        res = db.Find(x => x.Path == assetName).FirstOrDefault();
                    }
                    catch { }
                }

                if (res == null)
                    throw new ContentLoadException($"Resource {assetName} is not found");

                return res.Stream;
            }
            catch (FileNotFoundException innerException)
            {
                throw new ContentLoadException("The content file was not found.", innerException);
            }
            catch (DirectoryNotFoundException innerException2)
            {
                throw new ContentLoadException("The directory was not found.", innerException2);
            }
            catch (Exception innerException3)
            {
                throw new ContentLoadException("Opening stream error.", innerException3);
            }
        }

        public void Dispose()
        {
            _dataBase?.Dispose();
        }
    }
}
