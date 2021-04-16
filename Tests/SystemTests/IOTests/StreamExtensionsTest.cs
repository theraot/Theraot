#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.SystemTests.IOTests
{
    [TestFixture]
    public static class StreamExtensionsTest
    {
        [Test]
        public static void ReadAsyncReads()
        {
            ReadAsyncReadsExtracted().Wait();
        }

        [Test]
        public static void ReadAsyncThrowsOnCanceledToken()
        {
            var stream = new MemoryStream[1];
            using (stream[0] = new MemoryStream(new byte[50]))
            {
                var tokenSource = new CancellationTokenSource[1];
                using (tokenSource[0] = new CancellationTokenSource())
                {
                    tokenSource[0].Cancel();
                    var buffer = new byte[10];
                    AssertEx.AsyncThrows<OperationCanceledException>(() => stream[0].ReadAsync(buffer, 0, 10, tokenSource[0].Token));
                }
            }
        }

        [Test]
        public static void WriteAsyncThrowsOnCanceledToken()
        {
            var stream = new MemoryStream[1];
            using (stream[0] = new MemoryStream(new byte[50]))
            {
                var tokenSource = new CancellationTokenSource[1];
                using (tokenSource[0] = new CancellationTokenSource())
                {
                    tokenSource[0].Cancel();
                    var buffer = new byte[10];
                    AssertEx.AsyncThrows<OperationCanceledException>(() => stream[0].ReadAsync(buffer, 0, 10, tokenSource[0].Token));
                }
            }
        }

        private static async Task ReadAsyncReadsExtracted()
        {
            // ReSharper disable once UseAwaitUsing
            using (var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }))
            {
                var buffer = new byte[10];
                var x = await stream.ReadAsync(buffer, 0, 10).ConfigureAwait(false);
                Assert.AreEqual(10, x);
                AssertEx.CollectionEquals(buffer, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
            }
        }
    }
}