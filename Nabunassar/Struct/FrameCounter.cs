﻿namespace Nabunassar.Struct
{
    public class FrameCounter
    {
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public bool IsRunningSlowly { get; private set; }

        public const int MaximumSamples = 100;

        private Queue<float> _sampleBuffer = new();

        public void Update(float deltaTime, bool isRunningSlowly)
        {
            IsRunningSlowly=isRunningSlowly;
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MaximumSamples)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
        }

        public override string ToString()
        {
            return $"FPS: {AverageFramesPerSecond} ({(IsRunningSlowly ? "slowly" : "")})";
        }
    }
}
