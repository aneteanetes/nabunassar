namespace Nabunassar.Monogame.Settings
{
    public class AudioOptions
    {
        private float volume = 0.5f;
        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                if (volume > 1.0)
                {
                    volume = 1.0f;
                }
                if (volume < 0)
                {
                    volume = 0;
                }
            }
        }

        public bool Repeat { get; set; }
    }
}