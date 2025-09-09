
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ECS;

namespace Nabunassar.Components.Effects
{
    internal class WavesShaderEffect : ShaderEffectComponent
    {
        private float _seconds = 0f;
        private TimeSpan _effectTime;
        private Color _color;
        private Entity _entity;

        public override bool IsSeparateTexture => true;

        public WavesShaderEffect(NabunassarGame game, Size size, float amplitude = .07f, float frequency = 3f, Entity entity = null, TimeSpan effectTime = default) : base(game)
        {
            _entity = entity;
            _effectTime = effectTime;
            Effect = game.Content.Load<Effect>("Assets/Shaders/TeleportationWaves.fx");
            Effect.Parameters["TextureSize"].SetValue(new Vector2(size.Width, size.Height));
            Effect.Parameters["base_amplitude"].SetValue(amplitude);
            Effect.Parameters["base_frequency"].SetValue(frequency);

            SetColor();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"><see cref="Color.LightSeaGreen"/></param>
        public void SetColor(Color color = default)
        {
            if (color == default)
                color = Color.LightSeaGreen;

            _color = color;

            Effect.Parameters["targetColor"].SetValue(color.ToVector4());
        }

        public Color GetColor() { return _color; }

        public Action OnEnd { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (this.CanUpdate(gameTime, TimeSpan.FromMilliseconds(20)))
            {
                _seconds++;
                if (_seconds >= 600)
                    _seconds = 0;
            }
            Effect.Parameters["Time"].SetValue(_seconds);

            if (_effectTime != default && this.CanUpdate(gameTime, _effectTime))
            {
                _entity?.Detach<ShaderEffectComponent>();
                OnEnd?.Invoke();
            }
        }
    }
}