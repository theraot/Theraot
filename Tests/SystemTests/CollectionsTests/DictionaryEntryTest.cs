#if TARGETS_NET || LESSTHAN_NETCORE20 || LESSTHAN_NETSTANDARD21

#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System.Collections;

namespace Tests.SystemTests.CollectionsTests
{
    [TestFixture]
    class DictionaryEntryTest
    {
        [Test]
        public void Deconstruct()
        {
            DictionaryEntry dictionaryEntry = new DictionaryEntry(1, "2");
            (object key, object value) = dictionaryEntry;
            Assert.AreEqual(1, key);
            Assert.AreEqual("2", value);
        }
    }
}

#endif