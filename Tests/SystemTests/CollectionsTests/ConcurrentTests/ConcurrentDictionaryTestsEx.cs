﻿#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests.SystemTests.CollectionsTests.ConcurrentTests
{
    [TestFixture]
    public partial class ConcurrentDictionaryTestsEx
    {
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
            var map = new ConcurrentDictionary<string, int>();
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
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(dict.ContainsKey(null)));
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(((IDictionary)dict).Contains(null)));
            Assert.Throws<ArgumentNullException>(() => GC.KeepAlive(((ICollection<KeyValuePair<string, string>>)dict).Contains(new KeyValuePair<string, string>(null, null))));
            Assert.IsFalse(((IDictionary)dict).Contains(8));
        }
    }
}
