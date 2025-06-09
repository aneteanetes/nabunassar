namespace Nabunassar
{
    public static class StreamExtensions
    {
        public static string AsString(this Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
