#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System.Threading;
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

            var a = new Thread(() => guard.Execute(Action));
            var b = new Thread(() => guard.Execute(Action));
            a.Start();
            b.Start();
            a.Join();
            b.Join();
            Assert.AreEqual(255, count);
        }
    }
}