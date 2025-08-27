namespace Nabunassar.Entities.Game.Calendars
{
    internal class CalendarMonths
    {
        private CalendarMonths() { }

        /// <summary>
        /// Day by indexer
        /// </summary>
        public static CalendarMonths Months = new CalendarMonths();

        public CalendarMonth this[int key]
        {
            get => CalendarMonth.Create(this, key);
        }
    }

    internal class CalendarMonth
    {
        private CalendarMonth() { }

        public int Number { get; private set; }

        public string Name { get; private set; }

        public string GodName { get; private set; }

        private static Dictionary<int, CalendarMonth> _monthsCache = new();

        public static CalendarMonth Create(CalendarMonths months, int number)
        {
            var num = number.ToString();
            var game = NabunassarGame.Game;

            var monthGod = game.DataBase.GetFromDictionary("Data/Calendar/Months.json", num);

            if (!_monthsCache.TryGetValue(number, out var month))
            {
                month = new CalendarMonth()
                {
                    Number = number,
                    Name = game.Strings["Calendar/Month"][num],
                    GodName = game.Strings["GodNames"][monthGod]
                };
            }

            return month;
        }
    }
}
