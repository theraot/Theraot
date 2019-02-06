#if LESSTHAN_NET40

using NUnit.Framework;
using System;

namespace Tests.System
{
    [TestFixture]
    public class EnumTheraotExtensionsTest
    {
        [Flags]
        private enum WithZero
        {
            Zero = 0,
            One = 1,
        }

        [Flags]
        private enum WithoutZero
        {
            Two = 2,
        }


        [Test]
        public ThrowsOnNullEnum()
        {
            Assert.Throws<ArgumentNullException>(() => EnumTheraotExtensions.HasFlag((Enum)null, WithZero.One));
        }

        [Test]
        public ThrowsOnNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => EnumTheraotExtensions.HasFlag(WithZero.One, (Enum)null));
        }

        [Test]
        public HasFlagWithEnumHavingZeroValue()
        {
            Assert.IsTrue(WithZero.One.HasFlag(WithZero.Zero));
        }

        [Test]
        public HasFlagWithEnumNotHavingZeroValue()
        {
            Assert.IsTrue(WithoutZero.Two.HasFlag((WithoutZero)0));
        }
    }
}

#endif