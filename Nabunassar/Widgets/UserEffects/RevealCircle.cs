using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using Myra.Graphics2D.UI;
using Nabunassar.Components;
using Nabunassar.Entities.Data.Abilities.WorldAbilities;
using Nabunassar.Monogame.Extended;
using Nabunassar.Shaders;
using Nabunassar.Widgets.Base;
using ParticleEffect = MonoGame.Extended.Particles.ParticleEffect;

namespace Nabunassar.Widgets.UserEffects
{
    internal class RevealCircle : ScreenWidget
    {
        private Texture2D _particleTexture;
        private ParticleEffect _particleEffect;
        private RevealAbility _revealAbility;
        private GrayscaleMapShader _postProcessor;

        public override bool IsModal => true;

        public RevealCircle(NabunassarGame game, RevealAbility ability) : base(game)
        {
            _revealAbility = ability;
        }

        private static float _radius = 50;
        private static int _capacity = 500;
        private Profile.CircleRadiation _radiation = Profile.CircleRadiation.Out;
        private float _gravityStrength = 5000;
        private Range<float> _scale = new Range<float>(1f, 1.9f);
        private Range<float> _scaleForGravity = new Range<float>(0.2f, .7f);
        private CircleF _revealArea = default;

        public override void LoadContent()
        {
            _particleTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            _particleTexture.SetData(new[] { Color.White });

            _postProcessor = new GrayscaleMapShader(Game, () => new Rectangle(_circlePos, new Size(100, 100)), () => _dissapearingTimer);
            _postProcessor.Enable();

            _revealArea = new CircleF(Vector2.Zero, 50);

            var textureRegion = new Texture2DRegion(_particleTexture);
            _particleEffect = new ParticleEffect()
            {
                Emitters =
                [
                    new ParticleEmitter(textureRegion, _capacity, TimeSpan.FromMilliseconds(50),
                        Profile.Ring(_radius, _radiation))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(1f,10f),
                            Quantity = 5,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = _scaleForGravity
                        },
                        Modifiers =
                        {
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = _gravityStrength},
                        }
                    },
                     new ParticleEmitter(textureRegion, 500, TimeSpan.FromMilliseconds(50),
                        Profile.Ring(_radius, _radiation))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f,5f),
                            Quantity = 50,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = _scale
                        }
                    }
                ]
            };

            var partyOrigin = Game.GameState.Party.GetOrigin();

            partyOrigin = Game.Camera.WorldToScreen(partyOrigin);

            Mouse.SetPosition(((int)partyOrigin.X), ((int)partyOrigin.Y));
        }

        protected override Widget CreateWidget()
        {
            return new Panel() { };
        }

        public override void Dispose()
        {
            _particleTexture.Dispose();
            _particleEffect.Dispose();
            Game.IsMouseVisible = true;
            base.Dispose();
        }

        private bool _isDissapearing = false;
        private float _dissapearingTimer = 0;
        private MouseStateExtended _mouse;
        private Vector2 _mousepos;

        public override void Update(GameTime gameTime)
        {
            if (_isDissapearing)
            {
                _dissapearingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_dissapearingTimer >= 1)
                {
                    this.Close();
                    return;
                }
            }

            _mouse = MouseExtended.GetState();

            _mousepos = _mouse.Position.ToVector2();
            BoundMousePos();
            _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            _revealArea.Position = Game.Camera.ScreenToWorld(_mousepos);

            if (!_isDissapearing)
            {
                _particleEffect.Position = Game.Camera.ScreenToWorld(_mousepos);

                if (_mouse.WasButtonPressed(MouseButton.Left))
                {
                    CastDissapear();
                }
            }

            if (KeyboardExtended.GetState().WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                this.Close();
            }


            if (!_isDissapearing)
            {
                var pos = Game.Camera.ScreenToWorld(_mousepos).ToPoint();
                pos.X = pos.X - 50;
                pos.Y = pos.Y - 50;

                _circlePos = pos;
            }
        }

        private void CastDissapear()
        {
            _particleEffect.Emitters.Remove(_particleEffect.Emitters.FirstOrDefault());
            foreach (var emmiter in _particleEffect.Emitters)
            {
                emmiter.LifeSpan = TimeSpan.FromSeconds(1);
            }

            _particleEffect.Trigger(_mousepos);
            _isDissapearing = true;

            var objs = Game.QuerySpace(CollisionLayers.Hidden, _revealArea)
                .Select(x => x as MapObject)
                .GroupBy(x => x.GameObject)
                .Select(x => x.Key)
                .Where(x => x.RevealComplexity != null)
                .ToList();

            _revealAbility.CastReveal(objs);
        }

        private void BoundMousePos()
        {
            var pos = Game.Camera.ScreenToWorld(_mousepos);
            if (!Game.GameState.Party.RevealArea.InBounds(pos, out var newPos))
            {
                var newScreenPosition = Game.Camera.WorldToScreen(newPos);
                _mousepos = newScreenPosition;
                Mouse.SetPosition(((int)newScreenPosition.X), ((int)newScreenPosition.Y));
            }
        }

        private Point _circlePos;

        public override void Draw(GameTime gameTime)
        {
            var sb = Game.BeginDraw();
            sb.Draw(_particleEffect);

            if (Game.IsDrawBounds)
            {
                sb.DrawCircle(_revealArea, 50, Color.LightCyan);
            }

            sb.End();
        }

        public override void Close()
        {
            _postProcessor.Dispose();
            base.Close();
        }
    }
}