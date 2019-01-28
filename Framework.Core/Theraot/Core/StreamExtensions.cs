using System;
using System.IO;

namespace Theraot.Core
{
    public static class StreamExtensions
    {
        public static bool IsClosed(this Stream stream)
        {
            return !stream.CanRead && !stream.CanWrite;
        }

        public static void ReadComplete(this Stream stream, byte[] buffer, int offset, int length)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            while (length > 0)
            {
                var delta = stream.Read(buffer, offset, length);
                if (delta <= 0)
                {
                    throw new EndOfStreamException();
                }
                length -= delta;
                offset += delta;
            }
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
                    break;
            }

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}