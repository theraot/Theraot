using NUnit.Framework;
using System;

namespace Tests.System
{
    [TestFixture]
    public class EnumExTest
    {
        [Flags]
        private enum WithZero
        {
            Zero = 0,
            One = 1,
        }

        [Test]
        public void TryParseZero()
        {
            Assert.IsTrue(EnumEx.TryParse<WithZero>("Zero", out var zero));
            Assert.AreEqual(WithZero.Zero, zero);
        }
    }
}