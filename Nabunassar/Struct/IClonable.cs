using Nabunassar.Entities;

namespace Nabunassar
{
    internal interface IClonable<T>
        where T : class
    {
        T Clone(T instance=null);
    }
}
