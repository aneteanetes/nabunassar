using Microsoft.Xna.Framework.Graphics;

namespace Nabunassar.Shaders
{
    internal class GlobalSnow : PostProcessShader
    {
        private int _seconds;

        public GlobalSnow(NabunassarGame game) : base(game, "Assets/Shaders/GlobalSnow.fx")
        {
            _seconds = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.CanUpdate(gameTime, TimeSpan.FromMilliseconds(50)))
            {
                _seconds++;
                if (_seconds >= 600)
                    _seconds = 0;
            }

            Effect.Parameters["TIME"].SetValue(_seconds);
        }
    }
}
