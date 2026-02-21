using ioi.Components;
using ioi.Entities.Data.Abilities.WorldAbilities;
using ioi.Entities.Data.Dices;
using ioi.Entities.Data.Effects;
using ioi.Entities.Data.Locations;
using ioi.Entities.Data.Praying;
using ioi.Entities.Data.Temporary;
using ioi.Entities.Game;
using ioi.Entities.Game.Calendars;
using ioi.Entities.Map;
using ioi.Entities.Struct;
using ioi.Struct;
using ioi.Widgets.UserInterfaces;
using Monogame.Extended;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioi.Entities.Data
{
    internal class GameState
    {
        /// <summary>
        /// For visuals multiply on NabunassarGame.Camera.Zoom
        /// </summary>
        public int GameMeterMeasure { get; private set; } = 16; //px

        public bool IsActive { get; set; } = false;

        public Party Party { get; set; }

        public Cursor Cursor { get; set; } = new();

        public Action<string> OnLog { get; set; }

        public Minimap Minimap { get; set; }

        public Location Location { get; set; }

        public UIState UIState { get; set; } = new();

        public Calendar Calendar { get; set; } = new();

        public TypedHashSet<BaseEffect> PartyEffects { get; set; } = new();

        public PrayState Prayers { get; set; } = new PrayState();

        public string LoadedMapPostFix => Location.LoadedMap.GetPropertyValue<string>("AreaObjectPostfix");

        public RogueMap Map { get; set; }

        public bool EscapeSwitch { get; internal set; }

        public bool InGame { get; set; }

        public GameEntity Player { get; internal set; }

        [NotMapped]
        public TemporaryState Temp { get; set; } = new();

        public void Log(string message)
        {
            OnLog?.Invoke(message);
        }

        public void AddMessage(DrawText text)
        {
            if (ChatWindow.Exists)
                ChatWindow.AddMessage(text.ToString());
        }

        public void AddRollMessage(DrawText text, RollResultComplexity rollResult)
        {
            if (ChatWindow.Exists)
                ChatWindow.AddRollMessage(text.ToString(), rollResult);
        }

        internal void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            Calendar.Update(gameTime);
        }

        public void Init()
        {
            Calendar.OnNewDay += ResetPrayers;
        }

        private void ResetPrayers()
        {
            var priests = Party.Where(x=>x.Creature.Archetype== Game.Enums.Archetype.Priest).ToList();
            foreach (var priest in priests)
            {
                var prayerAbil = priest.Creature.WorldAbilities.FirstOrDefault(x => x.GetType() == typeof(PrayerAbility));

                if (prayerAbil.AbilityRank.Value >= 3)
                    priest.Creature.IsPrayerAvailable = true;
            }
        }
    }
}