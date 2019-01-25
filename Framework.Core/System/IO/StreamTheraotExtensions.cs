#if LESSTHAN_NET45

using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
#if LESSTHAN_NET40

    public static partial class StreamTheraotExtensions
    {
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
            } while (read != 0);
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
            } while (read != 0);
        }
    }

#endif

    public static
#if LESSTHAN_NET40
        partial
#endif
        class StreamTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.Factory.FromAsync
            (
                BeginRead,
                stream.EndRead,
                Tuple.Create(stream, buffer, offset, count)
            );
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            return Task.Factory.FromAsync
            (
                BeginRead,
                stream.EndRead,
                Tuple.Create(stream, buffer, offset, count)
            );
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.Factory.FromAsync
            (
                BeginWrite,
                stream.EndWrite,
                Tuple.Create(stream, buffer, offset, count)
            );
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            return Task.Factory.FromAsync
            (
                BeginWrite,
                stream.EndWrite,
                Tuple.Create(stream, buffer, offset, count)
            );
        }

        private static IAsyncResult BeginRead(AsyncCallback callback, object state)
        {
            var tuple = (Tuple<Stream, byte[], int, int>)state;
            return tuple.Item1.BeginRead(tuple.Item2, tuple.Item3, tuple.Item4, callback, tuple.Item4);
        }

        private static IAsyncResult BeginWrite(AsyncCallback callback, object state)
        {
            var tuple = (Tuple<Stream, byte[], int, int>)state;
            return tuple.Item1.BeginWrite(tuple.Item2, tuple.Item3, tuple.Item4, callback, tuple.Item4);
        }
    }
}

#endif