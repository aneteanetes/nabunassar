namespace Nabunassar
{
    internal class Result<T>
    {
        public T Value { get; set; }

        public Result(T result)
        {
            Value = result;
        }

        public Result(T result, string message):this(result)
        {
            Message = message;
        }

        public string Message { get; set; }


        public static implicit operator Result<T>(T result) => new Result<T>(result);

        public static implicit operator T(Result<T> result) => result.Value;
    }
}
