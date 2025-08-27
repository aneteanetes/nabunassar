using MonoGame.Extended;
using ShaiRandom.Generators;

namespace Nabunassar
{
    internal static class RandomPercentageExtension
    {
        private static IEnhancedRandom _enhancedRandom;

        static RandomPercentageExtension()
        {
            _enhancedRandom = new MizuchiRandom(ulong.Parse(new string(DateTime.UtcNow.Ticks.ToString().Take(8).ToArray())));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chance">from 0 to 1</param>
        /// <returns></returns>
        public static bool Chance(this FastRandom random, float chance)
        {
            return _enhancedRandom.NextBool(chance * 0.01f);
        }
    }
}
