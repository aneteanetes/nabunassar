using Nabunassar.Entities.Data.Enums;

namespace Nabunassar.Entities.Data.Praying
{
    internal class PrayState
    {
        public PrayState()
        {
            typeof(Gods).GetAllValues<Gods>().ForEach(x =>
            {
                PrayTracks[x] = new PrayTrack()
                {
                    God = x
                };
            });
        }

        public PrayResult Add(int prayValue)
        {
            PrayerCounter += prayValue;

            if (PrayerCounter == 100)
            {
                PrayerCounter = 0;
                return PrayResult.Equal100AndReseted;
            }

            if (PrayerCounter > 100)
            {
                PrayerCounter -= 100;
                return PrayResult.Overflowed;
            }

            return PrayResult.Added;
        }

        public int PrayerCounter { get; private set; } = 0;

        public Dictionary<Gods, PrayTrack> PrayTracks { get; set; } = new();
    }

    internal enum PrayResult
    {
        Added,
        Overflowed,
        Equal100AndReseted
    }
}
