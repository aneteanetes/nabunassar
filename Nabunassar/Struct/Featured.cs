using Nabunassar.Monogame.Interfaces;

namespace Nabunassar.Struct
{
    internal abstract class Featured : IFeatured
    {
        public Featured()
        {
            NabunassarGame.Game.FeatureValues.Add(this);
        }

        public abstract void Update(GameTime gameTime);
    }
}
