using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Nabunassar.Components.Effects
{
    internal class CooldownShaderEffect : ShaderEffectComponent
    {
        private Color _maskColor;
        private TimeSpan? _elapsed;
        private TimeSpan _cooldown;
        private float _fillPercent;
        private bool _isSeparateTExture;

        public override bool IsSeparateTexture => _isSeparateTExture;

        /// <summary>
        /// Call <see cref="Run"/> for start
        /// </summary>
        /// <param name="game"></param>
        /// <param name="cooldown"></param>
        /// <param name="maskColor"></param>
        /// <param name="isSeparateTExture"></param>
        public CooldownShaderEffect(NabunassarGame game, TimeSpan cooldown, Color maskColor = default, bool isSeparateTExture = false) : base(game)
        {
            if (maskColor == default)
                maskColor = Color.Black.SetAlpha(.5f);
            _maskColor = maskColor;

            _cooldown = cooldown;

            _isSeparateTExture = isSeparateTExture;
            Effect = game.Content.Load<Effect>("Assets/Shaders/CooldownMask.fx");
        }

        public float Percent
        {
            get => _fillPercent;
            set => _fillPercent = Math.Clamp(value, 0f, 100f);
        }

        public TimeSpan Cooldown
        {
            get => _cooldown;
            set
            {
                //reset current cooldown
                _elapsed = null;
                _fillPercent = 0;
            }
        }

        public void Run()
        {
            _elapsed = _cooldown;
            CalculateFillPercent();
        }

        public override void Update(GameTime gameTime)
        {
            if (_elapsed == null)
                return;

            _elapsed -= gameTime.ElapsedGameTime;

            CalculateFillPercent();
        }

        private void CalculateFillPercent()
        {
            _fillPercent = (float)(_elapsed / _cooldown * 100);
        }

        public override void Draw(GameTime gameTime, Sprite sprite, Vector2 position, float rotation = 0, Vector2 scale = default, bool isWithEffect = true)
        {
            if (_fillPercent == 0)
            {
                base.Draw(gameTime, sprite, position, rotation, scale, false);
                _elapsed = null;
                return;
            }
            else
            {
                Effect.Parameters["maskColor"].SetValue(_maskColor.ToVector4());
                Effect.Parameters["fillPercent"].SetValue(_fillPercent);

                base.Draw(gameTime, sprite, position, rotation, scale);
            }
        }
    }
}