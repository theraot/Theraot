using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#if LESSTHAN_NET45

using Theraot.Core;

#endif

namespace TestRunner.System.IO
{
    [TestFixture]
    public static class StreamExtensionsTest
    {
        [Test]
        public static async Task ReadAsyncReads()
        {
            var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
            var buffer = new byte[10];
            // ReSharper disable once MethodSupportsCancellation
            int x = await stream.ReadAsync(buffer, 0, 10);
            Assert.AreEqual(10, x);
            Assert.CollectionEquals(buffer, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
        }

        [Test]
        public static void ReadAsyncThrowsOnCanceledToken()
        {
            var stream = new MemoryStream(new byte[50]);
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            var buffer = new byte[10];
            // ReSharper disable once MethodSupportsCancellation
            Assert.ThrowsAsync<OperationCanceledException>(() => stream.ReadAsync(buffer, 0, 10, tokenSource.Token));
        }

        [Test]
        public static void WriteAsyncThrowsOnCanceledToken()
        {
            var stream = new MemoryStream(new byte[50]);
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            var buffer = new byte[10];
            // ReSharper disable once MethodSupportsCancellation
            Assert.ThrowsAsync<OperationCanceledException>(() => stream.ReadAsync(buffer, 0, 10, tokenSource.Token));
        }
    }
}