using System;
using System.IO;

namespace Theraot.Core
{
    public static class StreamExtensions
    {
        private const int INT_PageSize = 4096;

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
                byte[] buffer = new byte[INT_PageSize];
                int read;
                do
                {
                    read = input.Read(buffer, 0, INT_PageSize);
                    output.Write(buffer, 0, read);
                }
                while (read != 0);
            }
        }

        public static void CopyTo(this Stream input, Stream output, long count)
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
                byte[] buffer = new byte[INT_PageSize];
                int read;
                do
                {
                    read = input.Read(buffer, 0, INT_PageSize);
                    output.Write(buffer, 0, read);
                    count = count - (long)read;
                }
                while (read != 0);
            }
        }

        public static byte[] ToArray(this Stream stream)
        {
            if (ReferenceEquals(stream, null))
            {
                throw new ArgumentNullException("stream");
            }
            else
            {
                MemoryStream _stream = stream as MemoryStream;
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