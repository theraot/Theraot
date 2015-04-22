using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public class CancellationTokenSourceTestEx
    {
        [Test]
        public void CancelAfterDisposed()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            cts.Dispose();
            Thread.Sleep(2000);
            Assert.IsFalse(cts.IsCancellationRequested);
        }

        [Test]
        public void RegisterWhileCancelling()
        {
            // TODO - fix test case
            var cts = new CancellationTokenSource();
            var mre = new ManualResetEvent(false);
            var mre2 = new ManualResetEvent(false);
            int called = 0;

            cts.Token.Register(() =>
            {
                Assert.IsTrue(cts.IsCancellationRequested, "#10");
                Assert.IsTrue(cts.Token.WaitHandle.WaitOne(0), "#11");
                mre2.Set();
                mre.WaitOne(3000);
                called += 11;
            });

            var t = TaskScheduler.Default.AddWork(() => { cts.Cancel(); });
            t.Start();

            Assert.IsTrue(mre2.WaitOne(1000), "#0");
            cts.Token.Register(() => { called++; });
            Assert.AreEqual(1, called, "#1");
            Assert.IsFalse(t.IsCompleted, "#2");

            mre.Set();
            Assert.IsTrue(t.Wait(1000), "#3");
            Assert.AreEqual(12, called, "#4");
        }
    }
}