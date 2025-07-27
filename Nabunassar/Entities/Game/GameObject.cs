using Geranium.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Components.Effects;
using Nabunassar.Entities.Base;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Loot;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Entities.Struct.ImageRegions;
using Nabunassar.Struct;
using Newtonsoft.Json;

namespace Nabunassar.Entities.Game
{
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

            var landscape = GetLandscapeAbility();

            obj.LandscapeDice = LandscapeDice.Entity(landscape);
            obj.LandscapeRank = LandscapeRank.Entity(landscape);

            

            return obj;
        }

        public void Init(NabunassarGame game)
        {
            LootTable = game.DataBase.GetById<LootTable>("Data/Objects/LootTables.json", x => x.TableId == LootTableId);
        }

        private IEntity GetLandscapeAbility()
        {
            var game = NabunassarGame.Game;
            var landScapeAbilityModel = game.DataBase.GetAbility("Landscape");

            return game.DataBase.AddEntity(new DescribeEntity()
            {
                FormulaName = game.Strings["AbilityNames"][landScapeAbilityModel.Name] + " " + game.Strings["Entities"]["GameObject"].ToLower()
            });
        }

        public GroundType GroundType { get; set; }

        public Battler Battler { get; set; }

        public Rank LandscapeRank { get; set; } = Rank.Basic;

        public Dice LandscapeDice { get; set; } = Dice.d4;

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

            return objectNames[token];
        }
    }
}