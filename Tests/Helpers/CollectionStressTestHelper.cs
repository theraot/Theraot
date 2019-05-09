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

namespace Tests.Helpers
{
    public static class CollectionStressTestHelper
    {
        public static void AddStressTest(IProducerConsumerCollection<int> collection)
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var amount = -1;
                    const int itemsCount = 10;
                    const int threadCount = 5;

                    ParallelTestHelper.ParallelStressTest
                    (
                        () =>
                        {
                            var t = Interlocked.Increment(ref amount);
                            for (var i = itemsCount - 1; i >= 0; i--)
                            {
                                collection.TryAdd(t);
                            }
                        },
                        threadCount
                    );

                    Assert.AreEqual(threadCount * itemsCount, collection.Count, "#-1");
                    var values = new int[threadCount];
                    while (collection.TryTake(out var temp))
                    {
                        values[temp]++;
                    }

                    for (var i = 0; i < threadCount; i++)
                    {
                        Assert.AreEqual(itemsCount, values[i], "#" + i);
                    }
                }
            );
        }

        public static void RemoveStressTest(IProducerConsumerCollection<int> collection, CheckOrderingType order)
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    const int count = 10;
                    const int threads = 5;
                    const int delta = 5;

                    for (var i = 0; i < (count + delta) * threads; i++)
                    {
                        while (!collection.TryAdd(i))
                        {
                            // Empty
                        }
                    }

                    var state = true;

                    Assert.AreEqual((count + delta) * threads, collection.Count, "#0");

                    ParallelTestHelper.ParallelStressTest
                    (
                        () =>
                        {
                            var check = true;
                            for (var i = 0; i < count; i++)
                            {
                                check &= collection.TryTake(out _);
                                // try again in case it was a transient failure
                                if (!check && collection.TryTake(out _))
                                {
                                    check = true;
                                }
                            }

                            state &= check;
                        },
                        threads
                    );

                    Assert.IsTrue(state, "#1");
                    Assert.AreEqual(delta * threads, collection.Count, "#2");

                    var actual = string.Empty;
                    var builder = new StringBuilder();
                    builder.Append(actual);
                    while (collection.TryTake(out var temp))
                    {
                        builder.Append(temp.ToString());
                    }

                    actual = builder.ToString();

                    var range = Enumerable.Range(order == CheckOrderingType.Reversed ? 0 : count * threads, delta * threads);
                    if (order == CheckOrderingType.Reversed)
                    {
                        range = range.Reverse();
                    }

                    var expected = range.Aggregate(string.Empty, (acc, v) => acc + v);

                    if (order == CheckOrderingType.DoNotCare)
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