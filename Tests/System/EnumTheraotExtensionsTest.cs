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
        public void ThrowsOnNullEnum()
        {
            Assert.Throws<ArgumentNullException>(() => ((Enum)null).HasFlag(WithZero.One));
        }

        [Test]
        public void ThrowsOnNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => WithZero.One.HasFlag((Enum)null));
        }

        [Test]
        public void HasFlagWithEnumHavingZeroValue()
        {
            Assert.IsTrue(WithZero.One.HasFlag(WithZero.Zero));
        }

        [Test]
        public void HasFlagWithEnumNotHavingZeroValue()
        {
            Assert.IsTrue(WithoutZero.Two.HasFlag((WithoutZero)0));
        }
    }
}