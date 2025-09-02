using Nabunassar.Struct.Interfaces;

namespace Nabunassar.Struct
{
    internal class TypedHashSetStackable<T> : TypedHashSet<T>
        where T : IStackable
    {
        public override bool Add(T data)
        {
            var dataType = data.GetType();
            var typeHash = dataType.GetHashCode();

            if (IsTypeExists(typeHash))
            {
                var existed = this[data.GetType()];
                existed.Merge(data);

                return true;
            }

            return base.Add(data);
        }
    }
}
