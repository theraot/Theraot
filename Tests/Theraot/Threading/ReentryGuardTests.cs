#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System.Threading;
using Theraot.Collections.ThreadSafe;
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
            var count = 0;

            void Action()
            {
                if (count >= 255)
                {
                    return;
                }

                count++;
                guard.Execute(Action);
            }

            guard.Execute(Action);
            Assert.AreEqual(255, count);
        }

        [Test]
        public void ThreadedReentry()
        {
            var guard = new ReentryGuard();
            var threads = new ThreadSafeSet<Thread>();
            var count = 0;
            var fail = 0;

            void Action()
            {
                if (threads.Add(Thread.CurrentThread) == false)
                {
                    Interlocked.Increment(ref fail);
                }

                if (count < 255)
                {
                    count++;
                    guard.Execute(Action);
                }

                threads.Remove(Thread.CurrentThread);
            }

            var a = new Thread(() => guard.Execute(Action));
            var b = new Thread(() => guard.Execute(Action));
            a.Start();
            b.Start();
            a.Join();
            b.Join();
            Assert.AreEqual(0, fail);
            Assert.AreEqual(255, count);
        }
    }
}