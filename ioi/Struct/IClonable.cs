using ioi.Entities;

namespace ioi
{
    internal interface IClonable<T>
        where T : class
    {
        T Clone(T instance=null);
    }
}
