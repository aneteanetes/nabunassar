using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Struct
{
    internal class GameLoopFeatureValue<T> : IFeatured
        where T : struct
    {
        public GameLoopFeatureValue(NabunassarGame game, T initialValue)
        {
            game.FeatureValues.Add(this);
            _value = initialValue;
        }

        public void Update(GameTime gameTime)
        {
            if (_featureValue.HasValue)
            {
                SetValue(_featureValue.Value);
            }
        }

        private T _value;

        private T? _featureValue { get; set; }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _featureValue = value;
            }
        }

        /// <summary>
        /// Setting value on this loop
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value)
        {
            _value = value;
            _featureValue = null;
        }

        public static implicit operator T(GameLoopFeatureValue<T> loopValue) => loopValue.Value;
    }
}
