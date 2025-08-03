using Nabunassar.Entities.Data.Enums;
using Nabunassar.Entities.Struct.ImageRegions;

namespace Nabunassar.Entities.Game.Enums
{
    internal enum Archetype
    {
        Warrior,
        Wizzard,
        Rogue,
        Priest
    }
    internal static class ArchetypeAttributes
    {
        public static (ImageRegion, string) GetInfo(this Archetype itemType, NabunassarGame game)
        {
            string Text(Archetype type) => game.Strings["Enums/Archetypes"][type.ToString()+Sex.Male.ToString()];

            var texture = "Assets/Tilesets/transparent_packed.png";

            switch (itemType)
            {
                case Archetype.Warrior: return (new ImageRegion(512, 96, 16, 16, texture), Text(itemType));
                case Archetype.Wizzard: return (new ImageRegion(448, 176, 16, 16, texture), Text(itemType));
                case Archetype.Rogue: return (new ImageRegion(512, 176, 16, 16, texture), Text(itemType));
                case Archetype.Priest: return (new ImageRegion(592, 176, 16, 16, texture), Text(itemType));
                default: return (default, null);
            }
        }
    }
}
