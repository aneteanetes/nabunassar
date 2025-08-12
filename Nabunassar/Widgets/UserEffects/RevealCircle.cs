using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using Myra.Graphics2D.UI;
using Nabunassar.Widgets.Base;
using ParticleEffect = MonoGame.Extended.Particles.ParticleEffect;

namespace Nabunassar.Widgets.UserEffects
{
    internal class RevealCircle : ScreenWidget
    {
        private Texture2D _particleTexture;
        private ParticleEffect _particleEffect;

        public override bool IsModal => true;

        public RevealCircle(NabunassarGame game) : base(game)
        {
        }

        private static float _radius = 50;
        private static int _capacity = 500;
        private Profile.CircleRadiation _radiation = Profile.CircleRadiation.Out;
        private float _gravityStrength = 5000;
        private Range<float> _scale = new Range<float>(1f, 1.9f);
        private Range<float> _scaleForGravity = new Range<float>(0.2f, .7f);

        public override void LoadContent()
        {
            _particleTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            _particleTexture.SetData(new[] { Color.White });

            var circleTexture = Content.LoadTexture("Assets/Images/Interface/circleblend400.png");

            Game.GrayscaleMapActivate(circleTexture, () => new Rectangle(_circlePos, new Size(100, 100)), () => _dissapearingTimer);

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
                            //new AgeModifier
                            //{
                            //    Interpolators =
                            //    {
                            //        new ColorInterpolator
                            //        {
                            //            StartValue = new HslColor(0.33f, 0.5f, 0.5f),
                            //            EndValue = new HslColor(0.5f, 0.9f, 1.0f)
                            //        }
                            //    }
                            //},
                            //new RotationModifier {RotationRate = -2.1f},
                            //new RectangleContainerModifier {Width = 800, Height = 480},
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
                        },
                        Modifiers =
                        {
                            //new AgeModifier
                            //{
                            //    Interpolators =
                            //    {
                            //        new ColorInterpolator
                            //        {
                            //            StartValue = new HslColor(0.33f, 0.5f, 0.5f),
                            //            EndValue = new HslColor(0.5f, 0.9f, 1.0f)
                            //        }
                            //    }
                            //},
                            //new RotationModifier {RotationRate = -2.1f},
                            //new RectangleContainerModifier {Width = 800, Height = 480},
                        }
                    }
                ]
            };


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
                    Game.GrayscaleMapDisable();
                    return;
                }
            }

            _mouse = MouseExtended.GetState();

            _mousepos = _mouse.Position.ToVector2();
            _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (!_isDissapearing)
            {
                _particleEffect.Position = Game.Camera.ScreenToWorld(_mousepos);

                if (_mouse.WasButtonPressed(MouseButton.Left))
                {
                    _particleEffect.Emitters.Remove(_particleEffect.Emitters.FirstOrDefault());
                    foreach (var emmiter in _particleEffect.Emitters)
                    {
                        emmiter.LifeSpan = TimeSpan.FromSeconds(1);
                    }

                    _particleEffect.Trigger(_mousepos);
                    _isDissapearing = true;
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

        private Point _circlePos;
        private float _grayIntensive=0;

        public override void Draw(GameTime gameTime)
        {
            var sb = Game.BeginDraw();
            sb.Draw(_particleEffect);
            sb.End();
        }
    }
}
