//
// LazyTest.cs - NUnit Test Cases for Lazy
//
// Author:
//	Zoltan Varga (vargaz@gmail.com)
//
// Copyright (C) 2009 Novell, Inc (http://www.novell.com)
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

#define NET_4_0
#if NET_4_0

using NUnit.Framework;
using System;
using System.Threading;

namespace MonoTests.System
{
    [TestFixture]
    public class LazyTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Null_1()
        {
            var lazy = new Lazy<int>(null);
            GC.KeepAlive(lazy);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Null_2()
        {
            var lazy = new Lazy<int>(null, false);
            GC.KeepAlive(lazy);
        }

        [Test]
        public void IsValueCreated()
        {
            var l1 = new Lazy<int>();

            Assert.IsFalse(l1.IsValueCreated);

            GC.KeepAlive(l1.Value);

            Assert.IsTrue(l1.IsValueCreated);
        }

        [Test]
        public void DefaultCtor()
        {
            var l1 = new Lazy<DefaultCtorClass>();

            var o = l1.Value;
            Assert.AreEqual(5, o.Prop);
        }

        private class DefaultCtorClass
        {
            public DefaultCtorClass()
            {
                Prop = 5;
            }

            public int Prop { get; }
        }

        [Test]
        public void NoDefaultCtor()
        {
            var l1 = new Lazy<NoDefaultCtorClass>();

            try
            {
                GC.KeepAlive(l1.Value);
                Assert.Fail();
            }
            catch (MissingMemberException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        private class NoDefaultCtorClass
        {
            public NoDefaultCtorClass(int i)
            {
                GC.KeepAlive(i);
            }
        }

        [Test]
        public void NotThreadSafe()
        {
            var l1 = new Lazy<int>();

            Assert.AreEqual(0, l1.Value);

            var l2 = new Lazy<int>(() => 42);

            Assert.AreEqual(42, l2.Value);
        }

        private static int _counter;

        [Test]
        public void EnsureSingleThreadSafeExecution()
        {
            _counter = 42;

            var l = new Lazy<int>(() => _counter++, true);

            var monitor = new object();
            var threads = new Thread[10];
            for (var i = 0; i < 10; ++i)
            {
                threads[i] = new Thread(() =>
                {
                    lock (monitor)
                    {
                        Monitor.Wait(monitor); // TODO: Review
                    }
                    GC.KeepAlive(l.Value);
                });
            }
            for (var i = 0; i < 10; ++i)
            {
                threads[i].Start();
            }

            lock (monitor)
            {
                Monitor.PulseAll(monitor);
            }

            Assert.AreEqual(42, l.Value);
        }

        [Test]
        public void InitRecursion()
        {
            var c = new Lazy<DefaultCtorClass>[] { null };
            c[0] = new Lazy<DefaultCtorClass>
            (
                () =>
                {
                    Console.WriteLine(c[0].Value);
                    return null;
                }
            );
            try
            {
                GC.KeepAlive(c[0].Value);
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void ModeNone()
        {
            var fail = new[] { true };
            var lz = new Lazy<int>[] { null };
            lz[0] = new Lazy<int>(() =>
            {
                if (fail[0])
                {
                    throw new Exception();
                }
                return 99;
            }, LazyThreadSafetyMode.None);
            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#1");
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#2");
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }

            fail[0] = false;
            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#3");
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }

            var rec = new[] { true };
            lz[0] = new Lazy<int>(() => rec[0] ? lz[0].Value : 99, LazyThreadSafetyMode.None);

            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#4");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }

            rec[0] = false;
            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#5");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        [Test]
        public void ModePublicationOnly()
        {
            var fail = new[] { true };
            var invoke = new[] { 0 };
            var lz = new Lazy<int>[] { null };
            lz[0] = new Lazy<int>(() =>
            {
                invoke[0]++;
                if (fail[0])
                {
                    throw new Exception();
                }
                return 99;
            }, LazyThreadSafetyMode.PublicationOnly);

            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#1");
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }

            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#2");
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }

            Assert.AreEqual(2, invoke[0], "#3");
            fail[0] = false;
            Assert.AreEqual(99, lz[0].Value, "#4");
            Assert.AreEqual(3, invoke[0], "#5");

            invoke[0] = 0;
            var rec = true;
            lz[0] = new Lazy<int>(() =>
            {
                invoke[0]++;
                var r = rec;
                rec = false;
                return r ? lz[0].Value : 88;
            }, LazyThreadSafetyMode.PublicationOnly);

            Assert.AreEqual(88, lz[0].Value, "#6");
            Assert.AreEqual(2, invoke[0], "#7");
        }

        [Test]
        public void ModeExecutionAndPublication()
        {
            var invoke = 0;
            var fail = new[] { true };
            var lz = new Lazy<int>[] { null };
            lz[0] = new Lazy<int>(() =>
            {
                ++invoke;
                if (fail[0])
                {
                    throw new Exception();
                }
                return 99;
            }, LazyThreadSafetyMode.ExecutionAndPublication);

            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#1");
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }
            Assert.AreEqual(1, invoke, "#2");

            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#3");
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }
            Assert.AreEqual(1, invoke, "#4");

            fail[0] = false;
            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#5");
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }
            Assert.AreEqual(1, invoke, "#6");

            var rec = new[] { true };
            lz[0] = new Lazy<int>(() => rec[0] ? lz[0].Value : 99, LazyThreadSafetyMode.ExecutionAndPublication);

            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#7");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }

            rec[0] = false;
            try
            {
                GC.KeepAlive(lz[0].Value);
                Assert.Fail("#8");
            }
            catch (InvalidOperationException ex)
            {
                GC.KeepAlive(ex);
            }
        }

        private static int Return22()
        {
            return 22;
        }

        [Test]
        public void Trivial_Lazy()
        {
            var x = new Lazy<int>(Return22, false);
            Assert.AreEqual(22, x.Value, "#1");
        }

        [Test]
        public void ConcurrentInitialization()
        {
            using (var init = new AutoResetEvent(false))
            {
                using (var e1Set = new AutoResetEvent(false))
                {
                    var lazy = new Lazy<string>(() =>
                    {
                        init.Set();
                        Thread.Sleep(10);
                        throw new ApplicationException();
                    });

                    Exception e1 = null;
                    var thread = new Thread(() =>
                    {
                        try
                        {
                            GC.KeepAlive(lazy.Value);
                        }
                        catch (Exception ex)
                        {
                            e1 = ex;
                            e1Set.Set();
                        }
                    });
                    thread.Start();

                    Assert.IsTrue(init.WaitOne(3000), "#1");

                    Exception e2 = null;
                    try
                    {
                        GC.KeepAlive(lazy.Value);
                    }
                    catch (Exception ex)
                    {
                        e2 = ex;
                    }

                    Exception e3 = null;
                    try
                    {
                        GC.KeepAlive(lazy.Value);
                    }
                    catch (Exception ex)
                    {
                        e3 = ex;
                    }

                    Assert.IsTrue(e1Set.WaitOne(3000), "#2");
                    Assert.AreSame(e1, e2, "#3");
                    Assert.AreSame(e1, e3, "#4");
                }
            }
        }
    }
}

#endif