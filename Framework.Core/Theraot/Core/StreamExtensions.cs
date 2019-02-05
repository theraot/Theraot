#if FAT
using System;
using System.IO;
using System.Threading.Tasks;

namespace Theraot.Core
{
    public static class StreamExtensions
    {
        public static bool IsClosed(this Stream stream)
        {
            return !stream.CanRead && !stream.CanWrite;
        }

        public static void ReadComplete(this Stream source, byte[] buffer, int offset, int length)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            while (length > 0)
            {
                var delta = source.Read(buffer, offset, length);
                if (delta <= 0)
                {
                    throw new EndOfStreamException();
                }
                length -= delta;
                offset += delta;
            }
        }

        public static Task ReadCompleteAsync(this Stream source, byte[] buffer, int offset, int length)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (!source.CanRead)
            {
                throw new NotSupportedException("Source stream does not support read.");
            }
            return ReadCompletePrivateAsync(source, buffer, offset, length);
        }

        public static byte[] ToArray(this Stream stream)
        {
            switch (stream)
            {
                case null:
                    throw new ArgumentNullException(nameof(stream));
                case MemoryStream streamAsMemoryStream:
                    return streamAsMemoryStream.ToArray();
                default:
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
            }
        }

        private static async Task ReadCompletePrivateAsync(this Stream source, byte[] buffer, int offset, int length)
        {
            while (length > 0)
            {
                var delta = await source.ReadAsync(buffer, offset, length).ConfigureAwait(false);
                if (delta <= 0)
                {
                    throw new EndOfStreamException();
                }
                length -= delta;
                offset += delta;
            }
        }
    }
}
#endif