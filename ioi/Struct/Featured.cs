using ioi.Monogame.Interfaces;

namespace ioi.Struct
{
    internal abstract class Featured : IFeatured
    {
        public Featured()
        {
            GameHost.Game.FeatureValues.Add(this);
        }

        public abstract void Update(GameTime gameTime);
    }
}
