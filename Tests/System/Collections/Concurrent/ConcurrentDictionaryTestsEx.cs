using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace MonoTests.System.Collections.Concurrent
{
    [TestFixture]
    public class ConcurrentDictionaryTestsEx
    {
        [Test]
        public void EditWhileIterating()
        {
            var d = new ConcurrentDictionary<string, string>();
            Assert.IsTrue(d.TryAdd("0", "1"));
            Assert.IsTrue(d.TryAdd("a", "b"));
            var expectedCount = 2;
            Assert.AreEqual(expectedCount, d.Count);
            string a = null;
            var foundCount = 0;
            var didAdd = false;
            var didRemove = false;
            var found = new CircularBucket<string>(16);
            // MSDN says: "it does not represent a moment-in-time snapshot of the dictionary."
            // And also "The contents exposed through the enumerator may contain modifications made to the dictionary after GetEnumerator was called."
            foreach (var item in d)
            {
                found.Add(item.Key);
                foundCount++;
                Assert.AreEqual(expectedCount, d.Count);
                didRemove = d.TryRemove("a", out a);
                if (foundCount == 1)
                {
                    Assert.IsTrue(didRemove);
                    expectedCount--;
                }
                else
                {
                    Assert.IsFalse(didRemove);
                }
                Assert.AreEqual(expectedCount, d.Count);
                var added = d.TryAdd("c", "d");
                if (foundCount == 1)
                {
                    Assert.IsTrue(added);
                }
                else
                {
                    Assert.IsFalse(added);
                }
                if (!didAdd && added)
                {
                    expectedCount++;
                }
                didAdd = didAdd || added;
                Assert.AreEqual(expectedCount, d.Count);
            }
            Assert.IsNull(a);
            var array = found.ToArray();
            if (!array.SetEquals(new[] { "0", "c" }))
            {
                foreach (var item in array)
                {
                    Console.WriteLine(item);
                }
                Assert.Fail();
            }
            Assert.AreEqual(2, expectedCount);
            Assert.AreEqual(true, didAdd);
            Assert.AreEqual(false, didRemove);
            Assert.AreEqual(expectedCount, foundCount);
            Assert.AreEqual(expectedCount, d.Count);
        }

        [Test]
        public void EditWhileIteratingThreaded()
        {
            var d = new ConcurrentDictionary<string, string>();
            Assert.IsTrue(d.TryAdd("0", "1"));
            Assert.IsTrue(d.TryAdd("a", "b"));
            int[] expectedCount = { 2 };
            Assert.AreEqual(expectedCount[0], d.Count);
            string a = null;
            var foundCount = 0;
            var didAdd = 0;
            var didRemove = 0;
            var found = new CircularBucket<string>(16);

            ThreadStart remover = () =>
            {
                var removed = d.TryRemove("a", out a);
                if (Thread.VolatileRead(ref didRemove) == 0 && removed)
                {
                    expectedCount[0]--;
                }
                if (removed)
                {
                    Interlocked.CompareExchange(ref didRemove, 1, 0);
                }
            };

            ThreadStart adder = () =>
            {
                var added = d.TryAdd("c", "d");
                if (Thread.VolatileRead(ref didAdd) == 0 && added)
                {
                    expectedCount[0]++;
                }
                if (added)
                {
                    Interlocked.CompareExchange(ref didAdd, 1, 0);
                }
            };

            // MSDN says: "it does not represent a moment-in-time snapshot of the dictionary."
            // And also "The contents exposed through the enumerator may contain modifications made to the dictionary after GetEnumerator was called."
            foreach (var item in d)
            {
                found.Add(item.Key);
                foundCount++;
                var old = expectedCount[0];
                Assert.AreEqual(expectedCount[0], d.Count);
                {
                    var t = new Thread(remover);
                    t.Start();
                    t.Join();
                }
                if (foundCount == 1)
                {
                    Assert.AreNotEqual(old, expectedCount[0]);
                }
                else
                {
                    Assert.AreEqual(old, expectedCount[0]);
                }
                Assert.AreEqual(expectedCount[0], d.Count);
                old = expectedCount[0];
                {
                    var t = new Thread(adder);
                    t.Start();
                    t.Join();
                }
                if (foundCount == 1)
                {
                    Assert.AreNotEqual(old, expectedCount[0]);
                }
                else
                {
                    Assert.AreEqual(old, expectedCount[0]);
                }
                Assert.AreEqual(expectedCount[0], d.Count);
            }
            Assert.IsNull(a);
            var array = found.ToArray();
            if (!array.SetEquals(new[] { "0", "c" }))
            {
                foreach (var item in array)
                {
                    Console.WriteLine(item);
                }
                Assert.Fail();
            }
            Assert.AreEqual(2, expectedCount[0]);
            Assert.AreEqual(1, didAdd);
            Assert.AreEqual(1, didRemove);
            Assert.AreEqual(expectedCount[0], foundCount);
            Assert.AreEqual(expectedCount[0], d.Count);
        }

        [Test]
        public void InitWithConflictingData()
        {
            var data = new List<KeyValuePair<int, int>>
            {
                new KeyValuePair<int, int>(0, 0),
                new KeyValuePair<int, int>(0, 1)
            };
            Assert.Throws<ArgumentException>(() => new ConcurrentDictionary<int, int>(data));
        }

        [Test]
        public void NullOnNonExistingKey()
        {
            var dict = new ConcurrentDictionary<long, string>();
            Assert.Throws<KeyNotFoundException>(() => GC.KeepAlive(dict[1234L]));
        }

        [Test]
        public void PairCollide()
        {
            var firstPair = new KeyValuePair<string, string>("key", "validValue");
            var secondPair = new KeyValuePair<string, string>("key", "wrongValue");

            IDictionary<string, string> dict = new ConcurrentDictionary<string, string>();
            dict.Add(firstPair); // Do not change to object initialization
            Assert.Throws<ArgumentException>(() => dict.Add(secondPair));

            Assert.IsTrue(dict.Contains(firstPair));
            Assert.IsFalse(dict.Contains(secondPair));
        }

        [Test]
        public void SimpleTest()
        {
            ConcurrentDictionary<string, int> map;
            map = new ConcurrentDictionary<string, int>();
            Assert.AreEqual(0, map.Count);
            map.TryAdd("foo", 1);
            Assert.AreEqual(1, map.Count);
            map.TryAdd("bar", 2);
            Assert.AreEqual(2, map.Count);
            map["foobar"] = 3;
            Assert.AreEqual(3, map.Count);
        }

        [Test]
        public void UnexpectedAddAndRemove()
        {
            var dict = new ConcurrentDictionary<string, string>();
            Assert.Throws<ArgumentNullException>(() => ((ICollection<KeyValuePair<string, string>>)dict).Add(new KeyValuePair<string, string>(null, null)));
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(((ICollection<KeyValuePair<string, string>>)dict).Remove(new KeyValuePair<string, string>(null, null))));
        }

        [Test]
        public void UnexpectedContains()
        {
            var dict = new ConcurrentDictionary<string, string>();
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(dict.ContainsKey(null)));
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(((IDictionary)dict).Contains(null)));
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(((ICollection<KeyValuePair<string, string>>)dict).Contains(new KeyValuePair<string, string>(null, null))));
            Assert.IsFalse(((IDictionary)dict).Contains(8));
        }
    }
}