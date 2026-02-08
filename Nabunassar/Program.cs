global using Nabunassar;
global using Myra.Events;
global using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;
using Microsoft.Extensions.Configuration;
using Nabunassar.Content.Compiler;
using Nabunassar.Monogame.Settings;

var config = new ConfigurationBuilder()
                .AddJsonFile($"nabunassar.cfg", true)
                .AddJsonFile($"nabunassar.local.cfg", true)
                .Build();

var settings = config.Get<GameSettings>();
settings.Initialize();

if (settings.IsResourceCompiling)
    ResourceCompiler.Compile(settings);

using var game = new NabunassarGame(settings);
game.Run();