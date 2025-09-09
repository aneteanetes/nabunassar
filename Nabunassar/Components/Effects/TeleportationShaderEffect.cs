
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using System.Reflection;

namespace Nabunassar.Components.Effects
{
    internal class TeleportationShaderEffect : ShaderEffectComponent
    {
        private float _progress = 0f;
        private float _beamSize = .1f;
        private bool _isRunning = true;

        private Entity _entity;

        public override bool IsSeparateTexture => true;

        public TeleportationShaderEffect(NabunassarGame game, bool isAssembly=false, Entity entity=null) : base(game)
        {
            _entity = entity;
            Effect = game.Content.Load<Effect>($"Assets/Shaders/Teleportation{(isAssembly ? "" : "Reversed")}.fx");
            Effect.Parameters["color"].SetValue("#00ffff".AsColor().ToVector4());
            Effect.Parameters["noise_desnity"].SetValue(600);
            Effect.Parameters["beam_size"].SetValue(_beamSize);
        }

        public Action OnEnd { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (_isRunning && this.CanUpdate(gameTime, TimeSpan.FromMilliseconds(20)))
            {
                _progress += 0.01f;
                Effect.Parameters["progress"].SetValue(_progress);

                if (_progress >= 1)
                {
                    _isRunning = false;
                    _entity?.Detach<ShaderEffectComponent>();
                    OnEnd?.Invoke();
                }
            }
        }
    }

    internal static class TeleportationEffectExtensions
    {
        public static void Teleportation(this Entity entity, Action onEnd = null, NabunassarGame game = null)
        {
            var tpEffect = new TeleportationShaderEffect(game ?? NabunassarGame.Game,false, entity);
            entity.Attach(tpEffect as ShaderEffectComponent);

            if (onEnd != null)
                tpEffect.OnEnd += onEnd;
        }
    }
}