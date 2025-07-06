using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Entities.Base;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Game
{
    internal class GameObject : Propertied, IClonable<GameObject>
    {
        public Entity Entity { get; set; }

        public MapObject MapObject { get; set; }

        public long ObjectId { get; set; }

        public string Name { get; set; }

        public string Cursor { get; set; }

        public string Image { get; set; }

        public bool IsTrapped { get; set; }

        public string Dialogue { get; set; }

        public ObjectType ObjectType { get; set; } = ObjectType.None;

        public GameObject Clone()
        {
            var obj = new GameObject
            {
                ObjectId = ObjectId,
                Name = Name,
                Cursor = Cursor,
                Image = Image,
                ObjectType = ObjectType,
                Dialogue = Dialogue,
            };

            return obj;
        }
    }

    internal static class GameObjectMethods
    {
        public static string GetObjectName(this GameObject obj)
        {
            if (obj == null)
                return string.Empty;

            var objectNames = NabunassarGame.Game.Strings["ObjectNames"];

            string token = null;

            if (obj.Name != null)
                token = obj.Name;

            if (token == null && obj.ObjectId > 0)
                token = obj.ObjectId.ToString();

            if (token == null)
                token = obj.ObjectType.ToString();

            return objectNames[token];
        }
    }
}