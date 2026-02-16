using Geranium.Reflection;

namespace ioi.Stats
{
    public abstract class ParameterModifier<N> : ParameterModifier
    {
        public virtual N Get(N value) => value;

        public virtual N Set(N value) => value;

        public override T Get<T>(T value)
        {
            if (value is N nValue)
                return Get(nValue).As<T>();

            return base.Get(value);
        }

        public override T Set<T>(T value)
        {
            if (value is N nValue)
                return Set(nValue).As<T>();

            return base.Set(value);
        }
    }
}
