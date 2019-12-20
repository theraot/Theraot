using NUnit.Framework;
using System.Threading;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public class CancellationTokenSourceTestEx
    {
        [Test]
        [Category("LongRunning")]
        public void CancelAfterDisposed()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            cts.Dispose();
            Thread.Sleep(2000);
            Assert.IsFalse(cts.IsCancellationRequested);
        }
    }
}