using ioi.Entities.Data.Descriptions;
using ioi.Monogame.Interfaces;
using ioi.Struct.Interfaces;

namespace ioi.Entities.Data.Effects
{
    internal abstract class BaseEffect : IStackable, IFeatured, IDisposable
    {
        protected GameHost Game { get; set; }

        public BaseEffect(GameHost game)
        {
            Game = game;
            game.FeatureValues.Add(this);
        }

        public virtual EffectType Type => EffectType.Condition;

        public virtual string IconPath => "";

        public virtual Color IconColor => Color.White;

        public abstract Description GetDescription();

        public int Charges { get; set; } = 1;

        public virtual void Merge(IStackable other) { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Dispose()
        {
            Game.FeatureValues.Remove(this);
        }
    }
}
