using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Theraot.Collections;

namespace Tests.Theraot.Collections
{
    [TestFixture]
    public class ProgressorTest
    {
        [Test]
        public void ConvertedProgressor()
        {
            var source = new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5 });
            var progresor = Progressor<string>.CreateConverted(source, input => input.ToString(CultureInfo.InvariantCulture));
            var indexA = 0;
            var indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB.ToString(CultureInfo.InvariantCulture));
                    indexB++;
                }
            );
            string item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA.ToString(CultureInfo.InvariantCulture));
                indexA++;
            }
            Assert.AreEqual(6, indexA);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void CreatedFilteredConverted()
        {
            var source = new Progressor<int>(new[] { 0, 8, 1, 8, 2, 3, 4, 8, 5 });
            var progresor = Progressor<string>.CreatedFilteredConverted(source, input => input != 8, input => input.ToString(CultureInfo.InvariantCulture));
            var indexA = 0;
            var indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB.ToString(CultureInfo.InvariantCulture));
                    indexB++;
                }
            );
            string item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA.ToString(CultureInfo.InvariantCulture));
                indexA++;
            }
            Assert.AreEqual(6, indexA);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void DistinctProgressor()
        {
            var source = new Progressor<int>(new[] { 0, 0, 1, 1, 2, 0, 1, 3, 4, 4, 4, 4, 5, 5, 5, 0, 1, 3 });
            var progresor = Progressor<int>.CreateDistinct(source);
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
        public void FilteredProgressor()
        {
            var source = new Progressor<int>(new[] { 0, 8, 1, 8, 2, 3, 4, 8, 5 });
            var progresor = Progressor<int>.CreatedFiltered(source, input => input != 8);
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
            var progresor = new Progressor<int>((IObservable<int>) source);
            source.AsEnumerable().Consume();
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
        public void PrefaceProgressor()
        {
            var preface = new[] { 0, 1, 2, 3, 4, 5 };
            var source = new Progressor<int>(new[] { 6, 7, 8, 9 });
            var progresor = new Progressor<int>(preface, source);
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
            Assert.AreEqual(10, indexA);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void ProgressorProgressor()
        {
            var source = new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5 });
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
        public void ThreadedUse()
        {
            var source = new Progressor<int>(new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }).AsEnumerable();
            var handle = new ManualResetEvent(false);
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
            while (Thread.VolatileRead(ref count[0]) != 2)
            {
                Thread.Sleep(0);
            }
            handle.Set();
            while (Thread.VolatileRead(ref count[1]) != 2)
            {
                Thread.Sleep(0);
            }
            Assert.AreEqual(10, Thread.VolatileRead(ref count[2]));
            handle.Close();
        }

        [Test]
        public void ThreadedUseArray()
        {
            var source = new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }).AsEnumerable();
            var handle = new ManualResetEvent(false);
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
            while (Thread.VolatileRead(ref count[0]) != 2)
            {
                Thread.Sleep(0);
            }
            handle.Set();
            while (Thread.VolatileRead(ref count[1]) != 2)
            {
                Thread.Sleep(0);
            }
            Assert.AreEqual(10, Thread.VolatileRead(ref count[2]));
            handle.Close();
        }

        [Test]
        public void ThreadedUseWithArrayPreface()
        {
            var source = new Progressor<int>(new[] { 7, 7, 7, 7, 7 }, new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 })).AsEnumerable();
            var handle = new ManualResetEvent(false);
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
            while (Thread.VolatileRead(ref count[0]) != 2)
            {
                Thread.Sleep(0);
            }
            handle.Set();
            while (Thread.VolatileRead(ref count[1]) != 2)
            {
                Thread.Sleep(0);
            }
            Assert.AreEqual(15, Thread.VolatileRead(ref count[2]));
            handle.Close();
        }

        [Test]
        public void ThreadedUseWithPreface()
        {
            var source = new Progressor<int>( new List<int> {7, 7, 7, 7, 7}, new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 })).AsEnumerable();
            var handle = new ManualResetEvent(false);
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
            while (Thread.VolatileRead(ref count[0]) != 2)
            {
                Thread.Sleep(0);
            }
            handle.Set();
            while (Thread.VolatileRead(ref count[1]) != 2)
            {
                Thread.Sleep(0);
            }
            Assert.AreEqual(15, Thread.VolatileRead(ref count[2]));
            handle.Close();
        }

        [Test]
        public void TryTakeProgressor()
        {
            var source = new Queue<int>();
            source.Enqueue(0);
            source.Enqueue(1);
            source.Enqueue(2);
            source.Enqueue(3);
            source.Enqueue(4);
            source.Enqueue(5);
            var progresor = new Progressor<int>
            (
                (out int value) =>
                {
                    try
                    {
                        value = source.Dequeue();
                        return true;
                    }
                    catch (Exception)
                    {
                        value = 0;
                        return false;
                    }
                },
                false
            );
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
    }
}