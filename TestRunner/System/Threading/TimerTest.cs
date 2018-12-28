using System.Threading;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class TimerTest
    {
        [Test]
        public static void TimerCallbackRuns()
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Volatile.Write(ref data[0], 1), null, 700, Timeout.Infinite);
            Thread.Sleep(1000);
            Assert.AreEqual(1, Volatile.Read(ref data[0]));
            timer.Dispose();
        }

        [Test]
        public static void TimerCallbackRunsOnceWithTimeoutInfinite()
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, Timeout.Infinite);
            Thread.Sleep(1000);
            Assert.AreEqual(1, Volatile.Read(ref data[0]));
            Thread.Sleep(1000);
            Assert.AreEqual(1, Volatile.Read(ref data[0]));
            timer.Dispose();
        }

        [Test]
        public static void TimerCallbackRespectsState(int value)
        {
            var data = new[] { 0 };
            var timer = new Timer(input => Volatile.Write(ref data[0], (int)input), value, 700, Timeout.Infinite);
            Thread.Sleep(1000);
            Assert.AreEqual(value, Volatile.Read(ref data[0]));
            timer.Dispose();
        }
    }
}