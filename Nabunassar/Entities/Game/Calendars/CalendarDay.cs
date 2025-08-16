using Geranium.Reflection;
using System.Text.RegularExpressions;

namespace Nabunassar.Entities.Game.Calendars
{
    internal class CalendarDays
    {
        private CalendarDays() { }

        /// <summary>
        /// Day by indexer
        /// </summary>
        public static CalendarDays Days = new CalendarDays();

        public CalendarDay this[int key]
        {
            get => CalendarDay.Create(this, key);
        }
    }

    internal partial class CalendarDay
    {
        private CalendarDay() { }

        public int Number { get; private set; }

        public string Icon { get; private set; }

        public string NameLocalized { get; private set; }

        private static Dictionary<int, CalendarDay> _daysCache = new();

        public static CalendarDay Create(CalendarDays days, int number)
        {
            var game = NabunassarGame.Game;
            var dayModel = game.DataBase.GetFromDictionary<InternalCalendarDayModel>("Data/Calendar/Days.json", number.ToString());

            if (!_daysCache.TryGetValue(number, out var day))
            {
                day = new CalendarDay() { Number = number, Icon = dayModel.Icon };
                day.NameLocalized = ParseDayName(dayModel.Name);
                _daysCache[number] = day;
            }

            return day;
        }

        private static string ParseDayName(string name)
        {
            string localized = "";

            var tokens = CapitalizeSplitRegex().Split(name);
            foreach (var token in tokens)
            {
                if (localized.IsNotEmpty())
                    localized += "-";
                localized += NabunassarGame.Game.Strings["Calendar/Days"][token];
            }

            return localized;
        }

        private class InternalCalendarDayModel
        {
            public string Icon { get; set; }

            public string Name { get; set; }
        }

        [GeneratedRegex(@"(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])")]
        private static partial Regex CapitalizeSplitRegex();
    }
}