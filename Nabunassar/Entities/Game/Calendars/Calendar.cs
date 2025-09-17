using Nabunassar.Entities.Struct;
using System;
using System.Security.Cryptography;

namespace Nabunassar.Entities.Game.Calendars
{
    internal partial class Calendar
    {
        public TimeSpan Time { get; set; } = TimeSpan.FromHours(8).Add(TimeSpan.FromMinutes(56));

        public int SavedHours { get; set; }

        public int SavedMinutes { get; set; }

        public int Months { get; set; } = 9;

        public int Days { get; set; } = 15;

        public int Year { get; set; } = 1201;

        public CalendarDay Day => CalendarDays.Days[Days];

        public CalendarMonth Month => CalendarMonths.Months[Months];

        public void Add(int hours, int minutes)
        {
            var time = Time
                .Add(TimeSpan.FromMinutes(minutes))
                .Add(TimeSpan.FromHours(hours));

            if (time.Days > 0)
            {
                time -= TimeSpan.FromDays(time.Days);
                Days++;
                OnNewDay?.Invoke();

                if (Days > 30)
                {
                    Months++;
                    OnNewMonth?.Invoke();
                    if (Months > 12)
                    {
                        Months = 1;
                        Year++;
                        OnNewYear?.Invoke();
                    }
                }
            }

            Time = time;
        }

        double _elapsedMs;

        public void Update(GameTime gameTime)
        {
            _elapsedMs += gameTime.ElapsedGameTime.TotalMilliseconds;


            if (_elapsedMs >= MsToMinutes)
            {
                Add(0, 1);
                _elapsedMs = 0;
            }
        }

        /// <summary>
        /// How much milliseconds counts for 1 minute (for in-game time)
        /// </summary>
        public int MsToMinutes = 2000; //2 sec = 1 min

        public Calendar Save()
        {
            return new Calendar()
            {
                Days = Days,
                Months = Months,
                Year = Year,
                SavedHours = Time.Hours,
                SavedMinutes = Time.Minutes,
            };
        }

        public void Restore(Calendar calendar)
        {
            this.Days = calendar.Days;
            this.Months = calendar.Months;
            this.Year = calendar.Year;

            this.Time = TimeSpan.FromHours(calendar.SavedHours).Add(TimeSpan.FromMinutes(calendar.SavedMinutes));
        }

        public string ToTimeString()
        {
            return $"{Time.Hours:00}:{Time.Minutes:00}";
        }

        public string ToFullString(int sumerianFontSize = 40)
        {
            var day = Day;
            var month = Month;

            var text = DrawText.Create($"{Time.Hours:00}:{Time.Minutes:00} ")
                .Font(Fonts.Sumerian, sumerianFontSize)
                .Append(day.Icon)
                .ResetFont()
                .Append($":{day.Number}({day.NameLocalized}), {month.Name}, {Year}.");

            return text.ToString();
        }
        // 8:15 Пн, Месяц Капли, 1201г.

        public event Action OnNewDay;
        public event Action OnNewMonth;
        public event Action OnNewYear;
    }
}