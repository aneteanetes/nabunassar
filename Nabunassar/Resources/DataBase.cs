using Geranium.Reflection;
using Nabunassar.Entities;
using Nabunassar.Entities.Data;
using Nabunassar.Entities.Data.Abilities;
using Nabunassar.Entities.Data.Enums;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Game;
using Nabunassar.Struct;
using System.IO;

namespace Nabunassar.Resources
{
    internal class DataBase
    {
        public const string NotFoundStringConstant = "[String Was Not Found]";

        NabunassarGame Game;

        public DataBase(NabunassarGame game)
        {
            Game = game;
        }

        private static Dictionary<Guid, IEntity> Entities = new();

        public static IEntity AddEntity(IEntity entity)
        {
            return Entities[entity.ObjectId] = entity;
        }

        public static IEntity GetEntity(Guid id)
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

            var data = GetLocalized<Dictionary<string, string>>(file);

            if (data.TryGetValue(@string, out var value))
            {
                return value;
            }

            if (data.TryGetValue(@string.ToLowerInvariant(), out value))
            {
                return value;
            }

            return NotFoundStringConstant;
        }

        public T GetLocalized<T>(string file)
        {
            var code = Game.Settings.LanguageCode ?? "ru-RU";
            return Get<T>($"Data/Localization/{code}/{file}.json");
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
                obj.ObjectId = Game.Random.Next(-10000, -10);

            return obj;
        }

        public GameObject GetObjectGround(GroundType groundType)
        {
            var groundName = groundType + Game.GameState.LoadedMapPostFix;
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
                obj.ObjectId = Game.Random.Next(-10000, -10);

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
            newObject.Init(Game);

            return newObject;
        }

        public T Get<T>(string assetName)
        {
            return Game.Content.Load<T>(assetName);
        }

        public T GetById<T>(string assetName, Func<T, bool> idSelector)
        {
            return Game.Content.Load<List<T>>(assetName).FirstOrDefault(idSelector);
        }

        internal AbilityModel GetAbility(string abilityName)
        {
            var abilities = Game.Content.Load<List<AbilityModel>>("Data/Abilities/AbilityRegistry.json");
            return abilities.FirstOrDefault(x=>x.Name== abilityName);
        }

        internal Item GetItem(int itemId)
        {
            var item = GetById<Item>("Data/Objects/ItemsRegistry.json", x => x.ObjectId == itemId);

            return item.Clone();
        }

        internal Party CreateRandomParty(NabunassarGame game)
        {
            var party = new Party(game);

            party.First = new Hero(game, Nabunassar.Entities.Game.Enums.Archetype.Warrior)
            {
                Tileset = "warrior.png",
                Sex = Sex.Male,
                Name = GetName(Sex.Male)
            };
            party.First.Creature = new Creature( Nabunassar.Entities.Game.Enums.Archetype.Warrior, party.First)
            {
                Archetype = Nabunassar.Entities.Game.Enums.Archetype.Warrior
            };

            party.Second = new Hero(game, Nabunassar.Entities.Game.Enums.Archetype.Rogue)
            {
                Tileset = "rogue.png",
                Sex = Sex.Female,
                Name = GetName(Sex.Female),
            };
            party.Second.Creature = new Creature(Nabunassar.Entities.Game.Enums.Archetype.Rogue,party.Second)
            {
                Archetype = Nabunassar.Entities.Game.Enums.Archetype.Rogue
            };

            party.Third = new Hero(game, Nabunassar.Entities.Game.Enums.Archetype.Wizard)
            {
                Tileset = "wizard.png",
                Sex = Sex.Male,
                Name = GetName(Sex.Male)
            };
            party.Third.Creature = new Creature(Nabunassar.Entities.Game.Enums.Archetype.Wizard, party.Third)
            {
                Archetype = Nabunassar.Entities.Game.Enums.Archetype.Wizard
            };

            party.Fourth = new Hero(game, Nabunassar.Entities.Game.Enums.Archetype.Priest)
            {
                Tileset = "priest.png",
                Sex = Sex.Female,
                Name = GetName(Sex.Female)
            };
            party.Fourth.Creature = new Creature(Nabunassar.Entities.Game.Enums.Archetype.Priest, party.Fourth)
            {
                Archetype = Nabunassar.Entities.Game.Enums.Archetype.Priest
            };

            return party;
        }

        internal string GetName(Sex sex, int idx = -1)
        {
            var names = GetLocalized<string[]>("Names/"+sex.ToString());

            if (idx < 0)
            {
                idx = Game.Random.Next(0,names.Length-1);
            }

            return names[idx];
        }
    }
}
