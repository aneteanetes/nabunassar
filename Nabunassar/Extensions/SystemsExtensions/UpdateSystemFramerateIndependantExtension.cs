using Geranium.Reflection;
using MonoGame.Extended.ECS.Systems;

namespace Nabunassar
{
    internal static class UpdateSystemFramerateIndependantExtension
    {
        private static Dictionary<object, double> updatesCache = new();

        public static bool CanUpdate(this IUpdateSystem system, GameTime gameTime, double delayMilliseconds)
            => IsUpdateAvailable(system, gameTime, delayMilliseconds);

        public static bool IsUpdateAvailable(this IUpdateSystem system, GameTime gameTime, double delayMilliseconds)
            => IsUpdateAvailable(system.As<object>(), gameTime, delayMilliseconds);

        public static bool IsUpdateAvailable(this IUpdateable updateable, GameTime gameTime, double delayMilliseconds)
            => IsUpdateAvailable(updateable.As<object>(), gameTime, delayMilliseconds);

        public static bool IsUpdateAvailable(this object obj, GameTime gameTime, double delayMilliseconds)
        {
            if (!updatesCache.ContainsKey(obj))
            {
                updatesCache.Add(obj, 0);
            }

            updatesCache[obj] += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (updatesCache[obj] < delayMilliseconds)
            {
                return false;
            }
            updatesCache[obj] = 0;
            return true;
        }
    }
}
