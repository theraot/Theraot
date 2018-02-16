//
// CancellationTokenSourceTest.cs
//
// Authors:
//       Marek Safar (marek.safar@gmail.com)
//       Jeremie Laval (jeremie.laval@gmail.com)
//
// Copyright 2011 Xamarin, Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using NUnit.Framework;
using System;
using System.Threading;

namespace MonoTests.System.Threading
{
    [TestFixture]
    public partial class CancellationTokenSourceTest
    {
        [Test]
        public void Cancel()
        {
            using (var cts = new CancellationTokenSource())
            {
                int[] called = { 0 };
                cts.Token.Register(l =>
                {
                    Assert.AreEqual("v", l);
                    ++called[0];
                }, "v");
                cts.Cancel();
                Assert.AreEqual(1, called[0], "#1");

                called[0] = 0;
                cts.Token.Register(() => called[0] += 12);
                cts.Cancel();
                Assert.AreEqual(12, called[0], "#2");
            }
        }

        [Test]
        public void Cancel_ExceptionOrder()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("1");
                });
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("2");
                });
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("3");
                });

                try
                {
                    cts.Cancel();
                }
                catch (AggregateException e)
                {
                    Assert.AreEqual(3, e.InnerExceptions.Count, "#2");
                    Assert.AreEqual("3", e.InnerExceptions[0].Message, "#3");
                    Assert.AreEqual("2", e.InnerExceptions[1].Message, "#4");
                    Assert.AreEqual("1", e.InnerExceptions[2].Message, "#5");
                }
            }
        }

        [Test]
        public void Cancel_MultipleException_Recursive()
        {
            using (var cts = new CancellationTokenSource())
            {
                var c = cts.Token;
                c.Register(cts.Cancel);

                c.Register(() =>
                {
                    throw new ApplicationException();
                });

                c.Register(() =>
                {
                    throw new NotSupportedException();
                });

                try
                {
                    cts.Cancel(false);
                    Assert.Fail("#1");
                }
                catch (AggregateException e)
                {
                    Assert.AreEqual(2, e.InnerExceptions.Count, "#2");
                }
            }
        }

        [Test]
        public void Cancel_MultipleExceptions()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("1");
                });
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("2");
                });
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("3");
                });

                try
                {
                    cts.Cancel();
                    Assert.Fail("#1");
                }
                catch (AggregateException e)
                {
                    Assert.AreEqual(3, e.InnerExceptions.Count, "#2");
                }

                cts.Cancel();

                try
                {
                    cts.Token.Register(() =>
                    {
                        throw new ApplicationException("1");
                    });
                    Assert.Fail("#11");
                }
                catch (ApplicationException ex)
                {
                    GC.KeepAlive(ex);
                }

                cts.Cancel();
            }
        }

        [Test]
        public void Cancel_MultipleExceptionsFirstThrows()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("1");
                });
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("2");
                });
                cts.Token.Register(() =>
                {
                    throw new ApplicationException("3");
                });

                try
                {
                    cts.Cancel(true);
                    Assert.Fail("#1");
                }
                catch (ApplicationException ex)
                {
                    GC.KeepAlive(ex);
                }

                cts.Cancel();
            }
        }

        [Test]
        public void Cancel_NoRegistration()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();
            }
        }

        [Test]
        public void Cancel_Order()
        {
            using (var cts = new CancellationTokenSource())
            {
                var current = 0;
                Action<object> a = x =>
                {
                    Assert.AreEqual(current, x);
                    current++;
                };

                cts.Token.Register(a, 2);
                cts.Token.Register(a, 1);
                cts.Token.Register(a, 0);
                cts.Cancel();
            }
        }

        [Test]
        public void Cancel_SingleException()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Token.Register(() =>
                {
                    throw new ApplicationException();
                });
                try
                {
                    cts.Cancel();
                    Assert.Fail("#1");
                }
                catch (AggregateException e)
                {
                    Assert.AreEqual(1, e.InnerExceptions.Count, "#2");
                }

                cts.Cancel();
            }
        }

        [Test]
        public void CancelLinkedTokenSource()
        {
            using (var cts = new CancellationTokenSource())
            {
                var canceled = false;
                cts.Token.Register(() => canceled = true);

                using (CancellationTokenSource.CreateLinkedTokenSource(cts.Token))
                {
                    // Empty
                }

                Assert.IsFalse(canceled, "#1");
                Assert.IsFalse(cts.IsCancellationRequested, "#2");

                cts.Cancel();

                Assert.IsTrue(canceled, "#3");
            }
        }

        [Test]
        public void CancelWithDispose()
        {
            using (var cts = new CancellationTokenSource())
            {
                var c = cts.Token;
                c.Register(cts.Dispose);

                var called = 0;
                c.Register(() => called++);

                cts.Cancel();
                Assert.AreEqual(1, called, "#1");
            }
        }

        [Test]
        public void ConcurrentCancelLinkedTokenSourceWhileDisposing() // TODO: Review
        {
            ParallelTestHelper.Repeat(delegate
            {
                using (var src = new CancellationTokenSource())
                {
                    var linked = CancellationTokenSource.CreateLinkedTokenSource(src.Token);
                    using (var cntd = new CountdownEvent(2))
                    {
                        var t1 = new Thread(() =>
                        {
                            if (!cntd.Signal())
                            {
                                cntd.Wait(200);
                            }

                            src.Cancel();
                        });
                        var t2 = new Thread(() =>
                        {
                            if (!cntd.Signal())
                            {
                                cntd.Wait(200);
                            }

                            linked.Dispose();
                        });

                        t1.Start();
                        t2.Start();
                        t1.Join(500);
                        t2.Join(500);
                    }
                }
            }, 500);
        }

        [Test]
        public void CreateLinkedTokenSource()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();

                var linked = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
                Assert.IsTrue(linked.IsCancellationRequested, "#1");

                linked = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
                Assert.IsFalse(linked.IsCancellationRequested, "#2");
            }
        }

        [Test]
        public void CreateLinkedTokenSource_InvalidArguments()
        {
            using (var cts = new CancellationTokenSource())
            {
                GC.KeepAlive(cts.Token);

                try
                {
                    CancellationTokenSource.CreateLinkedTokenSource(null);
                    Assert.Fail("#1");
                }
                catch (ArgumentNullException ex)
                {
                    GC.KeepAlive(ex);
                }

                try
                {
                    CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken[0]);
                    Assert.Fail("#2");
                }
                catch (ArgumentException ex)
                {
                    GC.KeepAlive(ex);
                }
            }
        }

        [Test]
        public void Dispose()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            cts.Dispose();
            cts.Dispose();
            GC.KeepAlive(cts.IsCancellationRequested);
            token.ThrowIfCancellationRequested();

            try
            {
                cts.Cancel();
                Assert.Fail("#1");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                GC.KeepAlive(cts.Token);
                Assert.Fail("#2");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                token.Register(() =>
                {
                });
                Assert.Fail("#3");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                GC.KeepAlive(token.WaitHandle);
                Assert.Fail("#4");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                CancellationTokenSource.CreateLinkedTokenSource(token);
                Assert.Fail("#5");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

#if NET20 || NET30 || NET35 || NET_45
            try
            {
                cts.CancelAfter(1);
                Assert.Fail("#6");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }
#endif
        }

        [Test]
        public void DisposeAfterRegistrationTest()
        {
            var source = new CancellationTokenSource();
            var ran = false;
            var req = source.Token.Register(() => ran = true);
            source.Dispose();
            req.Dispose();
            Assert.IsFalse(ran);
        }

        [Test]
        public void ReEntrantRegistrationTest()
        {
            var unregister = false;
            var register = false;
            using (var source = new CancellationTokenSource())
            {
                var token = source.Token;

                Console.WriteLine("Test1");
                var reg = token.Register(() => unregister = true);
                token.Register(reg.Dispose);
                token.Register(() =>
                {
                    Console.WriteLine("Gnyah");
                    token.Register(() => register = true);
                });
                source.Cancel();

                Assert.IsFalse(unregister);
                Assert.IsTrue(register);
            }
        }

        [Test]
        public void RegisterThenDispose()
        {
            var cts1 = new CancellationTokenSource();
            var reg1 = cts1.Token.Register(() =>
            {
                throw new ApplicationException();
            });

            var cts2 = new CancellationTokenSource();
            cts2.Token.Register(() =>
            {
                throw new ApplicationException();
            });

            Assert.AreNotEqual(cts1, cts2, "#1");
            Assert.AreNotSame(cts1, cts2, "#2");

            reg1.Dispose();
            cts1.Cancel();

            try
            {
                cts2.Cancel();
                Assert.Fail("#3");
            }
            catch (AggregateException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void Token()
        {
            using (var cts = new CancellationTokenSource())
            {
                Assert.IsTrue(cts.Token.CanBeCanceled, "#1");
                Assert.IsFalse(cts.Token.IsCancellationRequested, "#2");
                Assert.IsNotNull(cts.Token.WaitHandle, "#3");
            }
        }

        //[Test]
        //public void RegisterWhileCancelling()
        //{
        //    cts.Token.Register(() =>
        //    {
        //        Assert.IsTrue(cts.IsCancellationRequested, "#10");
        //        Assert.IsTrue(cts.Token.WaitHandle.WaitOne(0), "#11");
        //        mre2.Set();
        //        mre.WaitOne(3000);
        //        called += 11;
        //    });

        //    Assert.IsTrue(mre2.WaitOne(1000), "#0");
        //    cts.Token.Register(() => { called++; });
        //    Assert.AreEqual(1, called, "#1");
        //    Assert.IsFalse(t.IsCompleted, "#2");

        //    mre.Set();
        //    Assert.IsTrue(t.Wait(1000), "#3");
        //    Assert.AreEqual(12, called, "#4");
        //}
    }

    public partial class CancellationTokenSourceTest
    {
#if NET20 || NET30 || NET35 || NET_45

        [Test]
        public void CancelAfter() // TODO: Review
        {
            for (var index = 0; index < 10000; index++)
            {
                var called = 0;
                using (var cts = new CancellationTokenSource())
                {
                    cts.Token.Register(() => called++);
                    cts.CancelAfter(20);
                    Thread.Sleep(50);
                    Assert.AreEqual(1, called, "#1");
                }
            }
        }

        [Test]
        public void CancelAfter_Disposed()
        {
            var called = 0;
            var cts = new CancellationTokenSource();
            cts.Token.Register(() => called++);
            cts.CancelAfter(50);
            cts.Dispose();
            Thread.Sleep(100);
            Assert.AreEqual(0, called, "#1");
        }

        [Test]
        public void CancelAfter_Invalid()
        {
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    cts.CancelAfter(-9);
                    Assert.Fail("#1");
                }
                catch (ArgumentException ex)
                {
                    GC.KeepAlive(ex);
                }
            }
        }

        [Test]
        public void Ctor_Invalid()
        {
            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(-4))
                {
                    GC.KeepAlive(cancellationTokenSource);
                }
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void Ctor_Timeout()
        {
            var called = 0;
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(20)))
            {
                cts.Token.Register(() => called++);
                Thread.Sleep(50);
                Assert.AreEqual(1, called, "#1");
            }
        }

        [Test]
        public void DisposeEx()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            cts.Dispose();
            cts.Dispose();
            GC.KeepAlive(cts.IsCancellationRequested);
            token.ThrowIfCancellationRequested();

            try
            {
                cts.Cancel();
                Assert.Fail("#1");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                GC.KeepAlive(cts.Token);
                Assert.Fail("#2");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                token.Register(() =>
                {
                });
                Assert.Fail("#3");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                GC.KeepAlive(token.WaitHandle);
                Assert.Fail("#4");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                CancellationTokenSource.CreateLinkedTokenSource(token);
                Assert.Fail("#5");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }
            try
            {
                cts.CancelAfter(1);
                Assert.Fail("#6");
            }
            catch (ObjectDisposedException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void DisposeRace()
        {
            for (var i = 0; i < 1000; ++i)
            {
                using (var c1 = new CancellationTokenSource())
                {
                    GC.KeepAlive(c1.Token.WaitHandle);
                    c1.CancelAfter(1);
                    Thread.Sleep(1);
                }
            }
        }

#endif
    }
}