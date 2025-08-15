using Geranium.Reflection;

namespace Nabunassar.Struct
{
    public struct Pair<TKey, TValue>
    {
        public Pair(TKey first, TValue second)
        {
            First = first;
            Second = second;
        }

        public TKey First { get; set; }

        public TValue Second { get; set; }

        public override bool Equals(object obj)
        {
            var internalValue = obj.GetPropValue<string>(nameof(InternalValue));
            return internalValue == InternalValue;
        }

        private string InternalValue => First.ToString();

        public override int GetHashCode() => InternalValue.GetHashCode();
    }
}