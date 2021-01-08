#if LESSTHAN_NET45

using System;
using NUnit.Framework;

namespace Tests.System
{
    [TestFixture]
    class MemoryTests
    {
        [Test]
        public void Constructor()
        {
            var memory = new Memory<int>(new[] { 1, 2, 3 });
            Assert.AreEqual(1, memory.Span[0]);

            Assert.AreEqual(0, new Memory<int>(null).Length);
        }
    }
}

#endif