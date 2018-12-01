// Needed for Workaround

using System;
using System.IO;

namespace Theraot.Core
{
    public static class StreamExtensions
    {
#if NET20 || NET30 || NET35
        private const int _defaultBufferSize = 4096;

        public static void CopyTo(this Stream input, Stream output)
        {
            //Added in .NET 4.0
            if (ReferenceEquals(input, null))
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (ReferenceEquals(output, null))
            {
                throw new ArgumentNullException(nameof(output));
            }
            var buffer = new byte[_defaultBufferSize];
            int read;
            do
            {
                read = input.Read(buffer, 0, _defaultBufferSize);
                output.Write(buffer, 0, read);
            }
            while (read != 0);
        }

#else

        public static void CopyTo(Stream input, Stream output)
        {
            // Added in .NET 4.0
            input.CopyTo(output);
        }

#endif

#if NET20 || NET30 || NET35

        public static void CopyTo(this Stream input, Stream output, int bufferSize)
        {
            //Added in .NET 4.0
            if (ReferenceEquals(input, null))
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (ReferenceEquals(output, null))
            {
                throw new ArgumentNullException(nameof(output));
            }
            var buffer = new byte[bufferSize];
            int read;
            do
            {
                read = input.Read(buffer, 0, bufferSize);
                output.Write(buffer, 0, read);
            }
            while (read != 0);
        }

#else

        public static void CopyTo(Stream input, Stream output, int bufferSize)
        {
            // Added in .NET 4.0
            input.CopyTo(output, bufferSize);
        }

#endif

        public static bool IsDisposed(this Stream stream)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Current);
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        public static void ReadComplete(this Stream stream, byte[] buffer, int offset, int length)
        {
            if (ReferenceEquals(stream, null))
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
            if (ReferenceEquals(stream, null))
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream is MemoryStream streamAsMemoryStream)
            {
                return streamAsMemoryStream.ToArray();
            }
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}