using NUnit.Framework;
using System;
using System.Reflection;
using System.Threading;

namespace MonoTests.System
{
    [TestFixture]
    public class LazyTestEx
    {
        [Test]
        public void CacheException()
        {
            // No Cache Publication Only
            var a = new Lazy<DefectiveCtorClass>(false);
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run (total 1)
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run (total 2)
            Assert.AreEqual(3, a.Value.Prop); // Did run once (total 3)

            // No Cache Execution and Publication
            a = new Lazy<DefectiveCtorClass>(true);
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run once (total 4)
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run once (total 5)
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run once (total 6)
            Assert.AreEqual(7, a.Value.Prop); // Did run once (total 7)

            // Cache Execution and Publication
            a = new Lazy<DefectiveCtorClass>(() => new DefectiveCtorClass(), LazyThreadSafetyMode.ExecutionAndPublication);
            Assert.Throws(typeof(InvalidOperationException), () => Console.WriteLine(a.Value)); // Did run once (total 8)
            Assert.Throws(typeof(InvalidOperationException), () => Console.WriteLine(a.Value));
            Assert.Throws(typeof(InvalidOperationException), () => Console.WriteLine(a.Value));

            // No Cache Publication Only
            a = new Lazy<DefectiveCtorClass>(() => new DefectiveCtorClass(), LazyThreadSafetyMode.PublicationOnly);
            Assert.Throws(typeof(InvalidOperationException), () => Console.WriteLine(a.Value)); // Did run once (total 9)
            Assert.Throws(typeof(InvalidOperationException), () => Console.WriteLine(a.Value)); // Did run once (total 10)
            Assert.AreEqual(11, a.Value.Prop); // Did run once (total 11)

            // Cache None
            a = new Lazy<DefectiveCtorClass>(() => new DefectiveCtorClass(), LazyThreadSafetyMode.None);
            Assert.Throws(typeof(InvalidOperationException), () => Console.WriteLine(a.Value)); // Did run once (total 12)
            Assert.Throws(typeof(InvalidOperationException), () => Console.WriteLine(a.Value));
            Assert.Throws(typeof(InvalidOperationException), () => Console.WriteLine(a.Value));

            // No Cache Publication Only (Again)
            a = new Lazy<DefectiveCtorClass>(false);
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run once (total 13)
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run once (total 14)
            Assert.AreEqual(15, a.Value.Prop); // Did run once (total 15)

            // No Cache Execution and Publication (Again)
            a = new Lazy<DefectiveCtorClass>(true);
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run once (total 16)
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run once (total 17)
            Assert.Throws(typeof(TargetInvocationException), () => Console.WriteLine(a.Value)); // Did run once (total 18)
            Assert.AreEqual(19, a.Value.Prop); // Did run once (total 23)
        }

        [Test]
        public void ConstructorWithNull()
        {
            Assert.Throws(typeof(ArgumentNullException), () => GC.KeepAlive(new Lazy<int>(null)));
        }

        [Test]
        public void ConstructorWithValueFactory()
        {
            var needle = new Lazy<int>(() => 5);
            Assert.AreEqual(needle.Value, 5);
        }

        [Test]
        public void DefaultConstructor()
        {
            //Not nullable
            var a = new Lazy<int>();
            Assert.IsFalse(a.IsValueCreated);
            Assert.AreEqual(a.Value, default(int));
            //Nullable
            var b = new Lazy<int?>();
            Assert.IsFalse(b.IsValueCreated);
            Assert.AreEqual(b.Value, null);
            //Object
            var c = new Lazy<int?>();
            Assert.IsFalse(c.IsValueCreated);
            Assert.AreEqual(c.Value, null);
        }

        [Test]
        public void InitializeOnlyOnce()
        {
            var control = 0;
            var threadDone = 0;
            var needle = new Lazy<int>(() =>
            {
                Interlocked.Increment(ref control); return 5;
            });
            var manual = new ManualResetEvent(false);
            var threadA = new Thread(() =>
            {
                manual.WaitOne();
                GC.KeepAlive(needle.Value);
                Interlocked.Increment(ref threadDone);
            });
            var threadB = new Thread(() =>
            {
                manual.WaitOne();
                GC.KeepAlive(needle.Value);
                Interlocked.Increment(ref threadDone);
            });
            var threadC = new Thread(() =>
            {
                manual.WaitOne();
                GC.KeepAlive(needle.Value);
                Interlocked.Increment(ref threadDone);
            });
            threadA.Start();
            threadB.Start();
            threadC.Start();
            manual.Set();
            threadA.Join();
            threadB.Join();
            threadC.Join();
            GC.KeepAlive(needle.Value);
            GC.KeepAlive(needle.Value);
            Assert.IsTrue(needle.IsValueCreated);
            Assert.AreEqual(needle.Value, 5);
            Assert.AreEqual(control, 1);
            Assert.AreEqual(threadDone, 3);
            manual.Close();
        }

        [Test]
        public void IsCompletedAndIsAlive()
        {
            //Not Nullable
            var a = new Lazy<int>(() => 5);
            Assert.IsFalse(a.IsValueCreated);
            Assert.AreEqual(a.Value, 5);
            Assert.IsTrue(a.IsValueCreated);
            //Nullable
            var b = new Lazy<int?>(() => null);
            Assert.IsFalse(b.IsValueCreated);
            Assert.AreEqual(b.Value, null);
            Assert.IsTrue(b.IsValueCreated);
            //Object
            var c = new Lazy<string>(() => null);
            Assert.IsFalse(c.IsValueCreated);
            Assert.AreEqual(c.Value, null);
            Assert.IsTrue(c.IsValueCreated);
        }

        [Test]
        public void ValueFactoryReentry()
        {
            Lazy<int>[] needle = { null };
            needle[0] = new Lazy<int>(() => ReferenceEquals(needle[0], null) ? 0 : needle[0].Value);
            Assert.Throws(typeof(InvalidOperationException), () => GC.KeepAlive(needle[0].Value));
        }

        private class DefectiveCtorClass
        {
            private static int count;

            public DefectiveCtorClass()
            {
                count++;
                if (count % 4 != 3)
                {
                    throw new InvalidOperationException();
                }
                Prop = count;
            }

            public int Prop
            {
                get;
                private set;
            }
        }
    }
}