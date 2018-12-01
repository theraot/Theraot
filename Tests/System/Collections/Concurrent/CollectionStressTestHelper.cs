#define NET_4_0
#if NET_4_0
//
// CollectionStressTestHelper.cs
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
using NUnit.Framework.Constraints;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoTests.System.Collections.Concurrent
{
    public enum CheckOrderingType
    {
        InOrder,
        Reversed,
        DontCare
    }

    public static class CollectionStressTestHelper
    {
        public static void AddStressTest(IProducerConsumerCollection<int> coll)
        {
            ParallelTestHelper.Repeat(delegate
            {
                var amount = -1;
                const int Count = 10;
                const int Threads = 5;

                ParallelTestHelper.ParallelStressTest(coll, (q) =>
                {
                    var t = Interlocked.Increment(ref amount);
                    for (var i = 0; i < Count; i++)
                    {
                        coll.TryAdd(t);
                    }
                }, Threads);

                Assert.AreEqual(Threads * Count, coll.Count, "#-1");
                var values = new int[Threads];
                int temp;
                while (coll.TryTake(out temp))
                {
                    values[temp]++;
                }

                for (var i = 0; i < Threads; i++)
                {
                    Assert.AreEqual(Count, values[i], "#" + i.ToString());
                }
            });
        }

        public static void RemoveStressTest(IProducerConsumerCollection<int> coll, CheckOrderingType order)
        {
            ParallelTestHelper.Repeat(delegate
            {
                const int Count = 10;
                const int Threads = 5;
                const int Delta = 5;

                for (var i = 0; i < (Count + Delta) * Threads; i++)
                {
                    while (!coll.TryAdd(i))
                    {
                    }
                }

                var state = true;

                Assert.AreEqual((Count + Delta) * Threads, coll.Count, "#0");

                ParallelTestHelper.ParallelStressTest(coll, (q) =>
                {
                    var s = true;
                    for (var i = 0; i < Count; i++)
                    {
                        s &= coll.TryTake(out _);
                        // try again in case it was a transient failure
                        if (!s && coll.TryTake(out _))
                        {
                            s = true;
                        }
                    }

                    state &= s;
                }, Threads);

                Assert.IsTrue(state, "#1");
                Assert.AreEqual(Delta * Threads, coll.Count, "#2");

                var actual = string.Empty;
                int temp;
                var builder = new StringBuilder();
                builder.Append(actual);
                while (coll.TryTake(out temp))
                {
                    builder.Append(temp.ToString());
                }
                actual = builder.ToString();

                var range = Enumerable.Range(order == CheckOrderingType.Reversed ? 0 : Count * Threads, Delta * Threads);
                if (order == CheckOrderingType.Reversed)
                {
                    range = range.Reverse();
                }

                var expected = range.Aggregate(string.Empty, (acc, v) => acc + v.ToString());

                if (order == CheckOrderingType.DontCare)
                {
                    Assert.That(actual, new CollectionEquivalentConstraint(expected), "#3");
                }
                else
                {
                    Assert.AreEqual(expected, actual, "#3");
                }
            }, 10);
        }
    }
}

#endif