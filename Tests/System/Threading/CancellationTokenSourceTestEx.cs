using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public class CancellationTokenSourceTestEx
    {
#if NET20 || NET30 || NET35 || NET45
        [Test]
        public void CancelAfterDisposed()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            cts.Dispose();
            Thread.Sleep(2000);
            Assert.IsFalse(cts.IsCancellationRequested);
        }
#endif

#if FAT && (NET20 || NET30 || NET35)
        [Test]
        [Category("RaceCondition")] // This test creates a race condition, that when resolved sequentially will fail
        public void RegisterWhileCancelling()
        {
            var cts = new CancellationTokenSource();
            var mre = new ManualResetEvent(false);
            var mre2 = new ManualResetEvent(false);
            var called = 0;

            cts.Token.Register(() =>
            {
                Assert.IsTrue(cts.IsCancellationRequested, "#10");
                Assert.IsTrue(cts.Token.WaitHandle.WaitOne(0), "#11");
                mre2.Set();
                mre.WaitOne(3000);
                called += 11;
            });

            var t = Task.Factory.StartNew(() => cts.Cancel());

            Assert.IsTrue(mre2.WaitOne(1000), "#0");
            cts.Token.Register(() => called++);
            Assert.AreEqual(1, called, "#1");
            Assert.IsFalse(t.IsCompleted, "#2");

            mre.Set();
            Assert.IsTrue(t.Wait(1000), "#3");
            Assert.AreEqual(12, called, "#4");
        }
#endif
    }
}