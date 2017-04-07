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
            var value = new int[] { 0 };
            var timeout = new Timeout(() => value[0] = 1, 1000);
            Assert.IsFalse(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted); // race condition
            timeout.Cancel();
            Assert.IsTrue(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
            Assert.AreEqual(0, value[0]);
        }

        [Test]
        public static void TimeoutCancelAndChange()
        {
            var value = new[] { 0 };
            var timeout = new Timeout(() => value[0] = 1, 100);
            Assert.IsFalse(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted); // race condition
            timeout.Cancel();
            Assert.IsTrue(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
            Assert.AreEqual(0, value[0]);
            Assert.IsFalse(timeout.Change(1000));
            Assert.IsTrue(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
        }

        [Test]
        public static void TimeoutChange()
        {
            var now = DateTime.Now;
            var value = new DateTime[] { now };
            var timeout = new Timeout(() => value[0] = DateTime.Now, 100);
            timeout.Change(1000); // race condition
            Assert.IsFalse(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted); // race condition
            ThreadingHelper.SpinWaitUntil(() => timeout.IsCompleted);
            Assert.Greater((value[0] - now).TotalMilliseconds, 100);
        }

        [Test]
        public static void TimeoutConstructorCanceledToken()
        {
            var value = new int[] { 0 };
            var token = new CancellationToken(true);
            var timeout = new Timeout(() => value[0] = 1, 0, token);
            Assert.IsTrue(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted);
            Assert.AreEqual(0, value[0]);
        }

        [Test]
        public static void TimeoutConstructorCancellationToken()
        {
            var value = new int[] { 0 };
            var timeout = new Timeout(() => value[0] = 1, 0, CancellationToken.None);
            Assert.IsFalse(timeout.IsCanceled);
        }

        [Test]
        public static void TimeoutConstructorZeroDueTime()
        {
            var value = new int[] { 0 };
            var timeout = new Timeout(() => value[0] = 1, 0);
            ThreadingHelper.SpinWaitUntil(() => timeout.IsCompleted);
            Assert.AreEqual(1, value[0]);
        }

        [Test]
        public static void TimeoutFinishAndChange()
        {
            var value = new[] { 0 };
            var timeout = new Timeout(() => value[0] = 1, 100);
            Assert.IsFalse(timeout.IsCanceled);
            Assert.IsFalse(timeout.IsCompleted); // race condition
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
            var now = DateTime.Now;
            var value = new DateTime[] { now };
            var timeout = new Timeout(() => value[0] = DateTime.Now, 500);
            var remaining = timeout.CheckRemaining();
            Assert.IsFalse(timeout.IsCompleted);
            Assert.LessOrEqual(remaining, 500);
            Thread.Sleep(1);
            var newremaining = timeout.CheckRemaining();
            Assert.LessOrEqual(newremaining, remaining);
            Assert.GreaterOrEqual(remaining, 0);
            Assert.GreaterOrEqual(newremaining, 0);
        }
    }
}