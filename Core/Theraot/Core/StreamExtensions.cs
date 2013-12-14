using System;
using System.IO;

namespace Theraot.Core
{
    public static class StreamExtensions
    {
        private const int INT_DefaultBufferSize = 4096;

#if NET20 || NET30 || NET35
        public static void CopyTo(this Stream input, Stream output)
        {
            //Added in .NET 4.0
            if (ReferenceEquals(input, null))
            {
                throw new ArgumentNullException("input");
            }
            else if (ReferenceEquals(output, null))
            {
                throw new ArgumentNullException("output");
            }
            else
            {
                var buffer = new byte[INT_DefaultBufferSize];
                int read;
                do
                {
                    read = input.Read(buffer, 0, INT_DefaultBufferSize);
                    output.Write(buffer, 0, read);
                }
                while (read != 0);
            }
        }
#else
        public static void CopyTo(Stream input, Stream output)
        {
            //Added in .NET 4.0
            input.CopyTo(output);
        }
#endif

#if NET20 || NET30 || NET35
        public static void CopyTo(this Stream input, Stream output, int bufferSize)
        {
            //Added in .NET 4.0
            if (ReferenceEquals(input, null))
            {
                throw new ArgumentNullException("input");
            }
            else if (ReferenceEquals(output, null))
            {
                throw new ArgumentNullException("output");
            }
            else
            {
                var buffer = new byte[bufferSize];
                int read;
                do
                {
                    read = input.Read(buffer, 0, bufferSize);
                    output.Write(buffer, 0, read);
                }
                while (read != 0);
            }
        }
#else
        public static void CopyTo(Stream input, Stream output, int bufferSize)
        {
            //Added in .NET 4.0
            input.CopyTo(output, bufferSize);
        }

        public static byte[] ToArray(this Stream stream)
        {
            if (ReferenceEquals(stream, null))
            {
                throw new ArgumentNullException("stream");
            }
            else
            {
                var _stream = stream as MemoryStream;
                if (_stream != null)
                {
                    return _stream.ToArray();
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
        }
    }
}