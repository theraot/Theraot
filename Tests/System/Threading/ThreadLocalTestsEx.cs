using NUnit.Framework;
using System;
using System.Threading;

namespace MonoTests.System.Threading
{
    [TestFixtureAttribute]
    public class ThreadLocalTestsEx
    {
        [Test]
        public void ThreadLocalDoesNotUseTheDefaultConstructor()
        {
            using (var local = new ThreadLocal<Random>())
            {
                Assert.AreEqual(null, local.Value);
            }
        }

        [Test]
        [Category("NotDotNet")] // Running this test against .NET 4.0 fails
        public void InitializeThrowingTest()
        {
            if (Environment.Version.Major >= 4)
            {
                throw new NotSupportedException("Not available in .NET 4.0");
            }
            TestException(false);
            TestException(true);
        }

        [Test]
        [Category("NotDotNet")] // nunit results in stack overflow
        public void MultipleReferenceToValueTest()
        {
            if (Environment.Version.Major >= 4)
            {
                throw new NotSupportedException("Results in stack overflow - blame Microsoft");
            }
            Assert.Throws
                (
                    typeof(InvalidOperationException),
                    () =>
                    {
                        ThreadLocal<int>[] threadLocal = {null};
                        using (threadLocal[0] = new ThreadLocal<int>(() => threadLocal[0] != null ? threadLocal[0].Value + 1 : 0, false))
                        {
                            GC.KeepAlive(threadLocal[0].Value);
                        }
                    }
                );
            Assert.Throws
                (
                    typeof(InvalidOperationException),
                    () =>
                    {
                        ThreadLocal<int>[] threadLocal = {null};
                        using (threadLocal[0] = new ThreadLocal<int>(() => threadLocal[0] != null ? threadLocal[0].Value + 1 : 0, true))
                        {
                            GC.KeepAlive(threadLocal[0].Value);
                        }
                    }
                );
        }
        private void TestException(bool tracking)
        {
            int callTime = 0;
            using
            (
                var threadLocal = new ThreadLocal<int>
                (
                    () =>
                    {
                        Interlocked.Increment(ref callTime);
                        throw new ApplicationException("foo");
                    },
                    tracking
                )
            )
            {

                Exception exception = null;

                try
                {
                    GC.KeepAlive(threadLocal.Value);
                }
                catch (Exception e)
                {
                    exception = e;
                }

                Assert.IsNotNull(exception, "#1");
                Assert.That(exception, Is.TypeOf(typeof (ApplicationException)), "#2");
                Assert.AreEqual(1, callTime, "#3");

                exception = null;

                try
                {
                    GC.KeepAlive(threadLocal.Value);
                }
                catch (Exception e)
                {
                    exception = e;
                }

                Assert.IsNotNull(exception, "#4");
                Assert.That(exception, Is.TypeOf(typeof (ApplicationException)), "#5");
                Assert.AreEqual(1, callTime, "#6");
            }
        }
    }
}