using MoonSharp.Interpreter;

namespace ioi.Systems.Roguelike
{
    [MoonSharpUserData]
    public class ItemRandomSystem
    {
        internal GameHost Game { get; }

        internal ItemRandomSystem(GameHost game)
        {
            Game = game;
        }

        public int GetSeed()
        {
           return Game.Random.Next();
        }

        public Random GetGenerator(double seedFromLua)
        {
            int seed = ((int)Math.Round(seedFromLua));
            return new Random(seed);
        }
    }
}