﻿using MonoGame.Extended.ECS;
using Nabunassar.Components;
using Nabunassar.Entities.Base;
using Nabunassar.Entities.Data.Dices;
using Nabunassar.Entities.Data.Rankings;
using Nabunassar.Entities.Game.Enums;
using Nabunassar.Struct;

namespace Nabunassar.Entities.Game
{
    internal class GameObject : Propertied, IClonable<GameObject>
    {
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
                Portrait = Portrait,
                Battler = Battler,
                DangerRating = DangerRating,
                Reputation = Reputation,
                BattlerId = BattlerId
            };

            return obj;
        }

        public Battler Battler { get; set; }

        public Rank LandscapeRank { get; set; } = Rank.Basic;

        public Dice LandscapeDice { get; set; } = Dice.d4;

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

        public void Destroy()
        {
            if (MapObject != null)
                MapObject.Destroy();
        }
    }

    internal static class GameObjectMethods
    {
        public static string GetObjectName(this GameObject obj)
        {
            if (obj == null)
                return string.Empty;

            var game = NabunassarGame.Game;

            var objectNames = game.Strings["ObjectNames"];

            string token = null;

            if (obj.Name != null)
                token = obj.Name;

            if (token == null && obj.ObjectId > 0)
                token = obj.ObjectId.ToString();

            if (token == null)
                token = obj.ObjectType.ToString();

            if (obj.ObjectType == ObjectType.Ground)
                token = obj.GetPropertyValue<GroundType>(nameof(GroundType)).ToString();

            return objectNames[token];
        }
    }
}