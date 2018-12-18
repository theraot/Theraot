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

using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace MonoTests.System.Collections.Concurrent
{
    [TestFixture()]
    public class ConcurrentQueueTests
    {
        private ConcurrentQueue<int> _queue;

        [SetUp]
        public void Setup()
        {
            _queue = new ConcurrentQueue<int>();
            for (var i = 0; i < 10; i++)
            {
                _queue.Enqueue(i);
            }
        }

        [Test]
        public void StressEnqueueTestCase()
        {
            /*ParallelTestHelper.Repeat (delegate {
                queue = new ConcurrentQueue<int> ();
                int amount = -1;
                const int count = 10;
                const int threads = 5;

                ParallelTestHelper.ParallelStressTest (queue, (q) => {
                    int t = Interlocked.Increment (ref amount);
                    for (int i = 0; i < count; i++)
                        queue.Enqueue (t);
                }, threads);

                Assert.AreEqual (threads * count, queue.Count, "#-1");
                int[] values = new int[threads];
                int temp;
                while (queue.TryDequeue (out temp)) {
                    values[temp]++;
                }

                for (int i = 0; i < threads; i++)
                    Assert.AreEqual (count, values[i], "#" + i);
            });*/

            CollectionStressTestHelper.AddStressTest(new ConcurrentQueue<int>());
        }

        [Test]
        public void StressDequeueTestCase()
        {
            /*ParallelTestHelper.Repeat (delegate {
                queue = new ConcurrentQueue<int> ();
                const int count = 10;
                const int threads = 5;
                const int delta = 5;

                for (int i = 0; i < (count + delta) * threads; i++)
                    queue.Enqueue (i);

                bool state = true;

                ParallelTestHelper.ParallelStressTest (queue, (q) => {
                    int t;
                    for (int i = 0; i < count; i++)
                        state &= queue.TryDequeue (out t);
                }, threads);

                Assert.IsTrue (state, "#1");
                Assert.AreEqual (delta * threads, queue.Count, "#2");

                string actual = string.Empty;
                int temp;
                while (queue.TryDequeue (out temp)) {
                    actual += temp;
                }
                string expected = Enumerable.Range (count * threads, delta * threads)
                    .Aggregate (string.Empty, (acc, v) => acc + v);

                Assert.AreEqual (expected, actual, "#3");
            });*/

            CollectionStressTestHelper.RemoveStressTest(new ConcurrentQueue<int>(), CheckOrderingType.InOrder);
        }

        [Test]
        public void StressTryPeekTestCase()
        {
            ParallelTestHelper.Repeat(delegate
            {
                var queue = new ConcurrentQueue<object>();
                queue.Enqueue(new object());

                const int Threads = 10;
                var threadCounter = 0;
                var success = true;

                ParallelTestHelper.ParallelStressTest(queue, (q) =>
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
                }, Threads);

                Assert.IsTrue(success, "TryPeek returned unexpected null value.");
            }, 10);
        }

        [Test]
        public void CountTestCase()
        {
            Assert.AreEqual(10, _queue.Count, "#1");
            _queue.TryPeek(out _);
            _queue.TryDequeue(out _);
            _queue.TryDequeue(out _);
            Assert.AreEqual(8, _queue.Count, "#2");
        }

        //[Ignore]
        [Test]
        public void EnumerateTestCase()
        {
            var s = string.Empty;
            var builder = new StringBuilder();
            builder.Append(s);
            foreach (var i in _queue)
            {
                builder.Append(i);
            }
            s = builder.ToString();
            Assert.AreEqual("0123456789", s, "#1 : " + s);
        }

        [Test()]
        public void TryPeekTestCase()
        {
            _queue.TryPeek(out var value);
            Assert.AreEqual(0, value, $"#1 : {value}");
            _queue.TryDequeue(out value);
            Assert.AreEqual(0, value, $"#2 : {value}");
            _queue.TryDequeue(out value);
            Assert.AreEqual(1, value, $"#3 : {value}");
            _queue.TryPeek(out value);
            Assert.AreEqual(2, value, $"#4 : {value}");
            _queue.TryPeek(out value);
            Assert.AreEqual(2, value, $"#5 : {value}");
        }

        [Test()]
        public void TryDequeueTestCase()
        {
            _queue.TryPeek(out var value);
            Assert.AreEqual(0, value, "#1");
            Assert.IsTrue(_queue.TryDequeue(out value), "#2");
            Assert.IsTrue(_queue.TryDequeue(out value), "#3");
            Assert.AreEqual(1, value, "#4");
        }

        [Test()]
        public void TryDequeueEmptyTestCase()
        {
            _queue = new ConcurrentQueue<int>();
            _queue.Enqueue(1);
            Assert.IsTrue(_queue.TryDequeue(out _), "#1");
            Assert.IsFalse(_queue.TryDequeue(out _), "#2");
            Assert.IsTrue(_queue.IsEmpty, "#3");
        }

        [Test]
        public void ToArrayTest()
        {
            var array = _queue.ToArray();
            var s = string.Empty;
            var builder = new StringBuilder();
            builder.Append(s);
            foreach (var i in array)
            {
                builder.Append(i);
            }
            s = builder.ToString();
            Assert.AreEqual("0123456789", s, "#1 : " + s);
            _queue.CopyTo(array, 0);
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

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ToExistingArray_Null()
        {
            _queue.CopyTo(null, 0);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToExistingArray_OutOfRange()
        {
            _queue.CopyTo(new int[3], -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ToExistingArray_IndexOverflow()
        {
            _queue.CopyTo(new int[3], 4);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ToExistingArray_Overflow()
        {
            _queue.CopyTo(new int[3], 0);
        }

        private static WeakReference CreateWeakReference(object obj)
        {
            return new WeakReference(obj);
        }

        [Test]
        public void TryDequeueReferenceTest()
        {
            var queue = new ConcurrentQueue<object>();
            var weakReference = AddObjectWeakReference(queue);
            DequeueIgnore(queue);
            Thread.MemoryBarrier();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.MemoryBarrier();
            Assert.IsFalse(weakReference.IsAlive);
        }

        private static void DequeueIgnore(ConcurrentQueue<object> queue)
        {
            queue.TryDequeue(out _);
        }

        private static WeakReference AddObjectWeakReference(ConcurrentQueue<object> queue)
        {
            var obj = new object();
            var weakReference = CreateWeakReference(obj);
            queue.Enqueue(obj);
            return weakReference;
        }
    }
}