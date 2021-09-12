using System.Collections.Generic;
using NUnit.Framework;

#pragma warning disable IDE0004 // Unnecessary cast

namespace System.Tests
{
    public static class StringExTests
    {
        [Test]
        public static void JoinTest()
        {
            Assert.AreEqual("1,2", StringEx.Join(",", 1, 2));
            Assert.AreEqual("1,2,", StringEx.Join(",", 1, 2, null));

            Assert.AreEqual("1,2", StringEx.Join(",", "1", "2"));
            Assert.AreEqual("1,2,", StringEx.Join(",", "1", "2", null));

            Assert.AreEqual("1,2", StringEx.Join(",", new[] { "1", "2", "3" }, 0, 2));
            Assert.AreEqual("1,2,", StringEx.Join(",", new[] { "1", "2", null }, 0, 3));

            Assert.AreEqual("1,2", StringEx.Join(",", (IEnumerable<string>)new[] { "1", "2" }));
            Assert.AreEqual("1,2,", StringEx.Join(",", (IEnumerable<string>)new[] { "1", "2", null }));

            Assert.AreEqual("1,2", StringEx.Join(",", (IEnumerable<int>)new[] { 1, 2 }));
            Assert.AreEqual("1,2,", StringEx.Join(",", (IEnumerable<int?>)new int?[] { 1, 2, null }));
        }
    }
}
