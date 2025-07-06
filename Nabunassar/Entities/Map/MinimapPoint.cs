using Nabunassar.Struct;

namespace Nabunassar.Entities.Map
{
    internal class MinimapPoint
    {
        public int EntityId { get; set; }

        public Vector2 Position { get; set; }

        public ObjectType ObjectType { get; set; }

        public GroundType GroundType { get; set; }

        public string Name { get; set; }
    }
}
