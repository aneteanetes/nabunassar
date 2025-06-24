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

        // other objects
        Player,
        Cursor,
        Pathing,
        Dummy,
        Hero,
        Sprite,
        Interface,
        Border
    }

    public static class ObjectTypeInspectorExtensions
    {
        internal static bool IsInteractive(this ObjectType objectType)
        {
            var code = (int)objectType;

            return code > 1 && code < 7;
        }
    }
}
