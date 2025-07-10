
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;

namespace Nabunassar.Components.Effects
{
    internal class DissolveEffect : EffectComponent
    {
        private float _dissolveAmount = 0.5f;
        private float _dissolveSpeed = .5f;
        private bool _isDissolving = true;
        private Texture2D _noiseTexture;

        private Entity _entity;

        public DissolveEffect(NabunassarGame game, Entity entity) : base(game)
        {
            _entity = entity;
            _noiseTexture = game.Content.Load<Texture2D>("Assets/Images/Effects/dissolvemax.png");
            Effect = game.Content.Load<Effect>("Assets/Shaders/Dissolve.fx");
            Effect.Parameters["noiseTex"].SetValue(_noiseTexture);
        }

        public Action OnEnd { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (_isDissolving)
            {
                _dissolveAmount += _dissolveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _dissolveAmount = MathHelper.Clamp(_dissolveAmount, 0, 1);

                Effect.Parameters["_DissolveAmount"].SetValue(_dissolveAmount);

                if (_dissolveAmount >= 1)
                {
                    _isDissolving = false;
                    _entity.Detach<EffectComponent>();
                    OnEnd?.Invoke();
                }
            }
        }
    }

    internal static class DissolveEffectExtensions
    {
        public static void AddDissolve(this Entity entity, Action onEnd=null, NabunassarGame game=null)
        {
            var dissolve = new DissolveEffect(game ?? NabunassarGame.Game, entity);
            entity.Attach(dissolve as EffectComponent);

            if (onEnd != null)
                dissolve.OnEnd += onEnd;
        }
    }
}