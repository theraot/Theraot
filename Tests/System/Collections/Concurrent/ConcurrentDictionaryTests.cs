#define NET_4_0
#if NET_4_0
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
//
//

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MonoTests.System.Collections.Concurrent
{
    [TestFixture]
    public class ConcurrentDictionaryTests
    {
        private ConcurrentDictionary<string, int> _map;

        [Test]
        public void AddOrUpdateTest()
        {
            Assert.AreEqual(11, _map.AddOrUpdate("bar", (_) => 12, (_, __) => 11));
            Assert.AreEqual(12, _map.AddOrUpdate("baz", (_) => 12, (_, __) => 11));
        }

        [Test]
        public void AddParallelWithoutDuplicateTest()
        {
            ParallelTestHelper.Repeat(delegate
            {
                Setup();
                var index = 0;

                ParallelTestHelper.ParallelStressTest(_map, delegate
                {
                    var own = Interlocked.Increment(ref index);

                    while (!_map.TryAdd("monkey" + own.ToString(), own))
                    {
                    }
                }, 4);

                Assert.AreEqual(7, _map.Count);
                int value;

                Assert.IsTrue(_map.TryGetValue("monkey1", out value), "#1");
                Assert.AreEqual(1, value, "#1b");

                Assert.IsTrue(_map.TryGetValue("monkey2", out value), "#2");
                Assert.AreEqual(2, value, "#2b");

                Assert.IsTrue(_map.TryGetValue("monkey3", out value), "#3");
                Assert.AreEqual(3, value, "#3b");

                Assert.IsTrue(_map.TryGetValue("monkey4", out value), "#4");
                Assert.AreEqual(4, value, "#4b");
            });
        }

        [Test]
        public void AddWithDuplicate()
        {
            Assert.IsFalse(_map.TryAdd("foo", 6));
        }

        [Test]
        public void AddWithoutDuplicateTest()
        {
            _map.TryAdd("baz", 2);
            int val;

            Assert.IsTrue(_map.TryGetValue("baz", out val));
            Assert.AreEqual(2, val);
            Assert.AreEqual(2, _map["baz"]);
            Assert.AreEqual(4, _map.Count);
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
            Assert.IsTrue(_map.ContainsKey("foo"));
            Assert.IsTrue(_map.ContainsKey("bar"));
            Assert.IsTrue(_map.ContainsKey("foobar"));
            Assert.IsFalse(_map.ContainsKey("baz"));
            Assert.IsFalse(_map.ContainsKey("oof"));
        }

        [Test]
        public void GetOrAddTest()
        {
            Assert.AreEqual(1, _map.GetOrAdd("foo", (_) => 12));
            Assert.AreEqual(13, _map.GetOrAdd("baz", (_) => 13));
        }

        [Test]
        public void GetValueTest()
        {
            Assert.AreEqual(1, _map["foo"], "#1");
            Assert.AreEqual(2, _map["bar"], "#2");
            Assert.AreEqual(3, _map.Count, "#3");
        }

        [Test, ExpectedException(typeof(KeyNotFoundException))]
        public void GetValueUnknownTest()
        {
            int val;
            Assert.IsFalse(_map.TryGetValue("barfoo", out val));
            val = _map["barfoo"];
        }

        [Test]
        public void DictionaryNullOnNonExistingKey()
        {
            IDictionary dict = new ConcurrentDictionary<long, string>();
            var val = dict[1234L];
            Assert.IsNull(val);
        }

        [Test]
        public void InitWithEnumerableTest()
        {
            int[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var ndic = data.ToDictionary(x => x);
            var cdic = new ConcurrentDictionary<int, int>(ndic);

            foreach (var index in data)
            {
                Assert.IsTrue(cdic.ContainsKey(index));
                int val;
                Assert.IsTrue(cdic.TryGetValue(index, out val));
                Assert.AreEqual(index, val);
            }
        }

        [Test]
        public void IterateTest()
        {
            string[] keys = { "foo", "bar", "foobar" };
            var occurence = new int[3];

            foreach (var kvp in _map)
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
            _map["foo"] = 9;
            int val;

            Assert.AreEqual(9, _map["foo"], "#1");
            Assert.IsTrue(_map.TryGetValue("foo", out val), "#3");
            Assert.AreEqual(9, val, "#4");
        }

        [Test]
        public void NullArgumentsTest()
        {
            AssertThrowsArgumentNullException(() =>
            {
                var x = _map[null];
            });
            AssertThrowsArgumentNullException(() => _map[null] = 0);
            AssertThrowsArgumentNullException(() => _map.AddOrUpdate(null, k => 0, (k, v) => v));
            AssertThrowsArgumentNullException(() => _map.AddOrUpdate("", null, (k, v) => v));
            AssertThrowsArgumentNullException(() => _map.AddOrUpdate("", k => 0, null));
            AssertThrowsArgumentNullException(() => _map.AddOrUpdate(null, 0, (k, v) => v));
            AssertThrowsArgumentNullException(() => _map.AddOrUpdate("", 0, null));
            AssertThrowsArgumentNullException(() => _map.ContainsKey(null));
            AssertThrowsArgumentNullException(() => _map.GetOrAdd(null, 0));
            int value;
            AssertThrowsArgumentNullException(() => _map.TryGetValue(null, out value));
            AssertThrowsArgumentNullException(() => _map.TryRemove(null, out value));
            AssertThrowsArgumentNullException(() => _map.TryUpdate(null, 0, 0));
        }

        [Test]
        public void QueryWithSameHashCodeTest()
        {
            var ids = new[] {
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
                Assert.IsFalse(dict.TryGetValue(id, out result), id.ToString() + " (second)");
            }
        }

        [Test]
        public void RemoveParallelTest()
        {
            ParallelTestHelper.Repeat(delegate
            {
                Setup();
                var index = 0;
                var r1 = false;
                var r2 = false;
                var r3 = false;
                int val;

                ParallelTestHelper.ParallelStressTest(_map, delegate
                {
                    var own = Interlocked.Increment(ref index);
                    switch (own)
                    {
                        case 1:
                            r1 = _map.TryRemove("foo", out val);
                            break;

                        case 2:
                            r2 = _map.TryRemove("bar", out val);
                            break;

                        case 3:
                            r3 = _map.TryRemove("foobar", out val);
                            break;
                    }
                }, 3);

                Assert.AreEqual(0, _map.Count);
                int value;

                Assert.IsTrue(r1, "1");
                Assert.IsTrue(r2, "2");
                Assert.IsTrue(r3, "3");

                Assert.IsFalse(_map.TryGetValue("foo", out value), "#1b " + value.ToString());
                Assert.IsFalse(_map.TryGetValue("bar", out value), "#2b");
                Assert.IsFalse(_map.TryGetValue("foobar", out value), "#3b");
            });
        }

        [Test]
        public void SameHashCodeInsertTest()
        {
            var classMap = new ConcurrentDictionary<DumbClass, string>();

            var class1 = new DumbClass(1);
            var class2 = new DumbClass(2);

            Assert.IsTrue(classMap.TryAdd(class1, "class1"), "class 1");
            Console.WriteLine();
            Assert.IsTrue(classMap.TryAdd(class2, "class2"), "class 2");

            Assert.AreEqual("class1", classMap[class1], "class 1 check");
            Assert.AreEqual("class2", classMap[class2], "class 2 check");
        }

        [SetUp]
        public void Setup()
        {
            _map = new ConcurrentDictionary<string, int>();
            AddStuff();
        }

        [Test]
        public void TryUpdateTest()
        {
            Assert.IsFalse(_map.TryUpdate("foo", 12, 11));
            Assert.AreEqual(1, _map["foo"]);
            Assert.IsTrue(_map.TryUpdate("foo", 11, 1));
            Assert.AreEqual(11, _map["foo"]);
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
                GC.KeepAlive(ex);
            }
        }

        private void AddStuff()
        {
            _map.TryAdd("foo", 1);
            _map.TryAdd("bar", 2);
            _map["foobar"] = 3;
        }

        private class DumbClass : IEquatable<DumbClass>
        {
            private readonly int _foo;

            public DumbClass(int foo)
            {
                _foo = foo;
            }

            public int Foo
            {
                get { return _foo; }
            }

            public override bool Equals(object obj)
            {
                var temp = obj as DumbClass;
                return temp != null && Equals(temp);
            }

            public bool Equals(DumbClass other)
            {
                return _foo == other._foo;
            }

            public override int GetHashCode()
            {
                return 5;
            }
        }
    }
}

#endif