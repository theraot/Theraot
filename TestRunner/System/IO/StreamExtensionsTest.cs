using System;
using System.IO;
using System.Threading;
using Theraot.Core;

namespace TestRunner.System.IO
{
    [TestFixture]
    public static class StreamExtensionsTest
    {
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