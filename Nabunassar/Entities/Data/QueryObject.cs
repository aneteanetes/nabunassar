using Nabunassar.Struct;

namespace Nabunassar.Entities.Data
{
    internal class QueryObject<T>
    {
        public int ObjectId { get; set; }

        public ObjectType ObjectType { get; set; }

        public T Data { get; set; }
    }
}
