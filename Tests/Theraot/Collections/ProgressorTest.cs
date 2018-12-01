using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections;

namespace Tests.Theraot.Collections
{
    [TestFixture]
    public class ProgressorTest
    {
        [Test]
        public void EnumerableProgressor()
        {
            var source = new[] { 0, 1, 2, 3, 4, 5 };
            var progresor = new Progressor<int>(source);
            var indexA = 0;
            var indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB);
                    indexB++;
                }
            );
            int item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA);
                indexA++;
            }
            Assert.AreEqual(6, indexA);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void ObservableProgressor()
        {
            var source = new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5 });
            var progresor = new Progressor<int>((IObservable<int>)source);
            source.Consume();
            var indexA = 0;
            var indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB);
                    indexB++;
                }
            );
            int item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA);
                indexA++;
            }
            Assert.AreEqual(6, indexA);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void ThreadedUse()
        {
            var source = new Progressor<int>(new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            using (var handle = new ManualResetEvent(false))
            {
                int[] count = { 0, 0, 0 };
                var work = new WaitCallback
                    (
                        _ =>
                        {
                            Interlocked.Increment(ref count[0]);
                            handle.WaitOne();
                            foreach (var item in source)
                            {
                                GC.KeepAlive(item);
                                Interlocked.Increment(ref count[2]);
                            }
                            Interlocked.Increment(ref count[1]);
                        }
                    );
                ThreadPool.QueueUserWorkItem(work);
                ThreadPool.QueueUserWorkItem(work);
                while (Volatile.Read(ref count[0]) != 2)
                {
                    Thread.Sleep(1);
                }
                handle.Set();
                while (Volatile.Read(ref count[1]) != 2)
                {
                    Thread.Sleep(1);
                }
                Assert.AreEqual(10, Volatile.Read(ref count[2]));
            }
        }

        [Test]
        public void ThreadedUseArray()
        {
            var source = new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            using (var handle = new ManualResetEvent(false))
            {
                int[] count = { 0, 0, 0 };
                var work = new WaitCallback
                    (
                        _ =>
                        {
                            Interlocked.Increment(ref count[0]);
                            handle.WaitOne();
                            foreach (var item in source)
                            {
                                GC.KeepAlive(item);
                                Interlocked.Increment(ref count[2]);
                            }
                            Interlocked.Increment(ref count[1]);
                        }
                    );
                ThreadPool.QueueUserWorkItem(work);
                ThreadPool.QueueUserWorkItem(work);
                while (Volatile.Read(ref count[0]) != 2)
                {
                    Thread.Sleep(1);
                }
                handle.Set();
                while (Volatile.Read(ref count[1]) != 2)
                {
                    Thread.Sleep(1);
                }
                Assert.AreEqual(10, Volatile.Read(ref count[2]));
            }
        }

        [Test]
        [Category("Performance")]
        public void ThreadedUseArrayLoop()
        {
            for (int i = 0; i < 100000; i++)
            {
                ThreadedUseArray();
            }
        }

        [Test]
        [Category("Performance")]
        public void ThreadedUseLoop()
        {
            for (int i = 0; i < 100000; i++)
            {
                ThreadedUse();
            }
        }
    }
}