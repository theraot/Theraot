// ManualResetEventSlimTests.cs
//
// Authors:
//       Marek Safar (marek.safar@gmail.com)
//       Jeremie Laval (jeremie.laval@gmail.com)
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
// Copyright (c) 2012 Xamarin, Inc (http://www.xamarin.com)
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
//
//

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Tests.Helpers;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public sealed class ManualResetEventSlimTests : IDisposable
    {
        private ManualResetEventSlim _mre;

        ~ManualResetEventSlimTests()
        {
            Dispose(false);
        }

        [Test]
        public void ConstructorDefaults()
        {
            Assert.IsFalse(_mre.IsSet, "#1");
#if GREATERTHAN_NETCOREAPP11
            // .NET Core has different defaults
            // This means we should not rely on these defaults for portable code
            // Specify the SpinCount in the constructor if you need it
            Assert.AreEqual(35, _mre.SpinCount, "#2");
#else
            Assert.AreEqual(10, _mre.SpinCount, "#2");
#endif
        }

        [Test]
        public void ConstructorInvalid()
        {
            try
            {
                using (new ManualResetEventSlim(true, -1))
                {
                    Assert.Fail("#1");
                }
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                using (new ManualResetEventSlim(true, 2048))
                {
                    Assert.Fail("#2");
                }
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [TearDown]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [Test]
        public void DisposeDouble()
        {
            var mre = new ManualResetEventSlim();
            mre.Dispose();
            mre.Dispose();
        }

        [Test]
        public void DisposeTest()
        {
            var mre = new ManualResetEventSlim(false);
            mre.Dispose();
            Assert.IsFalse(mre.IsSet, "#0a");

            try
            {
                mre.Reset();
                Assert.Fail("#1");
            }
            catch (ObjectDisposedException ex)
            {
                Theraot.No.Op(ex);
            }

            mre.Set();

            try
            {
                mre.Wait(0);
                Assert.Fail("#3");
            }
            catch (ObjectDisposedException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                GC.KeepAlive(mre.WaitHandle);
                Assert.Fail("#4");
            }
            catch (ObjectDisposedException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        public void IsSetTestCase()
        {
            Assert.IsFalse(_mre.IsSet, "#1");
            _mre.Set();
            Assert.IsTrue(_mre.IsSet, "#2");
            _mre.Reset();
            Assert.IsFalse(_mre.IsSet, "#3");
        }

        [Test]
        [Category("LongRunning")]
        public void SetAfterDisposeTest()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var countdownEvent = new CountdownEvent[2];
                    using (countdownEvent[0] = new CountdownEvent(2))
                    {
                        using (countdownEvent[1] = new CountdownEvent(2))
                        {
                            Exception exception = null, setting = null;

                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                try
                                {
                                    countdownEvent[0].Signal();
                                    countdownEvent[0].Wait(1000);
                                    _mre.Dispose();
                                }
                                catch (Exception e)
                                {
                                    exception = e;
                                }
                                countdownEvent[1].Signal();
                            });
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                try
                                {
                                    countdownEvent[0].Signal();
                                    countdownEvent[0].Wait(1000);
                                    _mre.Set();
                                }
                                catch (Exception e)
                                {
                                    setting = e;
                                }
                                countdownEvent[1].Signal();
                            });

                            countdownEvent[1].Wait();
                            Assert.IsNull(exception, "#1");
                            Assert.IsNull(setting, "#2");

                            countdownEvent[0].Dispose();
                            countdownEvent[1].Dispose();
                        }
                    }
                }
            );
        }

        [SetUp]
        public void Setup()
        {
            _mre = new ManualResetEventSlim();
        }

        [Test]
        public void WaitAfterDisposeTest()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _mre.Dispose();
                _mre.Wait();
            });
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void WaitDisposeWithCancel()
        {
            var token = new CancellationTokenSource[1];
            using (token[0] = new CancellationTokenSource())
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Thread.Sleep(10);
                    _mre.Dispose();
                    token[0].Cancel();
                });

                try
                {
                    _mre.Wait(10000, token[0].Token);
                    Assert.Fail("#0");
                }
                catch (OperationCanceledException exception)
                {
                    Theraot.No.Op(exception);
                }
            }
        }

        [Test]
        public void WaitExpired()
        {
            Assert.IsFalse(_mre.Wait(10));
        }

        [Test]
        public void WaitHandleInitialized()
        {
            using (var mre = new ManualResetEventSlim(true))
            {
                Assert.IsTrue(mre.WaitHandle.WaitOne(0), "#1");
                mre.Reset();
                Assert.IsFalse(mre.WaitHandle.WaitOne(0), "#2");
                Assert.AreEqual(mre.WaitHandle, mre.WaitHandle, "#3");
            }
        }

        [Test]
        public void WaitHandleNotInitialized()
        {
            using (var mre = new ManualResetEventSlim(false))
            {
                Assert.IsFalse(mre.WaitHandle.WaitOne(0), "#1");
                mre.Set();
                Assert.IsTrue(mre.WaitHandle.WaitOne(0), "#2");
            }
        }

        [Test]
        [Category("RaceCondition")] // This test creates a race condition
        public void WaitSetConcurrent()
        {
            var manualResetEvents = new List<ManualResetEventSlim>();
            for (var i = 0; i < 10000; ++i)
            {
                var mre = new ManualResetEventSlim();
                manualResetEvents.Add(mre);

                var b = true;

                ThreadPool.QueueUserWorkItem(state => mre.Set());

                ThreadPool.QueueUserWorkItem(state => b &= mre.Wait(1000));

                Assert.IsTrue(mre.Wait(1000), $"{i}");
                Assert.IsTrue(b, $"{i}");
            }
            foreach (var mre in manualResetEvents)
            {
                mre.Dispose();
            }
        }

        [Test]
        public void WaitTest()
        {
            var count = 0;
            var s = false;

            ParallelTestHelper.ParallelStressTest
            (
                () =>
                {
                    if (Interlocked.Increment(ref count) % 2 == 0)
                    {
                        Thread.Sleep(50);
                        for (var i = 0; i < 10; i++)
                        {
                            if (i % 2 == 0)
                            {
                                _mre.Reset();
                            }
                            else
                            {
                                _mre.Set();
                            }
                        }
                    }
                    else
                    {
                        _mre.Wait();
                        s = true;
                    }
                },
                2
            );

            Assert.IsTrue(s, "#1");
            Assert.IsTrue(_mre.IsSet, "#2");
        }

        [Test]
        [Category("LongRunning")]
        public void WaitWithCancellationTokenAndCancel()
        {
            var manualResetEvent = new ManualResetEventSlim[1];
            using (manualResetEvent[0] = new ManualResetEventSlim())
            {
                var cts = new CancellationTokenSource[1];
                using (cts[0] = new CancellationTokenSource())
                {
                    ThreadPool.QueueUserWorkItem
                    (
                        _ =>
                        {
                            Thread.Sleep(1000);
                            cts[0].Cancel();
                        }
                    );
                    try
                    {
                        manualResetEvent[0].Wait(TimeSpan.FromSeconds(10), cts[0].Token);
                        Assert.Fail("Wait did not throw an exception despite cancellation token was cancelled.");
                    }
                    catch (OperationCanceledException ex)
                    {
                        Theraot.No.Op(ex);
                    }
                }
            }
        }

        [Test]
        [Category("LongRunning")]
        public void WaitWithCancellationTokenAndNotImmediateSetTest() // TODO: Review
        {
            var manualResetEvent = new ManualResetEventSlim[1];
            using (manualResetEvent[0] = new ManualResetEventSlim())
            {
                using (var cts = new CancellationTokenSource())
                {
                    ThreadPool.QueueUserWorkItem
                    (
                        _ =>
                        {
                            Thread.Sleep(1000);
                            manualResetEvent[0].Set();
                        }
                    );
                    Assert.IsTrue(manualResetEvent[0].Wait(TimeSpan.FromSeconds(10), cts.Token), "Wait returned false despite event was set.");
                }
            }
        }

        [Test]
        [Category("LongRunning")]
        public void WaitWithCancellationTokenAndTimeout()
        {
            var manualResetEvent = new ManualResetEventSlim[1];
            using (manualResetEvent[0] = new ManualResetEventSlim())
            {
                using (var cts = new CancellationTokenSource())
                {
                    Assert.IsFalse(manualResetEvent[0].Wait(TimeSpan.FromSeconds(1), cts.Token), "Wait returned true despite timeout.");
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mre.Dispose();
            }
        }
    }
}