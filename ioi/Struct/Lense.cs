namespace ioi.Struct
{
    public class Lens<TSource, TProp>(Func<TSource, TProp> get, Func<TSource, TProp, TProp> set)
    {
        private readonly Func<TSource, TProp> get = get;
        private readonly Func<TSource, TProp, TProp> set = set;

        public virtual TProp Get(TSource source) => get(source);

        public virtual TProp Set(TSource source, TProp value) => set(source, value);
    }
}