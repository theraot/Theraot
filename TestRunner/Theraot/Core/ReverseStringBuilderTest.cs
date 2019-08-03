#pragma warning disable CA1062 // Validate arguments of public methods

using System;
using Theraot.Core;

namespace TestRunner.Theraot.Core
{
    [TestFixture]
    public static class ReverseStringBuilderTest
    {
        [Test]
        public static void ConstructorWithNegativeCapacityThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException, ReverseStringBuilder>(() => new ReverseStringBuilder(-1));
        }

        [Test]
        public static void LengthIncreasesOnPrepend(string textA, string textB)
        {
            var builder = new ReverseStringBuilder(100);
            Assert.AreEqual(builder.Length, 0);
            builder.Prepend(textA);
            Assert.AreEqual(builder.Length, textA.Length);
            builder.Prepend(textB);
            Assert.AreEqual(builder.Length, textA.Length + textB.Length);
        }

        [Test]
        public static void LengthStartsAtZero()
        {
            var builder = new ReverseStringBuilder(10);
            Assert.AreEqual(builder.Length, 0);
        }

        [Test]
        public static void PrependBeyondCapacityThrows(string text)
        {
            var builder = new ReverseStringBuilder(0);
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.Prepend(text));
        }

        [Test]
        public static void ToStringTakesRange()
        {
            var builder = new ReverseStringBuilder(100);
            builder.Prepend("0123456789");
            Assert.AreEqual("0", builder.ToString(builder.Length, 1));
            Assert.AreEqual("12345", builder.ToString(builder.Length - 1, 5));
        }

        [Test]
        public static void ToStringWithBackIndexBeyondCapacityThrows()
        {
            var builder = new ReverseStringBuilder(10);
            builder.Prepend("0123456789");
            Assert.Throws<ArgumentOutOfRangeException, string>(() => builder.ToString(builder.Length + 1, 1));
        }

        [Test]
        public static void ToStringWithBackIndexBeyondLengthReturnsNullChars()
        {
            var builder = new ReverseStringBuilder(20);
            builder.Prepend("0123456789");
            Assert.AreEqual("\0", builder.ToString(builder.Length + 1, 1));
        }

        [Test]
        public static void ToStringWithNegativeBackIndexThrows()
        {
            var builder = new ReverseStringBuilder(10);
            builder.Prepend("0123456789");
            Assert.Throws<ArgumentOutOfRangeException, string>(() => builder.ToString(-1, 1));
        }

        [Test]
        public static void ToStringWithNegativeBackIndexThrowsEvenWithCapacity()
        {
            var builder = new ReverseStringBuilder(20);
            builder.Prepend("0123456789");
            Assert.Throws<ArgumentOutOfRangeException, string>(() => builder.ToString(-1, 1));
        }
    }
}