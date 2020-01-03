#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

// ConcurrentDictionaryTests.cs
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

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Tests.Helpers;
using Theraot;

namespace Tests.SystemTests.CollectionsTests.ConcurrentTests
{
    [TestFixture]
    public class ConcurrentDictionaryTests
    {
        public static ConcurrentDictionary<string, int> Setup()
        {
            var map = new ConcurrentDictionary<string, int>();
            map.TryAdd("foo", 1);
            map.TryAdd("bar", 2);
            map["foobar"] = 3;
            return map;
        }

        [Test]
        public void AddOrUpdateTest()
        {
            var map = Setup();
            Assert.AreEqual(11, map.AddOrUpdate("bar", _ => 12, (_, __) => 11));
            Assert.AreEqual(12, map.AddOrUpdate("baz", _ => 12, (_, __) => 11));
        }

        [Test]
        [Category("LongRunning")]
        public void AddParallelWithoutDuplicateTest()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var map = Setup();
                    var index = 0;

                    ParallelTestHelper.ParallelStressTest
                    (
                        () =>
                        {
                            var own = Interlocked.Increment(ref index);

                            while (!map.TryAdd("monkey" + own, own))
                            {
                                // Empty
                            }
                        }, 4
                    );

                    Assert.AreEqual(7, map.Count);

                    Assert.IsTrue(map.TryGetValue("monkey1", out var value), "#1");
                    Assert.AreEqual(1, value, "#1b");

                    Assert.IsTrue(map.TryGetValue("monkey2", out value), "#2");
                    Assert.AreEqual(2, value, "#2b");

                    Assert.IsTrue(map.TryGetValue("monkey3", out value), "#3");
                    Assert.AreEqual(3, value, "#3b");

