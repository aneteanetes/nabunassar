using ioi.Components;
using SadConsole.UI;

namespace ioi.Entities.Map
{
    internal class RogueMap
    {
        public Dictionary<Vector2,List<ObjectMap>> ObjectMap { get; set; }

        public IEnumerable<ObjectMap> Objects => ObjectMap.SelectMany(x => x.Value);

        public void Add(ObjectMap obj)
        {
            var key = obj.KeyCoords();
            if (!ObjectMap.ContainsKey(key))
                ObjectMap[key] = [];
            ObjectMap[key].Add(obj);
        }

        public List<ObjectMap> Collide(Vector2 coords)
        {
            if (!ObjectMap.TryGetValue(coords, out List<ObjectMap> value))
                return [];

            return value;
        }

        internal void Move(ObjectMap obj, Vector2 coords)
        {
            ObjectMap[obj.Coords].Remove(obj);

            obj.Coords = coords;
            obj.RecalculatePositionFromCoords();

            if(!ObjectMap.ContainsKey(coords))
            {
                ObjectMap[coords] = [];
            }
            ObjectMap[obj.Coords].Add(obj);
        }
    }
}
