using LiteDB;
using Nabunassar.Monogame.Viewport;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Nabunassar.Content
{
    [DebuggerDisplay("{Path}")]
    public class Resource : IDisposable
    {
        /// <summary>
        /// Внутреннее свойство для LiteDb
        /// </summary>
        public int Id { get; set; }

        private class InformableMemoryStream : MemoryStream
        {
            public InformableMemoryStream(byte[] buffer) : base(buffer)
            {
            }

            public bool Disposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }
        }

        public string Path { get; set; }

        public string CustomInfo { get; set; }

        public PossibleResolution Resolution { get; set; }

        public byte[] Data { get; set; }

        public DateTime LastWriteTime { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        private InformableMemoryStream stream;

        [BsonIgnore]
        [JsonIgnore]
        public Stream Stream
        {
            get
            {
                if (stream == default || stream.Disposed)
                {
                    stream = new InformableMemoryStream(Data);
                }

                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                return stream;
            }
        }

        [BsonIgnore]
        public void Dispose()
        {
            OnDispose?.Invoke();
            stream?.Dispose();
            Data = null;
            GC.Collect();
        }

        [BsonIgnore]
        [JsonIgnore]
        public Action OnDispose { get; set; }

        public const string DataFileExtension = ".ndf";
    }
}
