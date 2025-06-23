using Nabunassar.Entities.Game;
using Nabunassar.Struct;

namespace Nabunassar.Resources
{
    internal class DataBase
    {
        NabunassarGame _game;

        public DataBase(NabunassarGame game)
        {
            _game = game;
        }

        public float GetGroundTypeSpeed(GroundType type)
        {
            var data = Get<Dictionary<GroundType, float>>("Data/Map/GroundTypeSpeed.json");
            return data[type];
        }


        private List<GameObject> _objects;

        public GameObject GetObject(ObjectType objectType)
        {
            var obj = GetObjectInternal(x=>x.ObjectType==objectType);

            if (obj != null)
                obj.ObjectId = _game.Random.Next(-10000, -10);

            return obj;
        }

        public GameObject GetObject(long objectId)
        {
            var obj = GetObjectInternal(x=>x.ObjectId==objectId);
            return obj;
        }

        private GameObject GetObjectInternal(Func<GameObject,bool> selector)
        {
            if (_objects == default)
            {
                _objects = Get<List<GameObject>>("Data/Objects/ObjectRegistry.json");
            }

            var @object = _objects.FirstOrDefault(selector);
            if (@object == default)
                return default;

            var newObject = @object.Clone();

            return newObject;
        }

        public T Get<T>(string assetName)
        {
            return _game.Content.Load<T>(assetName);
        }
    }
}
