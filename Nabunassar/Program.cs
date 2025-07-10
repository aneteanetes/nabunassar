using Microsoft.Extensions.Configuration;
using Nabunassar;
using Nabunassar.Content.Compiler;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Monogame.Settings;



/*
Цель: Ранг_Замка* N + Уровень_Замка (d4/d6/d8/d10/d12)
Бросок: Ранг_Умения + Кубик_Умения + Кубик_Характеристики


Ранг_Замка - 2
Уровень_Замка - d8

Ранг_Умения = 2
Кубик_Умения = d12
Кубик_Характеристики = d8
*/


bool roll()
{
    var @lock = ((int)Rank.Advanced) * 2 + Dice.d12;
    var user = ((int)Rank.Advanced) + Dice.d12 + Dice.d8;
    Console.WriteLine($"lock: {@lock}, roll: {user}, {(user>=@lock ? "success" : "failure")}");

    return user >= @lock;
}

//int success = 0;
//int failure = 0;

//for (int i = 0; i < 10000; i++)
//{
//    var isSuccess = roll();
//    if (isSuccess)
//        success++;
//    else
//        failure++;
//}

//Console.WriteLine($"success: {success}, failure: {failure}");

//Console.ReadLine();

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


