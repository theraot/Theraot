#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

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
            // Note: There is no guarantee the items are in insert order
            foreach (var item in d)
            {
                found.Add(item.Key);
                foundCount++;
                Assert.AreEqual(expectedCount, d.Count);
                var removed = d.TryRemove("a", out a);
                if (didRemove)
                {
                    Assert.IsFalse(removed);
                }
                else
                {
                    Assert.IsTrue(removed);
                    expectedCount--;
                }
                didRemove = didRemove | removed;
                Assert.IsTrue(didRemove);
                Assert.AreEqual(expectedCount, d.Count);
                var added = d.TryAdd("c", "d");
                if (didAdd)
                {
                    Assert.IsFalse(added);
                }
                else
                {
                    Assert.IsTrue(added);
                    expectedCount++;
                }
                didAdd = didAdd | added;
                Assert.IsTrue(didAdd);
                Assert.AreEqual(expectedCount, d.Count);
            }
            Assert.IsNull(a);
            var array = found.ToArray();
            if (!array.IsSupersetOf(new[] { "0", "c" }))
            {
                foreach (var item in array)
                {
                    Console.WriteLine(item);
                }
                Assert.Fail();
            }
            Assert.AreEqual(2, expectedCount);
            Assert.AreEqual(true, didAdd);
            Assert.AreEqual(true, didRemove);
            Assert.IsTrue(foundCount - expectedCount < 2, "foundCount: {0}, expectedCount:{1}", foundCount, expectedCount);
            Assert.AreEqual(expectedCount, d.Count);
        }

        [Test]
        [Category("LongRunning")]
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

            void Remover()
            {
                var removed = d.TryRemove("a", out a);
                if (Volatile.Read(ref didRemove) == 0 && removed)
                {
                    expectedCount[0]--;
                }
                if (removed)
                {
                    Interlocked.CompareExchange(ref didRemove, 1, 0);
                }
            }

            void Adder()
            {
                var added = d.TryAdd("c", "d");
                if (Volatile.Read(ref didAdd) == 0 && added)
                {
                    expectedCount[0]++;
                }

                if (added)
                {
                    Interlocked.CompareExchange(ref didAdd, 1, 0);
                }
            }

            // MSDN says: "it does not represent a moment-in-time snapshot of the dictionary."
            // And also "The contents exposed through the enumerator may contain modifications made to the dictionary after GetEnumerator was called."
            foreach (var item in d)
            {
                found.Add(item.Key);
                foundCount++;
                var old = expectedCount[0];
                Assert.AreEqual(expectedCount[0], d.Count);
                {
                    var t = new Thread(Remover);
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
                    var t = new Thread(Adder);
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
            if (!array.IsSupersetOf(new[] { "0", "c" }))
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
            Assert.IsTrue(foundCount - expectedCount[0] < 2, "foundCount: {0}, expectedCount:{1}", foundCount, expectedCount[0]);
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
            Assert.Throws<ArgumentException>(() => GC.KeepAlive(new ConcurrentDictionary<int, int>(data)));
        }

        [Test]
        public void NullOnNonExistingKey()
        {
            Assert.Throws<KeyNotFoundException>(() => GC.KeepAlive(new ConcurrentDictionary<long, string>()[1234L]));
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