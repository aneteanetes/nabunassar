using Myra.Graphics2D.TextureAtlases;
using ioi.Entities.Data.Enums;
using ioi.Entities.Struct.ImageRegions;

namespace ioi.Entities.Game.Enums
{
    internal enum Archetype
    {
        Warrior,
        Wizard,
        Rogue,
        Priest
    }
    internal static class ArchetypeAttributes
    {
        public static (ImageRegion, string) GetInfo(this Archetype archetype, GameHost game)
        {
            string Text(Archetype type) => game.Strings["Enums/Archetypes"][type.ToString()+Sex.Male.ToString()];

            var texture = "Assets/Tilesets/transparent_packed.png";

            switch (archetype)
            {
                case Archetype.Warrior: return (new ImageRegion(512, 96, 16, 16, texture), Text(archetype));
                case Archetype.Wizard: return (new ImageRegion(448, 176, 16, 16, texture), Text(archetype));
                case Archetype.Rogue: return (new ImageRegion(512, 176, 16, 16, texture), Text(archetype));
                case Archetype.Priest: return (new ImageRegion(592, 176, 16, 16, texture), Text(archetype));
                default: return (default, null);
            }
        }
    }
}
