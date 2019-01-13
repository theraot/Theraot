using System;
using System.Threading;

namespace TestRunner.System.Threading
{
    [TestFixture]
    public static class TimerTest
    {
        [Test]
        public static void TimerCallbackRespectsState(int value)
        {
            var data = new[] { 0 };
            var timer = new Timer(input => Volatile.Write(ref data[0], (int)input), value, 700, Timeout.Infinite);
            Thread.Sleep(1000);
            Assert.AreEqual(value, Volatile.Read(ref data[0]));
            timer.Dispose();
        }

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
        public static void TimerCallbackRunsMultipleTimesAccordingPeriod()
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            Thread.Sleep(1000);
            Assert.AreEqual(6, Volatile.Read(ref data[0]));
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
        public static void TimerCanBeChanged()
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            timer.Change(700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(4, Volatile.Read(ref data[0]));
            timer.Dispose();
        }

        [Test]
        public static void TimerCanBeStopped()
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            timer.Dispose();
        }

        [Test]
        public static void TimerCanBeStoppedIgnoringPeriod
        (
            [UseGenerator(typeof(SmallPositiveNumericGenerator))] int period
        )
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            timer.Change(Timeout.Infinite, period);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            timer.Dispose();
        }

        [Test]
        public static void TimerChangeWithDueTimeBelowNegativeOneThrows
        (
            [UseGenerator(typeof(SmallPositiveNumericGenerator))] int dueTime
        )
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            // ReSharper disable once AccessToDisposedClosure
            Assert.Throws<ArgumentOutOfRangeException>(() => timer.Change(-2 - dueTime, Timeout.Infinite));
            timer.Dispose();
        }

        [Test]
        public static void TimerChangeWithPeriodBelowNegativeOneThrows
        (
            [UseGenerator(typeof(SmallPositiveNumericGenerator))] int period
        )
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            // ReSharper disable once AccessToDisposedClosure
            Assert.Throws<ArgumentOutOfRangeException>(() => timer.Change(Timeout.Infinite, -2 - period));
            timer.Dispose();
        }

        [Test]
        public static void TimerDueTimeBelowMinusOneThrows
        (
            [UseGenerator(typeof(SmallPositiveNumericGenerator))] int dueTime
        )
        {
            Assert.Throws<ArgumentOutOfRangeException, Timer>(() => new Timer(_ => { }, null, -2 - dueTime, Timeout.Infinite));
        }

        [Test]
        public static void TimerDueTimeCanBeChanged()
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            timer.Change(700, Timeout.Infinite);
            Thread.Sleep(1000);
            Assert.AreEqual(3, Volatile.Read(ref data[0]));
            timer.Dispose();
        }

        [Test]
        public static void TimerInfiniteDueTimeIgnoresPositivePeriod
        (
            [UseGenerator(typeof(SmallPositiveNumericGenerator))] int period
        )
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, Timeout.Infinite, period);
            Thread.Sleep(1000);
            Assert.AreEqual(0, Volatile.Read(ref data[0]));
            timer.Dispose();
        }

        [Test]
        public static void TimerPeriodBelowMinusOneThrows
        (
            [UseGenerator(typeof(SmallPositiveNumericGenerator))] int period
        )
        {
            Assert.Throws<ArgumentOutOfRangeException, Timer>(() => new Timer(_ => { }, null, Timeout.Infinite, -2 - period));
        }

        [Test]
        public static void TimerPeriodCanBeChanged()
        {
            var data = new[] { 0 };
            var timer = new Timer(_ => Interlocked.Increment(ref data[0]), null, 700, 217);
            Thread.Sleep(1000);
            Assert.AreEqual(2, Volatile.Read(ref data[0]));
            timer.Change(0, 285);
            Thread.Sleep(1000);
            Assert.AreEqual(6, Volatile.Read(ref data[0]));
            timer.Dispose();
        }
    }
}