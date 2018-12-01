#if !NET40

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public class ThreadLocalTestsEx
    {
#if NET20 || NET30 || NET35 || NET45

        [Test]
        [Category("NotDotNet")] // Running this test against .NET 4.0 fails
        public void InitializeThrowingTest()
        {
            TestException(false);
            TestException(true);
        }

#endif

#if NET20 || NET30 || NET35 || NET45

        [Test]
        [Category("NotDotNet")] // nunit results in stack overflow
        [Ignore]
        public void MultipleReferenceToValueTest()
        {
            if (Environment.Version.Major >= 4)
            {
                throw new NotSupportedException("Results in stack overflow - blame Microsoft");
            }
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

#endif

#if NET20 || NET30 || NET35 || NET45

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

#endif

#if NET20 || NET30 || NET35 || NET45

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

#endif

        [Test]
        public void ThreadLocalDoesNotUseTheDefaultConstructor()
        {
            using (var local = new ThreadLocal<Random>())
            {
                Assert.AreEqual(null, local.Value);
            }
        }

#if NET20 || NET30 || NET35 || NET45

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

#endif

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
                    // Pokemon
                    GC.KeepAlive(exc);
                }
            });
            thread.Start();
            thread.Join();
        }

#if NET20 || NET30 || NET35 || NET45

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
                Assert.AreEqual(1, callTime, "#6");
            }
        }

#endif
    }
}

#endif