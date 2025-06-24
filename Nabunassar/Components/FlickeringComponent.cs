using Nabunassar.Entities.Game;

namespace Nabunassar.Components
{
    internal class FlickeringComponent
    {
        public FlickeringComponent(double amplitude, double period, int steps, double from=1, double to=0)
        {
            Amplitude = amplitude;
            Period = period;
            Steps = steps;
            FromValue = from;
            ToValue = to;
        }

        public GameObject GameObject { get; set; }

        public double FromValue { get; set; }

        public double ToValue { get; set; }

        public bool IsActive { get; set; }

        public int CurrentStep { get; set; }

        public double Amplitude { get; set; }

        public double Period { get; set; }

        public int Steps { get; set; }

        public Action<double> OnChange { get; set; }
    }
}
