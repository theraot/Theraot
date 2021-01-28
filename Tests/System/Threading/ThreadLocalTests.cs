//
// ThreadLazyTests.cs
//
// Author:
//       Jérémie "Garuma" Laval <jeremie.laval@gmail.com>
//
// Copyright (c) 2009 Jérémie "Garuma" Laval
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NUnit.Framework;
using System;
using System.Threading;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public sealed class ThreadLocalTests : IDisposable
    {
        private int _nTimes;
        private ThreadLocal<int> _threadLocal;

        ~ThreadLocalTests()
        {
            Dispose(false);
        }

        [Test]
        public void DefaultThreadLocalInitTest()
        {
            using (var local = new ThreadLocal<DateTime>())
            {
                using (var local2 = new ThreadLocal<object>())
                {
                    Assert.AreEqual(default(DateTime), local.Value);
                    Assert.AreEqual(default, local2.Value);
                }
            }
        }

        [TearDown]
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        [Test]
        public void DisposedOnIsValueCreatedTest()
        {
            Assert.Throws<ObjectDisposedException>
            (
                () =>
                {
                    var tl = new ThreadLocal<int>();
                    tl.Dispose();
                    GC.KeepAlive(tl.IsValueCreated);
                }
            );
        }

        [Test]
        public void DisposedOnValueTest()
        {
            Assert.Throws<ObjectDisposedException>
            (
                () =>
                {
                    var tl = new ThreadLocal<int>();
                    tl.Dispose();
                    GC.KeepAlive(tl.Value);
                }
            );
        }

        [Test]
        [Category("NotDotNet")] // Running this test against .NET 4.0 or .NET 4.5 fails
        [Ignore("Not working")]
        public void InitializeThrowingTest()
        {
            var callTime = 0;
            _threadLocal = new ThreadLocal<int>
            (
                () =>
                {
                    Interlocked.Increment(ref callTime);
                    throw new ApplicationException("foo");
                }
            );

            Exception exception = null;

            try
            {
                GC.KeepAlive(_threadLocal.Value);
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
                GC.KeepAlive(_threadLocal.Value);
            }
            catch (Exception e)
            {
                exception = e;
            }

            Assert.IsNotNull(exception, "#4");
            Assert.That(exception, Is.TypeOf(typeof(ApplicationException)), "#5");
            Assert.AreEqual(1, callTime, "#6");
        }

#if LESSTHAN_NET40

        [Test]
        [Category("NotDotNet")] // nUnit results in stack overflow
        [Ignore("Not working")]
        public void MultipleReferenceToValueTest()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    _threadLocal = new ThreadLocal<int>(() => _threadLocal.Value + 1);
                    GC.KeepAlive(_threadLocal.Value);
                }
            );
        }

#endif

        [Test]
        public void PerThreadException()
        {
            var callTime = 0;
            _threadLocal = new ThreadLocal<int>
            (
                () =>
                {
                    if (callTime == 1)
                    {
                        throw new ApplicationException("foo");
                    }

                    Interlocked.Increment(ref callTime);
                    return 43;
                }
            );

            Exception exception = null;

            var foo = _threadLocal.Value;
            var threadValueCreated = false;
            Assert.AreEqual(43, foo, "#3");
            var t = new Thread
            (
                _ =>
                {
                    try
                    {
                        GC.KeepAlive(_threadLocal.Value);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }

                    // should be false and not throw
                    threadValueCreated = _threadLocal.IsValueCreated;
                }
            );
            t.Start();
            t.Join();
            Assert.AreEqual(false, threadValueCreated, "#4");
            Assert.IsNotNull(exception, "#5");
            Assert.That(exception, Is.TypeOf(typeof(ApplicationException)), "#6");
        }

        [SetUp]
        public void Setup()
        {
            _nTimes = 0;
            _threadLocal = new ThreadLocal<int>
            (
                () =>
                {
                    Interlocked.Increment(ref _nTimes);
                    return 42;
                }
            );
        }

        [Test]
        public void SingleThreadTest()
        {
            AssertThreadLocal();
        }

        [Test]
        public void ThreadedTest()
        {
            AssertThreadLocal();

            var t = new Thread
            (
                _ =>
                {
                    Interlocked.Decrement(ref _nTimes);
                    AssertThreadLocal();
                }
            );
            t.Start();
            t.Join();
        }

        private void AssertThreadLocal()
        {
            Assert.IsFalse(_threadLocal.IsValueCreated, "#1");
            Assert.AreEqual(42, _threadLocal.Value, "#2");
            Assert.IsTrue(_threadLocal.IsValueCreated, "#3");
            Assert.AreEqual(42, _threadLocal.Value, "#4");
            Assert.AreEqual(1, _nTimes, "#5");
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _threadLocal.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}