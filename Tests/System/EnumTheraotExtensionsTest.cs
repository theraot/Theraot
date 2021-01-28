#pragma warning disable RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute)

using NUnit.Framework;
using System;
using Tests.Helpers;

namespace Tests.System
{
    [TestFixture]
    public class EnumTheraotExtensionsTest
    {
        [Flags]
        private enum WithoutZero
        {
            Two = 2
        }

        [Flags]
        private enum WithZero
        {
            Zero = 0,
            One = 1
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

        [Test]
        public void ThrowsOnNullEnum()
        {
            // ReSharper disable once PossibleNullReferenceException
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NullReferenceException>(() => ((Enum)null).HasFlag(WithZero.One));
        }

        [Test]
        public void ThrowsOnNullValue()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => WithZero.One.HasFlag(null));
        }
    }
}