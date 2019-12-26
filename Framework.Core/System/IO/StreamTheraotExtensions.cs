#if LESSTHAN_NET45

#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable RECS0002 // Convert anonymous method to method group
#pragma warning disable S112 // General exceptions should never be thrown

using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;

namespace System.IO
{
#if LESSTHAN_NET40

    public static partial class StreamTheraotExtensions
    {
        private const int _defaultBufferSize = 4096;

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo(this Stream source, Stream destination)
        {
            //Added in .NET 4.0
            if (source == null)
            {
                throw new NullReferenceException();
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var buffer = new byte[_defaultBufferSize];
            int read;
            do
            {
                read = source.Read(buffer, 0, _defaultBufferSize);
                destination.Write(buffer, 0, read);
            } while (read != 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo(this Stream source, Stream destination, int bufferSize)
        {
            //Added in .NET 4.0
            if (source == null)
            {
                throw new NullReferenceException();
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var buffer = new byte[bufferSize];
            int read;
            do
            {
                read = source.Read(buffer, 0, bufferSize);
                destination.Write(buffer, 0, read);
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
        public static Task CopyToAsync(this Stream source, Stream destination)
        {
            if (source == null)
            {
                throw new NullReferenceException();
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (!source.CanRead)
            {
                throw new NotSupportedException("Source stream does not support read.");
            }

            if (!destination.CanWrite)
            {
                throw new NotSupportedException("Destination stream does not support write.");
            }

            return CopyToPrivateAsync(source, destination, ArrayReservoir.MaxCapacity, CancellationToken.None);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task CopyToAsync(this Stream source, Stream destination, int bufferSize)
        {
            if (source == null)
            {
                throw new NullReferenceException();
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            if (!source.CanRead)
            {
                throw new NotSupportedException("Source stream does not support read.");
            }

            if (!destination.CanWrite)
            {
                throw new NotSupportedException("Destination stream does not support write.");
            }

            return CopyToPrivateAsync(source, destination, bufferSize, CancellationToken.None);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task CopyToAsync(this Stream source, Stream destination, CancellationToken cancellationToken)
        {
            if (source == null)
            {
                throw new NullReferenceException();
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (!source.CanRead)
            {
                throw new NotSupportedException("Source stream does not support read.");
            }

            if (!destination.CanWrite)
            {
                throw new NotSupportedException("Destination stream does not support write.");
            }

            return CopyToPrivateAsync(source, destination, ArrayReservoir.MaxCapacity, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task CopyToAsync(this Stream source, Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            if (source == null)
            {
                throw new NullReferenceException();
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            if (!source.CanRead)
            {
                throw new NotSupportedException("Source stream does not support read.");
            }

            if (!destination.CanWrite)
            {
                throw new NotSupportedException("Destination stream does not support write.");
            }

            return CopyToPrivateAsync(source, destination, bufferSize, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FlushAsync(this Stream stream)
        {
            if (stream == null)
            {
                throw new NullReferenceException();
            }

            return TaskEx.Run(stream.Flush);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FlushAsync(this Stream stream, CancellationToken token)
        {
            if (stream == null)
            {
                throw new NullReferenceException();
            }

            token.ThrowIfCancellationRequested();
            return TaskEx.Run(stream.Flush, token);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new NullReferenceException();
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.Factory.FromAsync
            (
                BeginRead!,
                stream.EndRead,
                buffer,
                offset,
                count,
                stream
            );
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null)
            {
                throw new NullReferenceException();
            }

            return Task.Factory.FromAsync
            (
                BeginRead!,
                stream.EndRead,
                buffer,
                offset,
                count,
                stream
            );
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new NullReferenceException();
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.Factory.FromAsync
            (
                BeginWrite!,
                stream.EndWrite,
                buffer,
                offset,
                count,
                stream
            );
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null)
            {
                throw new NullReferenceException();
            }

            return Task.Factory.FromAsync
            (
                BeginWrite!,
                stream.EndWrite,
                buffer,
                offset,
                count,
                stream
            );
        }

        private static IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var steam = (Stream)state;
            return steam.BeginRead(buffer, offset, count, callback, count);
        }

        private static IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var steam = (Stream)state;
            return steam.BeginWrite(buffer, offset, count, callback, count);
        }

        private static async Task CopyToPrivateAsync(Stream source, Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            var buffer = ArrayReservoir<byte>.GetArray(bufferSize);
            try
            {
                while (true)
                {
                    var bytesRead = await source.ReadAsync(buffer, 0, bufferSize, cancellationToken).ConfigureAwait(false);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                ArrayReservoir<byte>.DonateArray(buffer);
            }
        }
    }
}

#endif