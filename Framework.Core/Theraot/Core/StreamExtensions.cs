// Needed for Workaround

using System;
using System.IO;

#if LESSTHAN_NET45

using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#endif

namespace Theraot.Core
{
    public static class StreamExtensions
    {
#if LESSTHAN_NET40
        private const int _defaultBufferSize = 4096;

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo(this Stream input, Stream output)
        {
            //Added in .NET 4.0
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (output == null)
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

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo(this Stream input, Stream output, int bufferSize)
        {
            //Added in .NET 4.0
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (output == null)
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

#endif

#if LESSTHAN_NET45

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static async Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Task.Factory.FromAsync
            (
                BeginRead,
                stream.EndRead,
                Tuple.Create(stream, buffer, offset, count)
            ).ConfigureAwait(false);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static async Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            return await Task.Factory.FromAsync
            (
                BeginRead,
                stream.EndRead,
                Tuple.Create(stream, buffer, offset, count)
            ).ConfigureAwait(false);
        }

        private static IAsyncResult BeginRead(AsyncCallback callback, object state)
        {
            var tuple = (Tuple<Stream, byte[], int, int>)state;
            return tuple.Item1.BeginRead(tuple.Item2, tuple.Item3, tuple.Item4, callback, tuple.Item4);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static async Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Factory.FromAsync
            (
                BeginWrite,
                stream.EndWrite,
                Tuple.Create(stream, buffer, offset, count)
            ).ConfigureAwait(false);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static async Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            await Task.Factory.FromAsync
            (
                BeginWrite,
                stream.EndWrite,
                Tuple.Create(stream, buffer, offset, count)
            ).ConfigureAwait(false);
        }

        private static IAsyncResult BeginWrite(AsyncCallback callback, object state)
        {
            var tuple = (Tuple<Stream, byte[], int, int>)state;
            return tuple.Item1.BeginWrite(tuple.Item2, tuple.Item3, tuple.Item4, callback, tuple.Item4);
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
            if (stream == null)
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