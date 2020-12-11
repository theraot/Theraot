#if TARGETS_NET || LESSTHAN_NETCORE20 || LESSTHAN_NETSTANDARD21

#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System.Collections;

namespace Tests.SystemTests.CollectionsTests
{
    [TestFixture]
    internal class DictionaryEntryTest
    {
        [Test]
        public void Deconstruct()
        {
            (object key, object value) = new DictionaryEntry(1, "2");
            Assert.AreEqual(1, key);
            Assert.AreEqual("2", value);
        }
    }
}

#endif