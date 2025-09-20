using MonoGame.Extended;
using MonoGame.Extended.Screens.Transitions;

namespace Nabunassar
{
    public abstract class ScreenTransition : IScreenTransition
    {
        private readonly float _halfDuration;

        private float _currentSeconds;

        public TransitionState State { get; private set; } = TransitionState.Out;


        public float Duration { get; }

        public float Value => MathHelper.Clamp(_currentSeconds / _halfDuration, 0f, 1f);

        public event EventHandler StateChanged;

        public event EventHandler Completed;

        protected ScreenTransition(float duration)
        {
            if (duration < 0f)
                duration = NabunassarGame.Game.Settings.DefaultFadeTransitionDurationInSeconds;

            Duration = duration;
            _halfDuration = Duration / 2f;
        }

        public abstract void Dispose();

        public void Update(GameTime gameTime)
        {
            float elapsedSeconds = gameTime.GetElapsedSeconds();
            switch (State)
            {
                case TransitionState.Out:
                    _currentSeconds += elapsedSeconds;
                    if (_currentSeconds >= _halfDuration)
                    {
                        State = TransitionState.In;
                        this.StateChanged?.Invoke(this, EventArgs.Empty);
                    }

                    break;
                case TransitionState.In:
                    _currentSeconds -= elapsedSeconds;
                    if (_currentSeconds <= 0f)
                    {
                        this.Completed?.Invoke(this, EventArgs.Empty);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract void Draw(GameTime gameTime);
    }
}
