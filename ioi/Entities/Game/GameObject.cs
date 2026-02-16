using MonoGame.Extended;
using MonoGame.Extended.ECS;
using ioi.Components;
using ioi.Components.Effects;
using ioi.Entities.Base;
using ioi.Entities.Data.Dices;
using ioi.Entities.Data.Items;
using ioi.Entities.Data.Loot;
using ioi.Entities.Data.Rankings;
using ioi.Entities.Game.Enums;
using ioi.Resources;
using ioi.Struct;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ioi.Entities.Game
{
    [DebuggerDisplay("{Name} [{ObjectId}]")]
    internal class GameObject : Propertied, IDistanceMeter, IClonable<GameObject>
    {
        public virtual GameObject Clone(GameObject instance = null)
        {
            var obj = instance ?? new GameObject();

            obj.ObjectId = ObjectId;
            obj.Name = Name;
            obj.Cursor = Cursor;
            obj.Image = Image;
            obj.ObjectType = ObjectType;
            obj.Dialogue = Dialogue;
            obj.Portrait = Portrait;
            obj.PortraitBattle = PortraitBattle;
            obj.CreatureId = CreatureId;
            obj.Creature = Creature;
            obj.EncounterId = EncounterId;
            obj.Encounter = Encounter;
            obj.DangerRating = DangerRating;
            obj.Reputation = Reputation;
            obj.EncounterId = EncounterId;
            obj.LootTableId = LootTableId;
            obj.Properties= Properties;

            obj.LandscapeComplexity = LandscapeComplexity.Entity(GetAbilityEntity("Landscape"));

            if (RevealComplexity != null)
                obj.RevealComplexity = RevealComplexity.Entity(GetAbilityEntity("Reveal"));

            return obj;
        }

        public void Init(GameHost game)
        {
            LootTable = game.DataBase.GetById<LootTable>("Data/Objects/LootTables.json", x => x.TableId == LootTableId);
        }

        public static IEntity GetAbilityEntity(string abilityName)
        {
            var game = GameHost.Game;
            var landScapeAbilityModel = game.DataBase.GetAbility(abilityName);

            return DataBase.AddEntity(new DescribeEntity()
            {
                FormulaName = game.Strings["AbilityNames"][landScapeAbilityModel.Name] + " " + game.Strings["Entities"]["GameObject"].ToLower()
            });
        }

        #region skills

        public RankDice LandscapeComplexity { get; set; } = RankDice.BaseD4;

        public RankDice RevealComplexity { get; set; }

        #endregion

        public GroundType GroundType { get; set; }

        public Guid CreatureId { get; set; }

        public Creature Creature { get; set; }

        public RollResult RollResult { get; set; }

        public Guid EncounterId { get; set; }

        public Encounter Encounter { get; set; }

        public Reputation Reputation { get; set; }

        public DangerRating DangerRating { get; set; }

        public Entity Entity { get; set; }

        public MapObject MapObject { get; set; }

        public Guid ObjectId { get; set; }

        /// <summary>
        /// Токен.
        /// <br/>
        /// Для получения имени вызывать <see cref="GameObjectMethods.GetObjectName(GameObject)"/>
        /// </summary>
        public string Name { get; set; }

        public string Portrait { get; set; }

        public string PortraitBattle { get; set; }

        public string Cursor { get; set; }

        public string Image { get; set; }

        public bool IsTrapped { get; set; }

        public string Dialogue { get; set; }

        public ObjectType ObjectType { get; set; } = ObjectType.None;

        public List<GameObject> Dependant { get; internal set; } = new();

        public RectangleF DistanceMeterRectangle => this.MapObject.Bounds.BoundingRectangle.Multiple(2);

        public Guid LootTableId { get; set; }

        [JsonIgnore]
        protected LootTable LootTable { get; set; }


        protected List<Item> _items;

        public List<Item> Items(GameHost game)
        {
            if (_items == null)
            {
                _items = LootTable.Generate(game);
            }

            return _items;
        }

        public bool RemoveItem(Item item)
        {
            if (_items.Contains(item))
                _items.Remove(item);

            return true;
        }

        public bool AddItem(Item item)
        {
            if (!_items.Contains(item))
                _items.Add(item);

            return true;
        }

        public Result<bool> IsObjectNear(GameObject gameObject)
        {
            if (gameObject == null)
                return new Result<bool>(false, GameHost.Game.Strings["GameTexts"]["NoTarget"]);

            if (this.MapObject != default && this.MapObject.Bounds != default)
            {
                var visualBounds = this.MapObject.Bounds.BoundingRectangle.Multiple(2);
                return visualBounds.Intersects(gameObject.MapObject.Bounds.BoundingRectangle);
            }

            return false;
        }

        public void Destroy()
        {
            if (Entity != default)
            {
                MapObject?.DestroyPhysical();
                Entity.Dissolve(() =>
                {
                    if (MapObject != null)
                        MapObject.Destroy();
                });

                foreach (var depend in Dependant)
                {
                    depend.Destroy();
                }
            }
        }

        public virtual string GetObjectName()
        {
            var game = GameHost.Game;

            var objectNames = game.Strings["ObjectNames"];

            string token = null;

            if (Name != null)
                token = Name;

            if (token == null && ObjectId != default)
                token = ObjectId.ToString();

            if (token == null)
                token = ObjectType.ToString();

            if (token == null && ObjectType == ObjectType.Ground)
                token = GroundType.ToString();

            var name = objectNames[token].ToString();

            if (name == objectNames.NotFound && ObjectId != default)
            {
                return game.Strings["ObjectNames"][ObjectId.ToString()];
            }

            return name;
        }

        internal bool IsEmpty() => _items.Count == 0;

        internal string GetObjectNameTitle()
        {
            var game = GameHost.Game;
            var name = GetObjectName();

            if (this.ObjectType == ObjectType.Container && _items != null && _items.Count == 0)
            {
                name += $"{Environment.NewLine}({game.Strings["UI"]["Empty"]})";
            }

            return name;
        }

        public void Reveal()
        {
            RevealComplexity = null;
            MapObject?.Reveal();
        }
    }
}