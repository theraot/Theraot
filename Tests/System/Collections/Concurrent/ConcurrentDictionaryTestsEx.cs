using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace MonoTests.System.Collections.Concurrent
{
    [TestFixture]
    public class ConcurrentDictionaryTestsEx
    {
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
    }
}