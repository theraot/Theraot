using NUnit.Framework;
using System;
using System.Threading;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public partial class ThreadLocalTestsEx
    {
        [Test]
        public void ThreadLocalDoesNotUseTheDefaultConstructor()
        {
            using (var local = new ThreadLocal<Random>())
            {
                Assert.AreEqual(null, local.Value);
            }
        }
    }

    [TestFixture]
    public partial class ThreadLocalTestsEx
    {
        private static void LaunchAndWaitThread(ThreadLocal<int> threadLocal)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    GC.KeepAlive(threadLocal.Value);
                }
                catch (Exception exc)
                {
                    Theraot.No.Op(exc);
                }
            });
            thread.Start();
            thread.Join();
        }
    }

    public partial class ThreadLocalTestsEx
    {
#if LESSSTHAN_NET40
        [Test]
        [Category("NotDotNet")] // Running this test against .NET 4.0 fails
        public void InitializeThrowingTest()
        {
            TestException(false);
            TestException(true);
        }

        [Test]
        [Category("NotDotNet")] // nunit results in stack overflow
        public void MultipleReferenceToValueTest()
        {
            Assert.Throws(
                typeof(InvalidOperationException),
                () =>
                {
                    ThreadLocal<int>[] threadLocal = { null };
                    using (threadLocal[0] = new ThreadLocal<int>(() => threadLocal[0] != null ? threadLocal[0].Value + 1 : 0, false))
                    {
                        GC.KeepAlive(threadLocal[0].Value);
                    }
                }
            );
            Assert.Throws(
                typeof(InvalidOperationException),
                () =>
                {
                    ThreadLocal<int>[] threadLocal = { null };
                    using (threadLocal[0] = new ThreadLocal<int>(() => threadLocal[0] != null ? threadLocal[0].Value + 1 : 0, true))
                    {
                        GC.KeepAlive(threadLocal[0].Value);
                    }
                }
            );
        }

        [Test]
        public void TestValues()
        {
            var count = 0;
            var threadLocal = new ThreadLocal<int>(() => count++, true);
            using (threadLocal)
            {
                LaunchAndWaitThread(threadLocal);
                LaunchAndWaitThread(threadLocal);
                LaunchAndWaitThread(threadLocal);
                var expected = new List<int> { 0, 1, 2 };
                foreach (var item in threadLocal.Values)
                {
                    Assert.IsTrue(expected.Remove(item));
                }
                Assert.AreEqual(expected.Count, 0);
            }
            using (var tlocal = new ThreadLocal<int>(() => 0, false))
            {
                Assert.Throws
                (
                    typeof(InvalidOperationException),
                    () => GC.KeepAlive(tlocal.Values));
            }
        }

        [Test]
        public void TestValuesWithExceptions()
        {
            var count = 0;
            var threadLocal = new ThreadLocal<int>(() =>
            {
                count++;
                throw new Exception("Burn!");
            }, true);
            using (threadLocal)
            {
                LaunchAndWaitThread(threadLocal);
                LaunchAndWaitThread(threadLocal);
                LaunchAndWaitThread(threadLocal);
                Assert.AreEqual(threadLocal.Values.Count, 0);
            }
            Assert.AreEqual(count, 3);
        }

        [Test]
        public void ValuesIsNewCopy()
        {
            var threadLocal = new ThreadLocal<int>(() => 0, true);
            using (threadLocal)
            {
                LaunchAndWaitThread(threadLocal);
                var values = threadLocal.Values;
                Assert.IsFalse(ReferenceEquals(values, threadLocal.Values));
                values.Add(5);
                Assert.AreEqual(threadLocal.Values.Count, 1);
            }
        }

        private static void TestException(bool tracking)
        {
            var callTime = 0;
            using (var threadLocal = new ThreadLocal<int>(() =>
            {
                Interlocked.Increment(ref callTime);
                throw new ApplicationException("foo");
            }, tracking))
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
                Assert.That(exception, Is.TypeOf(typeof(ApplicationException)), "#2");
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
                Assert.That(exception, Is.TypeOf(typeof(ApplicationException)), "#5");
                // MSDN says on ThreadLocal.Value
                //
                // If this instance was not previously initialized for the current thread, accessing Value will attempt to
                // initialize it. If an initialization function was supplied during the construction, that initialization
                // will happen by invoking the function to retrieve the initial value for Value. Otherwise, the default
                // value of will be used. If an exception is thrown, that exception is cached and thrown on each subsequent
                // access of the property.
                //
                // That means that even though we did access ThreadLocal.Value twice, the value factory should have been
                // executed only once.... because the value factory we used throws an exception, and that should have been
                // cached and thrown instead of calling the value factory again...
                //
                // .NET 4.0 and .NET 4.5 are not doing that, instead they call the value factory again
                Assert.AreEqual(1, callTime, "#6");
            }
        }
#endif
    }
}