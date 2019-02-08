// ConcurrentQueueTest.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
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

using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Tests.Helpers;

namespace SystemTests.CollectionsTests.ConcurrentTests
{
    [TestFixture]
    public class ConcurrentQueueTests
    {
        public static ConcurrentQueue<int> Setup()
        {
            var queue = new ConcurrentQueue<int>();
            for (var i = 0; i < 10; i++)
            {
                queue.Enqueue(i);
            }

            return queue;
        }

        private static WeakReference AddObjectWeakReference(ConcurrentQueue<object> queue)
        {
            var obj = new object();
            var weakReference = CreateWeakReference(obj);
            queue.Enqueue(obj);
            return weakReference;
        }

        private static WeakReference CreateWeakReference(object obj)
        {
            return new WeakReference(obj);
        }

        private static void DequeueIgnore(ConcurrentQueue<object> queue)
        {
            queue.TryDequeue(out _);
        }

        [Test]
        public void CountTestCase()
        {
            var queue = Setup();
            Assert.AreEqual(10, queue.Count, "#1");
            queue.TryPeek(out _);
            queue.TryDequeue(out _);
            queue.TryDequeue(out _);
            Assert.AreEqual(8, queue.Count, "#2");
        }

        [Test]
        public void EnumerateTestCase()
        {
            var queue = Setup();
            var s = string.Empty;
            var builder = new StringBuilder();
            builder.Append(s);
            foreach (var item in queue)
            {
                builder.Append(item);
            }

            s = builder.ToString();
            Assert.AreEqual("0123456789", s, "#1 : " + s);
        }

        [Test]
        [Category("LongRunning")]
        public void StressDequeueTestCase()
        {
            CollectionStressTestHelper.RemoveStressTest(new ConcurrentQueue<int>(), CheckOrderingType.InOrder);
        }

        [Test]
        [Category("LongRunning")]
        public void StressEnqueueTestCase()
        {
            CollectionStressTestHelper.AddStressTest(new ConcurrentQueue<int>());
        }

        [Test]
        [Category("LongRunning")]
        public void StressTryPeekTestCase()
        {
            ParallelTestHelper.Repeat
            (
                delegate
                {
                    var queue = new ConcurrentQueue<object>();
                    queue.Enqueue(new object());

                    const int Threads = 10;
                    var threadCounter = 0;
                    var success = true;

                    ParallelTestHelper.ParallelStressTest
                    (
                        () =>
                        {
                            var threadId = Interlocked.Increment(ref threadCounter);
                            object temp;
                            if (threadId < Threads)
                            {
                                while (queue.TryPeek(out temp))
                                {
                                    success &= temp != null;
                                }
                            }
                            else
                            {
                                queue.TryDequeue(out temp);
                            }
                        }, Threads
                    );

                    Assert.IsTrue(success, "TryPeek returned unexpected null value.");
                }, 10
            );
        }

        [Test]
        public void ToArrayTest()
        {
            var queue = Setup();
            var array = queue.ToArray();
            var s = string.Empty;
            var builder = new StringBuilder();
            builder.Append(s);
            foreach (var i in array)
            {
                builder.Append(i);
            }

            s = builder.ToString();
            Assert.AreEqual("0123456789", s, "#1 : " + s);
            queue.CopyTo(array, 0);
            s = string.Empty;
            builder = new StringBuilder();
            builder.Append(s);
            foreach (var i in array)
            {
                builder.Append(i);
            }

            s = builder.ToString();
            Assert.AreEqual("0123456789", s, "#2 : " + s);
        }

        [Test]
        public void ToExistingArray_IndexOverflow()
        {
            var queue = Setup();
            Assert.Throws<ArgumentException>(() => queue.CopyTo(new int[3], 4));
        }

        [Test]
        public void ToExistingArray_Null()
        {
            var queue = Setup();
            Assert.Throws<ArgumentNullException>(() => queue.CopyTo(null, 0));
        }

        [Test]
        public void ToExistingArray_OutOfRange()
        {
            var queue = Setup();
            Assert.Throws<ArgumentOutOfRangeException>(() => queue.CopyTo(new int[3], -1));
        }

        [Test]
        public void ToExistingArray_Overflow()
        {
            var queue = Setup();
            Assert.Throws<ArgumentException>(() => queue.CopyTo(new int[3], 0));
        }

        [Test]
        public void TryDequeueEmptyTestCase()
        {
            var queue = new ConcurrentQueue<int>();
            queue.Enqueue(1);
            Assert.IsTrue(queue.TryDequeue(out _), "#1");
            Assert.IsFalse(queue.TryDequeue(out _), "#2");
            Assert.IsTrue(queue.IsEmpty, "#3");
        }

        [Test]
        public void TryDequeueReferenceTest()
        {
            var queue = new ConcurrentQueue<object>();
            var weakReference = AddObjectWeakReference(queue);
            DequeueIgnore(queue);
            ThreadEx.MemoryBarrier();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ThreadEx.MemoryBarrier();
            Assert.IsFalse(weakReference.IsAlive);
        }

        [Test]
        public void TryDequeueTestCase()
        {
            var queue = Setup();
            queue.TryPeek(out var value);
            Assert.AreEqual(0, value, "#1");
            Assert.IsTrue(queue.TryDequeue(out value), "#2");
            Assert.IsTrue(queue.TryDequeue(out value), "#3");
            Assert.AreEqual(1, value, "#4");
        }

        [Test]
        public void TryPeekTestCase()
        {
            var queue = Setup();
            queue.TryPeek(out var value);
            Assert.AreEqual(0, value, $"#1 : {value}");
            queue.TryDequeue(out value);
            Assert.AreEqual(0, value, $"#2 : {value}");
            queue.TryDequeue(out value);
            Assert.AreEqual(1, value, $"#3 : {value}");
            queue.TryPeek(out value);
            Assert.AreEqual(2, value, $"#4 : {value}");
            queue.TryPeek(out value);
            Assert.AreEqual(2, value, $"#5 : {value}");
        }
    }
}