using LiteDB;
using Nabunassar.Entities.Game;
using Nabunassar.Monogame.Settings;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Nabunassar.Content.Compiler
{
    /// <summary>
    /// Компилятор ресурсов
    /// </summary>
    public class ResourceCompiler
    {
        string manifestPath;

        GameSettings configuration;

        /// <summary>
        /// Манифест предыдущего билда
        /// </summary>
        public ResourceManifest LastBuild { get; private set; }

        /// <summary>
        /// Манифест текущего билда
        /// </summary>
        public ResourceManifest CurrentBuild { get; private set; }

        private const string ResourceManifestName = "ResourceManifest.ndm";

        private const string BaseModuleName = "BaseGame";

        private ResourceCompiler(GameSettings settings)
        {
            configuration = settings;

            manifestPath = Path.Combine(configuration.PathData, ResourceManifestName);

            CurrentBuild = new ResourceManifest();
        }

        private void LoadLastBuild()
        {
            if (File.Exists(manifestPath))
            {
                LastBuild = JsonConvert.DeserializeObject<ResourceManifest>(File.ReadAllText(manifestPath));
            }
            else
            {
                Console.WriteLine("Resource manifest not found!");
                LastBuild = new ResourceManifest();
            }
        }

        private void WriteCurrentBuild()
        {
            var manifest = JsonConvert.SerializeObject(CurrentBuild, Formatting.Indented);
            File.WriteAllText(manifestPath, manifest);
        }

        internal static void Compile(GameSettings cfg)
        {
            var compiler = new ResourceCompiler(cfg);
            compiler.LoadLastBuild();

            var resDir = Path.Combine(compiler.configuration.PathProject, "Resources");
            var folders = Directory.GetDirectories(resDir)
                .Where(x => Directory.GetFiles(x, "*.*", SearchOption.AllDirectories).Length > 0)
                .ToArray();

            if (!Directory.Exists(compiler.configuration.PathData))
                Directory.CreateDirectory(compiler.configuration.PathData);

            foreach (var folder in folders)
            {
                var dir = new DirectoryInfo(folder);
                var resFilePath = Path.Combine(compiler.configuration.PathData, $"{dir.Name}{Resource.DataFileExtension}");
                using var resDb = new LiteDatabase(resFilePath);
                var resources = resDb.GetCollection<Resource>();
                resources.EnsureIndex("Path");

                compiler.ProcessResourcesFolder(folder, resources);
            }

            compiler.WriteCurrentBuild();
        }

        private void ProcessResourcesFolder(string folderPath, ILiteCollection<Resource> db)
        {
            var filePaths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            var formattedFilePaths = filePaths.Select(FormatPathForDB);

            var currentRes = db.Query().Select(x => x.Path).ToArray();

            // удалить из БД удалённые ресурсы
            var filesForDelete = currentRes.Except(formattedFilePaths);
            foreach (var fileForDelete in filesForDelete)
            {
                Console.WriteLine($"Resource {fileForDelete} was deleted!");
                db.DeleteMany(x => x.Path == fileForDelete);
            }

            // обрабатываем каждую папку в Resources как файл
            foreach (string filePath in filePaths)
            {
                try
                {
                    ProcessFile(filePath, db);
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    throw;
                }
            }
        }

        /// <summary>
        /// Переносит файлы из папки в БД
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="db"></param>
        private void ProcessFile(string filePath, ILiteCollection<Resource> db)
        {
            var formattedPath = FormatPathForDB(filePath);
            var lastTime = File.GetLastWriteTime(filePath);
            var res = LastBuild.Resources.FirstOrDefault(x => x.Path == formattedPath);

            CurrentBuild.Resources.Add(new Resource() { Path = formattedPath, LastWriteTime = lastTime });

            if (res == default)
            {
                var newResource = new Resource()
                {
                    Path = formattedPath,
                    LastWriteTime = lastTime,
                    Data = GetData(filePath)
                };
                db.Insert(newResource);

                if (configuration.IsLogging)
                    Console.WriteLine($"Resource {formattedPath} was added!");
            }
            else
            {
                if (res.LastWriteTime.ToString() != lastTime.ToString())
                {
                    var dataResource = db.Find(x => x.Path == formattedPath).FirstOrDefault();
                    dataResource.Data = GetData(filePath);
                    dataResource.LastWriteTime = lastTime;
                    db.Update(dataResource);

                    if (configuration.IsLogging)
                        Console.WriteLine($"Resource {formattedPath} was updated!");
                }
            }
        }

        private byte[] GetData(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        private string FormatPathForDB(string filePath)
            => Path.GetRelativePath(Path.Combine(configuration.PathRepository,Assembly.GetEntryAssembly().GetName().Name,"Resources",BaseModuleName), filePath).Replace("\\","/");

    }
}