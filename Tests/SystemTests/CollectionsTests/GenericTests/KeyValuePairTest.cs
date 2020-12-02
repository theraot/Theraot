#if TARGETS_NET || LESSTHAN_NETCORE20 || LESSTHAN_NETSTANDARD21

#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.SystemTests.CollectionsTests.GenericTests
{
    [TestFixture]
    internal class KeyValuePairTest
    {
        [Test]
        public void Create_ReturnsExpected()
        {
            KeyValuePair<int, string> keyValuePair = KeyValuePair.Create(1, "2");
            Assert.AreEqual(1, keyValuePair.Key);
            Assert.AreEqual("2", keyValuePair.Value);
        }

        [Test]
        public void Deconstruct()
        {
            KeyValuePair<int, string> keyValuePair = KeyValuePair.Create(1, "2");
            (int key, string value) = keyValuePair;
            Assert.AreEqual(1, key);
            Assert.AreEqual("2", value);
        }
    }
}

#endif