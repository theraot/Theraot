using NUnit.Framework;
using System.Threading;

#if FAT

using System.Threading.Tasks;

#endif

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

#if FAT

        [Test]
        [Category("RaceCondition")] // This test creates a race condition, that when resolved sequentially will fail
        public void RegisterWhileCancelling()
        {
            using (var cts = new CancellationTokenSource())
            {
                var ctsa = new[] { cts };
                using (var mre = new ManualResetEvent(false))
                {
                    using (var mre2 = new ManualResetEvent(false))
                    {
                        var mrea = new[] { mre, mre2 };
                        var called = 0;

                        cts.Token.Register(() =>
                        {
                            Assert.IsTrue(ctsa[0].IsCancellationRequested, "#10");
                            Assert.IsTrue(ctsa[0].Token.WaitHandle.WaitOne(0), "#11");
                            mrea[1].Set();
                            mrea[0].WaitOne(3000);
                            called += 11;
                        });

                        var t = Task.Factory.StartNew(() => ctsa[0].Cancel(), CancellationToken.None);

                        Assert.IsTrue(mre2.WaitOne(1000), "#0");
                        cts.Token.Register(() => called++);
                        Assert.AreEqual(1, called, "#1");
                        Assert.IsFalse(t.IsCompleted, "#2");

                        mre.Set();
                        Assert.IsTrue(t.Wait(1000), "#3");
                        Assert.AreEqual(12, called, "#4");
                    }
                }
            }
        }

#endif
    }
}