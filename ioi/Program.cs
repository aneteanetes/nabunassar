global using ioi;
global using Myra.Events;
global using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;
using ioi.Content.Compiler;
using ioi.Monogame.Settings;
using Microsoft.Extensions.Configuration;
//using SadConsole;
//using SadConsole.Configuration;

//SadConsole.Configuration.Builder
//    .GetBuilder()
//    .SetWindowSizeInCells(90, 30)
//    .ConfigureFonts(true)
//    .UseDefaultConsole()
//    .OnStart(Startup)
//    .Run();

//static void Startup(object? sender, SadConsole.GameHost host)
//{
//    SadConsole.Game.Instance.StartingConsole!.FillWithRandomGarbage(SadConsole.Game.Instance.StartingConsole!.Font);
//    SadConsole.Game.Instance.StartingConsole.Fill(new SadRogue.Primitives.Rectangle(3, 3, 23, 3), SadRogue.Primitives.Color.Violet, SadRogue.Primitives.Color.Black, 0, Mirror.None);
//    SadConsole.Game.Instance.StartingConsole.Print(4, 4, "Hello from SadConsole");
//}

var config = new ConfigurationBuilder()
                .AddJsonFile($"ioi.cfg", true)
                .AddJsonFile($"ioi.local.cfg", true)
                .Build();

var settings = config.Get<GameSettings>();
settings.Initialize();

if (settings.IsResourceCompiling)
    ResourceCompiler.Compile(settings);

using var game = new ioi.GameHost(settings);
game.Run();