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
                _value = _featureValue.Value;
                _featureValue = null;
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

        public static implicit operator T(GameLoopFeatureValue<T> loopValue) => loopValue.Value;
    }
}
