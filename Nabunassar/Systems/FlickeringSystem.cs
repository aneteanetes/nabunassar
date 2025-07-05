using MonoGame.Extended.ECS;
using Nabunassar.Components;

namespace Nabunassar.Systems
{
    internal class FlickeringSystem : BaseSystem
    {
        private ComponentMapper<FlickeringComponent> _flickeringMapper;


        public FlickeringSystem(NabunassarGame game) : base(game, Aspect.One(typeof(FlickeringComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _flickeringMapper = mapperService.GetMapper<FlickeringComponent>();
        }

        public override void Update(GameTime gameTime, bool sys)
        {
            if (!this.IsUpdateAvailable(gameTime, 15))
                return;

            foreach (var entity in ActiveEntities)
            {
                var flicker = _flickeringMapper.Get(entity);
                if (!flicker.IsActive)
                    continue;

                if (flicker.CurrentStep >= flicker.Steps)
                    flicker.CurrentStep = 0;

                double time = (double)flicker.CurrentStep / flicker.Steps * flicker.Period;
                double value = CustomSineWave(time, flicker.Period, /*flicker.Amplitude,*/ flicker.FromValue, flicker.ToValue);

                flicker.OnChange?.Invoke(value);

                flicker.CurrentStep++;
            }
        }


        /// <summary>
        /// Генерирует значение синусоиды в диапазоне [minValue, maxValue]
        /// </summary>
        /// <param name="time">Текущее время</param>
        /// <param name="period">Период полного цикла</param>
        /// <param name="minValue">Нижнее значение</param>
        /// <param name="maxValue">Верхнее значение</param>
        /// <returns>Значение волны в заданный момент времени</returns>
        static double CustomSineWave(double time, double period, double minValue, double maxValue)
        {
            // Нормализованный угол [0, 2π]
            double angle = 2 * Math.PI * time / period;

            // Базовое значение в диапазоне [0, 1]
            double normalized = (1 - Math.Cos(angle)) / 2;

            // Масштабирование в нужный диапазон
            return minValue + normalized * (maxValue - minValue);
        }
    }
}
