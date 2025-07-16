using Geranium.Reflection;
using Nabunassar.Entities;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;

namespace Nabunassar.Resources
{
    internal class DataBase
    {
        public const string NotFoundStringConstant = "[String Was Not Found]";

        NabunassarGame _game;

        public DataBase(NabunassarGame game)
        {
            _game = game;
        }

        private Dictionary<Guid, IEntity> Entities = new();

        public IEntity AddEntity(IEntity entity)
        {
            return Entities[entity.ObjectId] = entity;
        }

        public IEntity GetEntity(Guid id)
        {
            if(Entities.ContainsKey(id)) 
                return Entities[id];

            return default;
        }

        public float GetGroundTypeSpeed(GroundType type)
        {
            var data = Get<Dictionary<GroundType, float>>("Data/Map/GroundTypeSpeed.json");
            return data[type];
        }

        public string GetFromDictionary(string file, string key)
        {
            var data = Get<Dictionary<string, string>>(file);
            return data[key];
        }

        public TValue GetFromDictionary<TValue>(string file, string key)
        {
            var data = Get<Dictionary<string, TValue>>(file);
            return data[key];
        }

        public string GetString(string file, string @string)
        {
            if (@string == null)
                return NotFoundStringConstant;

            var code = _game.Settings.LanguageCode ?? "ru-RU";

            var data = Get<Dictionary<string, string>>($"Data/Localization/{code}/{file}.json");

            if(data.TryGetValue(@string, out var value))
            {
                return value;
            }

            return NotFoundStringConstant;
        }

        private List<GameObject> _objects;

        public GameObject GetObject(ObjectType objectType)
        {
            var obj = GetObjectInternal(x=>x.ObjectType==objectType);
            if (obj == null)
            {
                obj = GetObjectInternal(x => x.Name == x.ObjectType.ObjectTypeInLoadedMap());

                if (obj == null)
                    obj = new GameObject()
                    {
                        ObjectType = objectType
                    };
            }

            if (obj != null)
                obj.ObjectId = _game.Randoms.Next(-10000, -10);

            return obj;
        }

        public GameObject GetObjectGround(GroundType groundType)
        {
            var groundName = groundType + _game.GameState.LoadedMapPostFix;
            var obj = GetObjectInternal(x => x.Name == groundName);
            obj.ObjectType = ObjectType.Ground;
            obj.GroundType = groundType;
            if (obj == null)
            {
                obj = new GameObject()
                {
                    ObjectType = ObjectType.Ground
                };
            }

            if (obj != null)
                obj.ObjectId = _game.Randoms.Next(-10000, -10);

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

            if (@object.BattlerId != 0)
            {
                var battler = Get<List<Battler>>("Data/Battlers/BattlerRegistry.json").FirstOrDefault(x => x.BattlerId == @object.BattlerId);
                if (battler != default)
                    @object.Battler = battler;
            }

            var newObject = @object.Clone();

            return newObject;
        }

        public T Get<T>(string assetName)
        {
            return _game.Content.Load<T>(assetName);
        }

        internal AbilityModel GetAbility(string abilityName)
        {
            var abilities = _game.Content.Load<List<AbilityModel>>("Data/Abilities/AbilityRegistry.json");
            return abilities.FirstOrDefault(x=>x.Name== abilityName);
        }
    }
}
