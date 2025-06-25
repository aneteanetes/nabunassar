using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Game
{
    internal class GameObject : IClonable<GameObject>
    {
        public Entity Entity { get; set; }

        public MapObject MapObject { get; set; }

        public long ObjectId { get; set; }

        public string Name { get; set; }

        public string Cursor { get; set; }

        public string Image { get; set; }

        public ObjectType ObjectType { get; set; } = ObjectType.None;

        public GameObject Clone()
        {
            var obj = new GameObject
            {
                ObjectId = ObjectId,
                Name = Name,
                Cursor = Cursor,
                Image = Image,
                ObjectType = ObjectType
            };

            return obj;
        }
    }
}