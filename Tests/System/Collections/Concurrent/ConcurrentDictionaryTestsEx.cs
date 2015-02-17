using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NUnit.Framework;

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
            int expectedCount = 2;
            Assert.AreEqual(expectedCount, d.Count);
            string a = null;
            var foundCount = 0;
            // MSDN says: "it does not represent a moment-in-time snapshot of the dictionary."
            // And also "The contents exposed through the enumerator may contain modifications made to the dictionary after GetEnumerator was called."
            foreach (var item in d)
            {
                foundCount++;
                Assert.AreEqual(expectedCount, d.Count);
                var didRemove = d.TryRemove("a", out a);
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
                var didAdd = d.TryAdd("c", "d");
                if (foundCount == 1)
                {
                    Assert.IsTrue(didAdd);
                    expectedCount++;
                }
                else
                {
                    Assert.IsFalse(didAdd);
                }
                Assert.AreEqual(expectedCount, d.Count);
            }
            Assert.IsNull(a);
            Assert.AreEqual(2, foundCount);
            Assert.AreEqual(2, d.Count);
        }

        [Test]
        public void InitWithConflictingData()
        {
            var data = new List<KeyValuePair<int, int>>();
            data.Add(new KeyValuePair<int, int>(0, 0));
            data.Add(new KeyValuePair<int, int>(0, 1));
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
            dict.Add(firstPair);
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