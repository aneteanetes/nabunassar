using Microsoft.Extensions.Configuration;
using Nabunassar;
using Nabunassar.Content.Compiler;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Monogame.Settings;
using System.Reflection;


var members = typeof(Rank).GetMembers(BindingFlags.Static | BindingFlags.Public)
.OfType<FieldInfo>().ToList();


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