                    Assert.IsTrue(map.TryGetValue("monkey4", out value), "#4");
                    Assert.AreEqual(4, value, "#4b");
                }
            );
        }

        [Test]
        public void AddWithDuplicate()
        {
            var map = Setup();
            Assert.IsFalse(map.TryAdd("foo", 6));
        }

        [Test]
        public void AddWithoutDuplicateTest()
        {
            var map = Setup();
            map.TryAdd("baz", 2);

            Assert.IsTrue(map.TryGetValue("baz", out var val));
            Assert.AreEqual(2, val);
            Assert.AreEqual(2, map["baz"]);
            Assert.AreEqual(4, map.Count);
        }

        [Test]
        public void ContainsKeyPairTest()
        {
            var validKeyPair = new KeyValuePair<string, string>("key", "validValue");
            var wrongKeyPair = new KeyValuePair<string, string>("key", "wrongValue");

            IDictionary<string, string> dict = new ConcurrentDictionary<string, string>();
            dict.Add(validKeyPair); // Do not change to object initialization

            Assert.IsTrue(dict.Contains(validKeyPair));
            Assert.IsFalse(dict.Contains(wrongKeyPair));
        }

        [Test]
        public void ContainsTest()
        {
            var map = Setup();
            Assert.IsTrue(map.ContainsKey("foo"));
            Assert.IsTrue(map.ContainsKey("bar"));
            Assert.IsTrue(map.ContainsKey("foobar"));
            Assert.IsFalse(map.ContainsKey("baz"));
            Assert.IsFalse(map.ContainsKey("oof"));
        }

        [Test]
        public void DictionaryNullOnNonExistingKey()
        {
            var val = ((IDictionary)new ConcurrentDictionary<long, string>())[1234L];
            Assert.IsNull(val);
        }

        [Test]
        public void GetOrAddTest()
        {
            var map = Setup();
            Assert.AreEqual(1, map.GetOrAdd("foo", _ => 12));
            Assert.AreEqual(13, map.GetOrAdd("baz", _ => 13));
        }

        [Test]
        public void GetValueTest()
        {
            var map = Setup();
            Assert.AreEqual(1, map["foo"], "#1");
            Assert.AreEqual(2, map["bar"], "#2");
            Assert.AreEqual(3, map.Count, "#3");
        }

        [Test]
        public void GetValueUnknownTest()
        {
            var map = Setup();
            Assert.IsFalse(map.TryGetValue("barFoo", out _));
            Assert.Throws<KeyNotFoundException>(() => GC.KeepAlive(map["barFoo"]));
        }

        [Test]
        public void InitWithEnumerableTest()
        {
            int[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var dictionary = new ConcurrentDictionary<int, int>(data.ToDictionary(x => x));

            foreach (var index in data)
            {
                Assert.IsTrue(dictionary.ContainsKey(index));
                Assert.IsTrue(dictionary.TryGetValue(index, out var val));
                Assert.AreEqual(index, val);
            }
        }

        [Test]
        public void IterateTest()
        {
            string[] keys = { "foo", "bar", "foobar" };
            var occurence = new int[3];
            var map = Setup();
            foreach (var kvp in map)
            {
                var index = Array.IndexOf(keys, kvp.Key);
                Assert.AreNotEqual(-1, index, "#a");
                Assert.AreEqual(index + 1, kvp.Value, "#b");
                Assert.That(++occurence[index], Is.LessThan(2), "#c");
            }
        }

        [Test]
        public void ModificationTest()
        {
            var map = Setup();
            map["foo"] = 9;

            Assert.AreEqual(9, map["foo"], "#1");
            Assert.IsTrue(map.TryGetValue("foo", out var val), "#3");
            Assert.AreEqual(9, val, "#4");
        }

        [Test]
        public void NullArgumentsTest()
        {
            var map = Setup();
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertThrowsArgumentNullException(() => GC.KeepAlive(map[null]));
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertThrowsArgumentNullException(() => map[null] = 0);
            AssertThrowsArgumentNullException(() => map.AddOrUpdate(null, _ => 0, (_, v) => v));
            AssertThrowsArgumentNullException(() => map.AddOrUpdate("", null, (_, v) => v));
            AssertThrowsArgumentNullException(() => map.AddOrUpdate("", _ => 0, null));
            AssertThrowsArgumentNullException(() => map.AddOrUpdate(null, 0, (_, v) => v));
            AssertThrowsArgumentNullException(() => map.AddOrUpdate("", 0, null));
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertThrowsArgumentNullException(() => GC.KeepAlive(map.ContainsKey(null)));
            AssertThrowsArgumentNullException(() => map.GetOrAdd(null, 0));
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertThrowsArgumentNullException(() => map.TryGetValue(null, out _));
            AssertThrowsArgumentNullException(() => map.TryRemove(null, out _));
            AssertThrowsArgumentNullException(() => map.TryUpdate(null, 0, 0));
        }

        [Test]
        public void QueryWithSameHashCodeTest()
        {
            var ids = new[]
            {
                34359738370,
                34359738371,
                34359738372,
                34359738373,
                34359738374,
                34359738375,
                34359738376,
                34359738377,
                34359738420
            };

            var dict = new ConcurrentDictionary<long, long>();
            long result;

            for (var i = 0; i < 20; i++)
            {
                dict[-i] = -i * 1000;
            }

            foreach (var id in ids)
            {
                Assert.IsFalse(dict.TryGetValue(id, out result), id.ToString());
            }

            foreach (var id in ids)
            {
                Assert.IsTrue(dict.TryAdd(id, id));
                Assert.AreEqual(id, dict[id]);
            }

            foreach (var id in ids)
            {
                Assert.IsTrue(dict.TryRemove(id, out result));
                Assert.AreEqual(id, result);
            }

            foreach (var id in ids)
            {
                Assert.IsFalse(dict.TryGetValue(id, out result), id + " (second)");
            }
        }

        [Test]
        [Category("LongRunning")]
        public void RemoveParallelTest()
        {
            ParallelTestHelper.Repeat
            (
                () =>
                {
                    var map = Setup();
                    var index = 0;
                    var r1 = false;
                    var r2 = false;
                    var r3 = false;

                    ParallelTestHelper.ParallelStressTest
                    (
                        () =>
                        {
                            var own = Interlocked.Increment(ref index);
                            switch (own)
                            {
                                case 1:
                                    r1 = map.TryRemove("foo", out _);
                                    break;

                                case 2:
                                    r2 = map.TryRemove("bar", out _);
                                    break;

                                case 3:
                                    r3 = map.TryRemove("foobar", out _);
                                    break;

                                default:
                                    break;
                            }
                        },
                        3
                    );

                    Assert.AreEqual(0, map.Count);

                    Assert.IsTrue(r1, "1");
                    Assert.IsTrue(r2, "2");
                    Assert.IsTrue(r3, "3");

                    Assert.IsFalse(map.TryGetValue("foo", out var value), "#1b " + value);
                    Assert.IsFalse(map.TryGetValue("bar", out value), "#2b");
                    Assert.IsFalse(map.TryGetValue("foobar", out value), "#3b");
                }
            );
        }

        [Test]
        public void SameHashCodeInsertTest()
        {
            var classMap = new ConcurrentDictionary<DumbClass, string>();

            var class1 = new DumbClass(1);
            var class2 = new DumbClass(2);

            Assert.IsTrue(classMap.TryAdd(class1, nameof(class1)), "class 1");
            Console.WriteLine(string.Empty);
            Assert.IsTrue(classMap.TryAdd(class2, nameof(class2)), "class 2");

            Assert.AreEqual(nameof(class1), classMap[class1], "class 1 check");
            Assert.AreEqual(nameof(class2), classMap[class2], "class 2 check");
        }

        [Test]
        public void TryUpdateTest()
        {
            var map = Setup();
            Assert.IsFalse(map.TryUpdate("foo", 12, 11));
            Assert.AreEqual(1, map["foo"]);
            Assert.IsTrue(map.TryUpdate("foo", 11, 1));
            Assert.AreEqual(11, map["foo"]);
        }

        private static void AssertThrowsArgumentNullException(Action action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                action();
                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ex)
            {
                No.Op(ex);
            }
        }

        private sealed class DumbClass : IEquatable<DumbClass>
        {
            private readonly int _foo;

            public DumbClass(int foo)
            {
                _foo = foo;
            }

            public bool Equals(DumbClass other)
            {
                return other != null && _foo == other._foo;
            }

            public override bool Equals(object obj)
            {
                return obj is DumbClass temp && Equals(temp);
            }

            public override int GetHashCode()
            {
                return 5;
            }
        }
    }
}