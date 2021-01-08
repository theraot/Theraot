#if LESSTHAN_NET45

using System;
using NUnit.Framework;

namespace Tests.System
{
    [TestFixture]
    class SpanTests
    {
        [Test]
        public void Constructor()
        {
            var span = new Span<int>(new[] {1, 2, 3});
            Assert.AreEqual(1, span[0]);


            Assert.AreEqual(0, new Span<int>(null).Length);
        }
    }
}

#endif
