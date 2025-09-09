
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;

namespace Nabunassar.Components.Effects
{
    internal class LineJitterShaderEffect : ShaderEffectComponent
    {
        private float _seconds;
        private TimeSpan _time;
        private Entity _entity;

        public override bool IsSeparateTexture => true;

        public LineJitterShaderEffect(NabunassarGame game, Entity entity, TimeSpan time) : base(game)
        {
            _time = time;
            _entity = entity;
            Effect = game.Content.Load<Effect>("Assets/Shaders/LineJitter.fx");
            Effect.Parameters["shaking"].SetValue(false);
            Effect.Parameters["amplitude"].SetValue(1f);
            Effect.Parameters["noise_scale"].SetValue(20f);
            Effect.Parameters["noise_speed"].SetValue(20f);
            Effect.Parameters["samplerWidth"].SetValue(2560f);
            Effect.Parameters["samplerHeight"].SetValue(1440);

            _seconds = 0f;

        }

        public Action OnEnd { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (this.CanUpdate(gameTime, TimeSpan.FromMicroseconds(20)))
            {
                _seconds++;
                if (_seconds >= 60)
                    _seconds = 0;
            }
            Effect.Parameters["timeSeconds"].SetValue(_seconds);

            if (this.CanUpdate(gameTime, _time))
            {
                _entity.Detach<ShaderEffectComponent>();
                OnEnd?.Invoke();
            }
        }
    }

    internal static class LineJitterEffectExtensions
    {
        public static void LineJitter(this Entity entity, TimeSpan time, Action onEnd = null, NabunassarGame game = null)
        {
            var tpEffect = new LineJitterShaderEffect(game ?? NabunassarGame.Game, entity, time);
            entity.Attach(tpEffect as ShaderEffectComponent);

            if (onEnd != null)
                tpEffect.OnEnd += onEnd;
        }
    }
}