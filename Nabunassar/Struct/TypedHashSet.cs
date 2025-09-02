using Nabunassar.Entities.Data.Effects.PartyEffects;
using System.Collections;

namespace Nabunassar.Struct
{
    internal class TypedHashSet<T> : Featured, IEnumerable<T>
    {
        private readonly Dictionary<int,T> _data = new();
        private readonly HashSet<int> _containedTypeHashes = new();

        public bool IsChanged {  get; private set; }

        public override void Update(GameTime gameTime)
        {
            IsChanged = false;
        }

        public virtual bool Add(T data)
        {
            var dataType = data.GetType();
            var typeHash = dataType.GetHashCode();

            if (IsTypeExists(typeHash))
                throw new InvalidOperationException($"Element of type '{dataType}' already added!");

            var isAdded = _data.TryAdd(typeHash, data);
            if (isAdded)
                _containedTypeHashes.Add(typeHash);

            IsChanged = true;

            return isAdded;
        }

        public bool Remove(T data)
        {
            var type = data.GetType();
            var typeHash = type.GetHashCode();

            var isRemoved = _data.Remove(typeHash);
            if (isRemoved)
                _containedTypeHashes.Remove(typeHash);

            IsChanged = true;

            if(data is IDisposable disposable) 
                disposable.Dispose();

            return isRemoved;
        }

        public T Get<TSearch>() 
            => Get(typeof(TSearch));

        public bool TryGet<TSearch>(out T result)
        {
            result = default;

            var hash = typeof(TSearch).GetHashCode();

            if (!IsTypeExists(hash))
                return false;

            result = _data[hash];

            return true;
        }

        public bool Contains<TSearch>() 
            => IsTypeExists(typeof(TSearch).GetHashCode());

        public bool Remove<TSearch>()
        {
            var hash = typeof(TSearch).GetHashCode();

            if (!IsTypeExists(hash))
                return false;

            var item = Get<TSearch>();
            return Remove(item);
        }

        protected bool IsTypeExists(int typeHash)
        {
            return _containedTypeHashes.Contains(typeHash);
        }

        public T this[Type type]
        {
            get => Get(type);
        }

        private T Get(Type type)
        {
            var hash = type.GetHashCode();
            if (_data.TryGetValue(hash, out var result))
                return result;

            throw new InvalidDataException($"Element of type {type} does not exists in hash!");
        }

        public IEnumerator<T> GetEnumerator() => _data.Select(x => x.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
