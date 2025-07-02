using Microsoft.Extensions.Configuration;
using Nabunassar;
using Nabunassar.Content.Compiler;
using Nabunassar.Monogame.Settings;

var config = new ConfigurationBuilder()
                .AddJsonFile($"nabunassar.cfg", true)
                .AddJsonFile($"nabunassar.local.cfg", true)
                .Build();

var settings = config.Get<GameSettings>();
settings.Initialize();

#if DEBUG
ResourceCompiler.Compile(settings);
#endif

using var game = new NabunassarGame(settings);
game.Run();


