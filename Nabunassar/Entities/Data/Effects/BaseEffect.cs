using Nabunassar.Entities.Data.Descriptions;
using Nabunassar.Monogame.Interfaces;
using Nabunassar.Struct.Interfaces;

namespace Nabunassar.Entities.Data.Effects
{
    internal abstract class BaseEffect : IStackable, IFeatured, IDisposable
    {
        protected NabunassarGame Game { get; set; }

        public BaseEffect(NabunassarGame game)
        {
            Game = game;
            game.FeatureValues.Add(this);
        }

        public virtual string IconPath => "";

        public virtual Color IconColor => Color.White;

        public abstract Description GetDescription();

        public int Charges { get; set; }

        public virtual void Merge(IStackable other) { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Dispose()
        {
            Game.FeatureValues.Remove(this);
        }
    }
}
