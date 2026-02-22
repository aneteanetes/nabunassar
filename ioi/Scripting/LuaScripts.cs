using ioi.Components;
using MoonSharp.Interpreter;

namespace ioi.Scripting
{
    internal class LuaScripts
    {
        public GameHost Game { get; }
        public Table LuaMeta { get; }

        public Script Executor;

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
            
            Executor = new Script();
            Script.DefaultOptions.DebugPrint = s => Console.WriteLine(s);

            LuaMeta = new Table(Executor);
            LuaMeta["__index"] = (Func<Table, string, DynValue>)((t, k) => t.GetSmart(k));
            Executor.Globals.Set("LuaMeta", DynValue.NewTable(LuaMeta));

        }

        public DynValue Call(DynValue function, params object[] args)
        {
            try
            {
                return Executor.Call(function, args);
            }
            catch (InterpreterException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Lua: {ex.DecoratedMessage}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Lua unhandled: {ex}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            return DynValue.Nil;
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

            table["_components"] = sources;
            table.MetaTable = LuaMeta;

            table.Init();

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
                    LoadFile(file);
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
                LoadFile(_changedFile);
                _needsReload = false;
            }
        }

        private void LoadFile(string file)
        {
            try
            {
                Executor.DoFile(file);
            }
            catch (InterpreterException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"Lua: {ex.DecoratedMessage}");

                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}