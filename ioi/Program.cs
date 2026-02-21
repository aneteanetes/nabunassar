global using ioi;
global using Myra.Events;
global using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;
using ioi.Content.Compiler;
using ioi.Monogame.Settings;
using Microsoft.Extensions.Configuration;
using MoonSharp.Interpreter;

var config = new ConfigurationBuilder()
                .AddJsonFile($"ioi.cfg", true)
                .AddJsonFile($"ioi.local.cfg", true)
                .Build();

var settings = config.Get<GameSettings>();
settings.Initialize();

if (settings.IsResourceCompiling)
    ResourceCompiler.Compile(settings);

//var script = new Script();
//// Создаем глобальную таблицу для всех типов
//var charactersTable = new Table(script);
//script.Globals["CharacterTypes"] = charactersTable;

//charactersTable.Values

//string path = "Characters/";
//foreach (var file in Directory.GetFiles(path, "*.lua"))
//{
//    // Выполняем файл и получаем возвращаемую таблицу
//    DynValue characterData = script.DoFile(file);

//    // Используем имя файла как ключ (например, "Warrior")
//    string typeName = Path.GetFileNameWithoutExtension(file);
//    charactersTable[typeName] = characterData;
//}

using var game = new ioi.GameHost(settings);
game.Run();