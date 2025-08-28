using MonoGame.Extended;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Components.Effects;
using Nabunassar.Entities.Base;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Items;
using Nabunassar.Entities.Data.Loot;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Resources;
using Nabunassar.Struct;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Nabunassar.Entities.Game
{
    [DebuggerDisplay("{Name}")]
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
            obj.Battler = Battler;
            obj.DangerRating = DangerRating;
            obj.Reputation = Reputation;
            obj.BattlerId = BattlerId;
            obj.LootTableId = LootTableId;

            obj.LandscapeComplexity = LandscapeComplexity.Entity(GetAbilityEntity("Landscape"));

            if (RevealComplexity != null)
                obj.RevealComplexity = RevealComplexity.Entity(GetAbilityEntity("Reveal"));

            return obj;
        }

        public void Init(NabunassarGame game)
        {
            LootTable = game.DataBase.GetById<LootTable>("Data/Objects/LootTables.json", x => x.TableId == LootTableId);
        }

        public static IEntity GetAbilityEntity(string abilityName)
        {
            var game = NabunassarGame.Game;
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

        public Battler Battler { get; set; }

        public RollResult RollResult { get; set; }

        public int BattlerId { get; set; }

        public Reputation Reputation { get; set; }

        public DangerRating DangerRating { get; set; }

        public Entity Entity { get; set; }

        public MapObject MapObject { get; set; }

        public long ObjectId { get; set; }

        /// <summary>
        /// Токен.
        /// <br/>
        /// Для получения имени вызывать <see cref="GameObjectMethods.GetObjectName(GameObject)"/>
        /// </summary>
        public string Name { get; set; }

        public string Portrait { get; set; }

        public string Cursor { get; set; }

        public string Image { get; set; }

        public bool IsTrapped { get; set; }

        public string Dialogue { get; set; }

        public ObjectType ObjectType { get; set; } = ObjectType.None;

        public List<GameObject> Dependant { get; internal set; } = new();

        public RectangleF DistanceMeterRectangle => this.MapObject.Bounds.BoundingRectangle.Multiple(2);

        public int LootTableId { get; set; }

        [JsonIgnore]
        protected LootTable LootTable { get; set; }


        protected List<Item> _items;

        public List<Item> Items(NabunassarGame game)
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
                return new Result<bool>(false, NabunassarGame.Game.Strings["GameTexts"]["NoTarget"]);

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
            var game = NabunassarGame.Game;

            var objectNames = game.Strings["ObjectNames"];

            string token = null;

            if (Name != null)
                token = Name;

            if (token == null && ObjectId > 0)
                token = ObjectId.ToString();

            if (token == null)
                token = ObjectType.ToString();

            if (token == null && ObjectType == ObjectType.Ground)
                token = GroundType.ToString();

            var name = objectNames[token].ToString();

            return name;
        }

        internal bool IsEmpty() => _items.Count == 0;

        internal string GetObjectNameTitle()
        {
            var game = NabunassarGame.Game;
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