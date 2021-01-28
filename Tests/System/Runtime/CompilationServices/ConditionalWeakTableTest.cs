//
// ConditionalWeakTableTest.cs
//
// Author:
//	Rodrigo Kumpera   <rkumpera@novell.com>
//
// Copyright (C) 2010 Novell, Inc (http://www.novell.com)
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace MonoTests.System.Runtime.CompilerServices
{
    [TestFixture]
    public partial class ConditionalWeakTableTest
    {
        private static readonly object _lock1 = new();

        private static readonly object _lock2 = new();

        private static int _reachable;

        [Test]
        public void Add()
        {
            var cwt = new ConditionalWeakTable<object, object>();
            object c = new Key();

            cwt.Add(c, new Link(c));

            try
            {
                cwt.Add(c, new Link(c));
                Assert.Fail("#0");
            }
            catch (ArgumentException ex)
            {
                Theraot.No.Op(ex);
            }

            cwt.Add("zzz", null);//ok

            try
            {
                cwt.Add(null, new Link(c));
                Assert.Fail("#1");
            }
            catch (ArgumentNullException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [Test]
        [Category("NotDotNet")] // This fails in .net 4.0 too, so yeah
        [Category("GC")]
        [Ignore("Not working")]
        public void FinalizableObjectsThatRetainDeadKeys()
        {
            if (GC.MaxGeneration == 0) /*Boehm doesn't handle ephemerons */
            {
                Assert.Ignore("Not working on Boehm.");
            }

            lock (_lock1)
            {
                var cwt = new ConditionalWeakTable<object, object>();

                void Start()
                {
                    FillWithFinalizable(cwt);
                }

                var th = new Thread(Start);
                th.Start();
                th.Join();
                GC.Collect();
                GC.Collect();

                Assert.AreEqual(0, _reachable, "#1");
            }

            GC.Collect();
            GC.Collect();
            lock (_lock2)
            {
                Monitor.Wait(_lock2, 1000);
            }

            Assert.AreEqual(20, _reachable, "#1");
        }

        [Test]
        public void GetValue()
        {
            var cwt = new ConditionalWeakTable<object, object>();

            try
            {
                cwt.GetValue(null, k => null);
                Assert.Fail("#0");
            }
            catch (ArgumentNullException ex)
            {
                Theraot.No.Op(ex);
            }

            try
            {
                cwt.GetValue(20, null);
                Assert.Fail("#1");
            }
            catch (ArgumentNullException ex)
            {
                Theraot.No.Op(ex);
            }

            object key = "foo";
            var val = cwt.GetValue(key, k => new Link(k));
            Assert.IsTrue(val != null, "#2");
            Assert.AreEqual(typeof(Link), val.GetType(), "#3");

            Assert.AreEqual(val, cwt.GetValue(key, k => new object()), "#4");
        }

        [Test]
        [Category("NotWorking")] // No ephemerons
        [Category("GC")]
        [Ignore("Not working")]
        public void InsertStress()
        {
            if (GC.MaxGeneration == 0) /*Boehm doesn't handle ephemerons */
            {
                Assert.Ignore("Not working on Boehm.");
            }

            var cwt = new ConditionalWeakTable<object, object>();

            var a = new object();
            var b = new object();

            cwt.Add(a, new object());
            cwt.Add(b, new object());

            List<WeakReference> res = null;

            void Start()
            {
                res = FillWithNetwork(cwt);
            }

            var th = new Thread(Start);
            th.Start();
            th.Join();

            GC.Collect();
            GC.Collect();

            for (var i = 0; i < res.Count; ++i)
            {
                Assert.IsFalse(res[i].IsAlive, "#r" + i.ToString());
            }
        }

        [Test]
        [Category("GC")]
        [Category("NotWorking")] // No ephemerons
        public void OldGenKeysMakeNewGenObjectsReachable()
        {
            if (GC.MaxGeneration == 0) /*Boehm doesn't handle ephemerons */
            {
                Assert.Ignore("Not working on Boehm.");
            }

            var table = new ConditionalWeakTable<object, Val>();
            var keys = new List<Key>();

            //
            // This list references all keys for the duration of the program, so none
            // should be collected ever.
            //
            for (var x = 0; x < 1000; x++)
            {
                keys.Add
                (
                    new Key
                    {
                        Foo = x
                    }
                );
            }

            for (var i = 0; i < 1000; ++i)
            {
                // Insert all keys into the ConditionalWeakTable
                foreach (var key in keys)
                {
                    table.Add
                    (
                        key,
                        new Val
                        {
                            Foo = key.Foo
                        }
                    );
                }

                // Look up all keys to verify that they are still there
                foreach (var key in keys)
                {
                    Assert.IsTrue(table.TryGetValue(key, out _), "#1-" + i.ToString() + "-k-" + key);
                }

                // Remove all keys from the ConditionalWeakTable
                foreach (var key in keys)
                {
                    Assert.IsTrue(table.Remove(key), "#2-" + i.ToString() + "-k-" + key);
                }
            }
        }

        [Test]
        [Category("NotWorking")] // No ephemerons
        [Category("GC")]
        [Ignore("Not working")]
        public void OldGenStress()
        {
            if (GC.MaxGeneration == 0) /*Boehm doesn't handle ephemerons */
            {
                Assert.Ignore("Not working on Boehm.");
            }

            var cwt = new ConditionalWeakTable<object, object>[1];
            List<object> k = null;
            List<WeakReference> res = null;
            List<WeakReference> res2 = null;

            void Start()
            {
                res = FillWithNetwork2(cwt);
                ForcePromotion();
                k = FillReachable(cwt);
                res2 = FillWithNetwork2(cwt);
            }

            var th = new Thread(Start);
            th.Start();
            th.Join();

            GC.Collect();

            for (var i = 0; i < res.Count; ++i)
            {
                Assert.IsFalse(res[i].IsAlive, "#r0-" + i.ToString());
            }

            for (var i = 0; i < res2.Count; ++i)
            {
                Assert.IsFalse(res2[i].IsAlive, "#r1-" + i.ToString());
            }

            for (var i = 0; i < k.Count; ++i)
            {
                Assert.IsTrue(cwt[0].TryGetValue(k[i], out var val), "k0-" + i.ToString());
                Assert.AreEqual(i, val, "k1-" + i.ToString());
            }
        }

        [Test]
        [Category("GC")]
        public void Reachability()
        {
            if (GC.MaxGeneration == 0) /*Boehm doesn't handle ephemerons */
            {
                Assert.Ignore("Not working on Boehm.");
            }

            var cwt = new ConditionalWeakTable<object, object>();
            List<object> keepAlive = null;
            List<WeakReference> keys = null;
            var t = new Thread(() => FillStuff(cwt, out keepAlive, out keys));
            t.Start();
            t.Join();

            GC.Collect();

            Assert.IsTrue(keys[0].IsAlive, "r0");
            Assert.IsFalse(keys[1].IsAlive, "r1");
            Assert.IsTrue(keys[2].IsAlive, "r2");

            Assert.IsTrue(cwt.TryGetValue(keepAlive[0], out var res), "ka0");
            Assert.IsTrue(res is Link, "ka1");

            var link = (Link)res;
            Assert.IsTrue(cwt.TryGetValue(link.Obj, out res), "ka2");
            Assert.AreEqual("str0", res, "ka3");
        }

        [Test]
        public void Remove()
        {
            var cwt = new ConditionalWeakTable<object, object>();
            object c = new Key();

            cwt.Add(c, "x");

            try
            {
                cwt.Remove(null);
                Assert.Fail("#0");
            }
            catch (ArgumentNullException ex)
            {
                Theraot.No.Op(ex);
            }

            Assert.IsFalse(cwt.Remove("x"), "#1");
            Assert.IsTrue(cwt.Remove(c), "#2");
            Assert.IsFalse(cwt.Remove(c), "#3");
        }

        [Test]
        public void TryGetValue()
        {
            var cwt = new ConditionalWeakTable<object, object>();
            object res;
            object c = new Key();

            cwt.Add(c, "foo");

            try
            {
                cwt.TryGetValue(null, out res);
                Assert.Fail("#0");
            }
            catch (ArgumentNullException ex)
            {
                Theraot.No.Op(ex);
            }

            Assert.IsFalse(cwt.TryGetValue("foo", out _), "#1");
            Assert.IsTrue(cwt.TryGetValue(c, out res), "#2");
            Assert.AreEqual("foo", res, "#3");
        }

        private static List<object> FillReachable(ConditionalWeakTable<object, object>[] cwt)
        {
            var res = new List<object>();
            for (var i = 0; i < 10; ++i)
            {
                res.Add(new object());
                cwt[0].Add(res[i], i);
            }

            return res;
        }

        private static void FillStuff(ConditionalWeakTable<object, object> cwt, out List<object> keepAlive, out List<WeakReference> keys)
        {
            keepAlive = new List<object>();
            keys = new List<WeakReference>();

            object a = new Key();
            object b = new Key();
            object c = new Key();

            cwt.Add(a, new string("str0".ToCharArray()));
            cwt.Add(b, "str1");
            cwt.Add(c, new Link(a));

            keepAlive.Add(c);
            keys.Add(new WeakReference(a));
            keys.Add(new WeakReference(b));
            keys.Add(new WeakReference(c));
        }

        private static void FillWithFinalizable(ConditionalWeakTable<object, object> cwt)
        {
            var a = new object();
            object b = new FinalizableLink(0, a, cwt);
            cwt.Add(a, "foo");
            cwt.Add(b, "bar");

            for (var i = 1; i < 20; ++i)
            {
                b = new FinalizableLink(i, b, cwt);
                cwt.Add(b, i);
            }
        }

        private static List<WeakReference> FillWithNetwork(ConditionalWeakTable<object, object> cwt)
        {
            const int k = 500;
            var keys = new object[k];
            for (var i = 0; i < k; ++i)
            {
                keys[i] = new object();
            }

            var rand = new Random();

            /*produce a complex enough network of links*/
            for (var i = 0; i < k; ++i)
            {
                cwt.Add(keys[i], new Link(keys[rand.Next(k)]));
            }

            var res = new List<WeakReference>();

            for (var i = 0; i < 10; ++i)
            {
                res.Add(new WeakReference(keys[rand.Next(k)]));
            }

            Array.Clear(keys, 0, keys.Length);

            return res;
        }

        private static List<WeakReference> FillWithNetwork2(ConditionalWeakTable<object, object>[] cwt)
        {
            if (cwt[0] == null)
            {
                cwt[0] = new ConditionalWeakTable<object, object>();
            }

            var res = FillWithNetwork(cwt[0]);

            return res;
        }

        private static void ForceMinor()
        {
            for (var i = 0; i < 64000; ++i)
            {
                var x = new object();
                GC.KeepAlive(x);
            }
        }

        private static void ForcePromotion()
        {
            var o = new object[64000];

            for (var i = 0; i < 64000; ++i)
            {
                o[i] = new int[10];
            }
        }

        public class FinalizableLink
        {
            // The sole purpose of this object is to keep a reference to another object, so it is fine to not use it.
            private readonly ConditionalWeakTable<object, object> _cwt;

            // For debug purposes
            // ReSharper disable once NotAccessedField.Local
            private readonly int _id;

            // For debug purposes
            // ReSharper disable once NotAccessedField.Local
            private readonly object _obj;

            public FinalizableLink(int id, object obj, ConditionalWeakTable<object, object> cwt)
            {
                _id = id;
                _obj = obj;
                _cwt = cwt;
            }

            ~FinalizableLink()
            {
                lock (_lock1)
                {
                    // Empty
                }

                var res = _cwt.TryGetValue(this, out _);
                if (res)
                {
                    _reachable++;
                }

                if (_reachable != 20)
                {
                    return;
                }

                lock (_lock2)
                {
                    Monitor.Pulse(_lock2);
                }
            }
        }

        public class Link
        {
            public object Obj;

            public Link(object obj)
            {
                Obj = obj;
            }
        }

        private sealed class Key
        {
            public int Foo;

            public override string ToString()
            {
                return "key-" + Foo.ToString();
            }
        }

        private sealed class Val
        {
            public int Foo;

            public override string ToString()
            {
                return "value-" + Foo.ToString();
            }
        }
    }

    public partial class ConditionalWeakTableTest
    {
#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD12

        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static void PromotedCwtPointingToYoungStuff()
        {
            var cwt = new ConditionalWeakTable<object, object>();

            var handles = FillTable3(cwt);

            GC.Collect(0);

            /*Be 100% sure it will be on the young gen*/

            /*cwt array now will be on old gen*/
            ForceMinor();
            ForceMinor();
            ForceMinor();

            //Make them non pinned
            MakeObjMovable(handles);

            GC.Collect(0);

            //Force a minor GC - this should cause
            ForceMinor();
            ForceMinor();
            ForceMinor();
            ForceMinor();

            GC.Collect(0);

            Assert.IsTrue(cwt.TryGetValue(handles[0].Target, out _), "#1");
            Assert.IsTrue(cwt.TryGetValue(handles[1].Target, out _), "#2");

            GC.Collect();
            GC.KeepAlive(cwt.GetHashCode());
        }

        [Test]
        public void GetOrCreateValue()
        {
            var cwt = new ConditionalWeakTable<object, object>();

            try
            {
                cwt.GetOrCreateValue(null);
                Assert.Fail("#0");
            }
            catch (ArgumentNullException ex)
            {
                Theraot.No.Op(ex);
            }

            object key = "foo";
            var val = cwt.GetOrCreateValue(key);
            Assert.IsTrue(val != null, "#2");
            Assert.AreEqual(typeof(object), val.GetType(), "#3");

            Assert.AreEqual(val, cwt.GetOrCreateValue(key), "#4");

            var cwt2 = new ConditionalWeakTable<object, string>();
            try
            {
                cwt2.GetOrCreateValue(key);
                Assert.Fail("#5");
            }
            catch (MissingMethodException ex)
            {
                Theraot.No.Op(ex);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        private static List<GCHandle> FillTable3(ConditionalWeakTable<object, object> cwt)
        {
            var handles = new List<GCHandle>();

            var a = (object)10;
            var b = (object)20;
            var k1 = (object)30;
            var k2 = (object)40;

            handles.Add(GCHandle.Alloc(a, GCHandleType.Pinned));
            handles.Add(GCHandle.Alloc(b, GCHandleType.Pinned));
            handles.Add(GCHandle.Alloc(k1, GCHandleType.Pinned));
            handles.Add(GCHandle.Alloc(k2, GCHandleType.Pinned));

            cwt.Add(a, k1);
            cwt.Add(b, k2);

            return handles;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        private static void MakeObjMovable(List<GCHandle> handles)
        {
            for (var i = 0; i < handles.Count; ++i)
            {
                var o = handles[i].Target;
                handles[i].Free();
                handles[i] = GCHandle.Alloc(o, GCHandleType.Normal);
            }
        }

#endif
    }
}