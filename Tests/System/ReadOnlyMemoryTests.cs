#if LESSTHAN_NET45

using System;
using NUnit.Framework;

namespace Tests.System
{
    [TestFixture]
    class ReadOnlyMemoryTests
    {
        [Test]
        public void Constructor()
        {
            var memory = new ReadOnlyMemory<int>(new[] { 1, 2, 3 });
            Assert.AreEqual(1, memory.Span[0]);

            Assert.AreEqual(0, new ReadOnlyMemory<int>(null).Length);
        }
    }
}

#endif