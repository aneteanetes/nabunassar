using Microsoft.Xna.Framework.Graphics;

namespace Nabunassar.Components.Effects
{
    internal abstract class EffectComponent
    {
        public EffectComponent(NabunassarGame game)
        {
        }

        public abstract void Update(GameTime gameTime);

        public Effect Effect { get; set; }
    }
}
