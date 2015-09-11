using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Threading;

namespace Tests.Theraot.Threading
{
    [TestFixture]
    internal class ReentryGuardTests
    {

        [Test]
        public void SingleThreadReentry()
        {
            var guard = new ReentryGuard();
            int count = 0;
            Action action = null;
            action = () =>
            {
                if (count < 255)
                {
                    count++;
                    guard.Execute(action);
                }
            };
            guard.Execute(action);
            Assert.AreEqual(255, count);
        }

        [Test]
        public void ThreadedReentry()
        {
            var guard = new ReentryGuard();
            int count = 0;
            Action action = null;
            action = () =>
            {
                if (count < 255)
                {
                    count++;
                    guard.Execute(action);
                }
            };
            var a = new Thread(() => guard.Execute(action));
            var b = new Thread(() => guard.Execute(action));
            a.Start();
            b.Start();
            a.Join();
            b.Join();
            Assert.AreEqual(255, count);
        }
    }
}
