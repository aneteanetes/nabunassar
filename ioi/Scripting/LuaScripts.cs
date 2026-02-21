using ioi.Components;
using MonoGame.Extended.ECS;
using MoonSharp.Interpreter;

namespace ioi.Scripting
{
    internal class LuaScripts
    {
        public GameHost Game { get; }
        public Table SharedMeta { get; }

        public Script Executor = new Script();

        private FileSystemWatcher _watcher;
        private bool _needsReload;
        private string _changedFile;

        public Table Globals => Executor.Globals;

        public DynValue Execute(string scriptText)
        {
            return Executor.DoString(scriptText);
        }

        public LuaScripts(GameHost game)
        {
            Game = game;
            SharedMeta = new Table(Executor);
            SharedMeta["__index"] = (Func<Table, string, DynValue>)((t, k) => t.GetSmart(k));

        }

        public DynValue Call(object func, params object[] args)
        {
            return Executor.Call(func, args);
        }

        public Table MergeTables(Table t1, Table t2)
        {
            var func = Executor.Globals.Get("Core").Table.Get("mergeTables");
            return Executor.Call(func, t1, t2).Table;
        }

        public Table CreateTable(params string[] templates)
        {
            var table = new Table(Executor);

            Table sources = new Table(Executor);
            foreach (var template in templates)
            {
                sources.Append(DynValue.NewString(template));
            }

            table["_sources"] = sources;
            table.MetaTable = SharedMeta;

            return table;
        }

        public void Init()
        {
            UserData.RegisterType<GameEntity>();

            if (Game.Settings.IsDebug)
            {
                var scriptsPath = Path.Combine(Game.Settings.PathProject, "Resources\\BaseGame\\Data\\Scripts");

                _watcher = new FileSystemWatcher(scriptsPath, "*.lua")
                {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.Attributes |
                       NotifyFilters.CreationTime |
                       NotifyFilters.FileName |
                       NotifyFilters.LastAccess |
                       NotifyFilters.LastWrite |
                       NotifyFilters.Size |
                       NotifyFilters.Security,
                    EnableRaisingEvents = true
                };

                // Это событие сработает в другом потоке!
                _watcher.Changed += (s, e) =>
                {
                    _needsReload = true;
                    _changedFile = e.FullPath;
                };

                var files = Directory.GetFiles(scriptsPath, "*.lua", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    Executor.DoFile(file);
                }
            }
            else
            {
                var scripts = Game.Content.LoadResourcePack("Data/Scprits");
                foreach (var script in scripts)
                {
                    Executor.DoStream(script.Stream);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_needsReload)
            {
                Console.WriteLine($"Updated Lua script: {_changedFile}");
                Executor.DoFile(_changedFile);
                _needsReload = false;
            }
        }
    }
}