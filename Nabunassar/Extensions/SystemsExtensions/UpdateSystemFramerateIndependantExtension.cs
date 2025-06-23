using MonoGame.Extended.ECS.Systems;

namespace Nabunassar
{
    internal static class UpdateSystemFramerateIndependantExtension
    {
        private static Dictionary<IUpdateSystem, double> updatesCache = new();

        public static bool CanUpdate(this IUpdateSystem system, GameTime gameTime, double delayMilliseconds)
        {
            if (!updatesCache.ContainsKey(system))
            {
                updatesCache.Add(system, 0);
            }

            updatesCache[system] += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (updatesCache[system] < delayMilliseconds)
            {
                return false;
            }
            updatesCache[system] = 0;
            return true;
        }
    }
}
