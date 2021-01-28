using NUnit.Framework;
using System;
using System.Threading;

namespace Theraot.Threading
{
    public static class TimeoutTests
    {
        [Test]
        public static void TimeoutCancel()
        {
            RootedTimeout timeout;
            var value = new[] { 0 };
            while (true)
            {
                value[0] = 0;
                timeout = RootedTimeout.Launch(() => value[0] = 1, 1000);
                Assert.IsFalse(timeout.IsCanceled);
                timeout.Cancel();
                while (!timeout.IsCompleted && !timeout.IsCanceled)
                {
                    // Empty
                }

                if (!timeout.IsCompleted)
                {
                    break;
                }

                Assert.AreEqual(1, value[0]);
            }

            Assert.IsTrue(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
            Assert.AreEqual(0, value[0]);
        }

        [Test]
        public static void TimeoutCancelAndChange()
        {
            RootedTimeout timeout;
            var value = new[] { 0 };
            while (true)
            {
                value[0] = 0;
                timeout = RootedTimeout.Launch(() => value[0] = 1, 100);
                Assert.IsFalse(timeout.IsCanceled);
                timeout.Cancel();
                while (!timeout.IsCompleted && !timeout.IsCanceled)
                {
                    // Empty
                }

                if (!timeout.IsCompleted)
                {
                    break;
                }

                Assert.AreEqual(1, value[0]);
            }

            Assert.IsTrue(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
            Assert.AreEqual(0, value[0]);
            Assert.IsFalse(timeout.Change(1000));
            Assert.IsTrue(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
        }

        [Test]
        [Category("LongRunning")]
        public static void TimeoutChange()
        {
            RootedTimeout timeout;
            var value = new DateTime[1];
            DateTime now;
            do
            {
                now = DateTime.Now;
                value[0] = now;
                timeout = RootedTimeout.Launch(() => value[0] = DateTime.Now, 100);
            }
            while (!timeout.Change(1000));

            Assert.IsFalse(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
            ThreadingHelper.SpinWaitUntil(() => timeout.IsCompleted);
            Assert.Greater((value[0] - now).TotalMilliseconds, 100);
        }

        [Test]
        public static void TimeoutConstructorCanceledToken()
        {
            var value = new[] { 0 };
            var token = new CancellationToken(true);
            var timeout = RootedTimeout.Launch(() => value[0] = 1, 0, token);
            Assert.IsTrue(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
            Assert.AreEqual(0, value[0]);
        }

        [Test]
        public static void TimeoutConstructorCancellationToken()
        {
            var value = new[] { 0 };
            var timeout = RootedTimeout.Launch(() => value[0] = 1, 0, CancellationToken.None);
            Assert.IsFalse(timeout.IsCanceled);
        }

        [Test]
        public static void TimeoutConstructorZeroDueTime()
        {
            var value = new[] { 0 };
            var timeout = RootedTimeout.Launch(() => value[0] = 1, 0);
            ThreadingHelper.SpinWaitUntil(() => timeout.IsCompleted);
            Assert.AreEqual(1, value[0]);
        }

        [Test]
        public static void TimeoutFinishAndChange()
        {
            var value = new[] { 0 };
            var timeout = RootedTimeout.Launch(() => value[0] = 1, 100);
            Assert.IsFalse(timeout.IsCanceled);
            ThreadingHelper.SpinWaitUntil(() => timeout.IsCompleted);
            Assert.IsFalse(timeout.IsCanceled);
            Assert.IsTrue(timeout.IsCompleted);
            Assert.AreEqual(1, value[0]);
            Assert.IsFalse(timeout.Change(1000));
            Assert.IsFalse(timeout.IsCanceled);
            Assert.IsTrue(timeout.IsCompleted);
        }

        [Test]
        public static void TimeoutRemaining()
        {
            long remaining;
            RootedTimeout timeout;
            do
            {
                var now = DateTime.Now;
                var value = new[] { now };
                timeout = RootedTimeout.Launch(() => value[0] = DateTime.Now, 500);
                remaining = timeout.CheckRemaining();
            }
            while (timeout.IsCompleted);

            Assert.LessOrEqual(remaining, 500);
            Thread.Sleep(1);
            var newRemaining = timeout.CheckRemaining();
            Assert.LessOrEqual(newRemaining, remaining);
            Assert.GreaterOrEqual(remaining, 0);
            Assert.GreaterOrEqual(newRemaining, 0);
        }
    }
}