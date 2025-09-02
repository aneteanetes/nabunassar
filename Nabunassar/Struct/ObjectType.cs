using Nabunassar.Entities.Game;

namespace Nabunassar.Struct
{
    internal enum ObjectType
    {
        // common objects
        None,
        Ground,

        // interactive objects
        Door,
        Tree,
        Wall,
        Object,
        NPC,
        Container,

        // other objects
        Player,
        Cursor,
        Pathing,
        Dummy,
        Hero,
        Sprite,
        Interface,
        Border,
        Hull,
        RollResult,
        Merchant,
    }

    public static class ObjectTypeInspectorExtensions
    {
        internal static bool IsInteractive(this ObjectType objectType)
        {
            var code = (int)objectType;

            return code > 1 && code < 8;
        }

        /// <summary>
        /// (<see cref="ObjectType"/>)<see cref="object.ToString"/>+LoadedMap.AreaObjectPostfix
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        internal static string ObjectTypeInLoadedMap(this ObjectType objectType)
        {
            return objectType.ToString() + NabunassarGame.Game.GameState.Location.LoadedMap.GetPropertyValue<string>("AreaObjectPostfix");
        }
    }
}
