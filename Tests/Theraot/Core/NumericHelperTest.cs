using NUnit.Framework;
using System;
using Theraot.Core;

namespace Tests.Theraot.Core
{
    [TestFixture]
    internal class NumericHelperTest
    {
#if FAT

        [Test]
        public void Decrements()
        {
            Assert.AreEqual(int.MaxValue, NumericHelper.UncheckedDecrement(int.MinValue));
            Assert.AreEqual(short.MaxValue, NumericHelper.UncheckedDecrement(short.MinValue));
            Assert.AreEqual(long.MaxValue, NumericHelper.UncheckedDecrement(long.MinValue));

            Assert.Throws<OverflowException>(() => NumericHelper.CheckedDecrement(int.MinValue));
            Assert.Throws<OverflowException>(() => NumericHelper.CheckedDecrement(short.MinValue));
            Assert.Throws<OverflowException>(() => NumericHelper.CheckedDecrement(long.MinValue));
        }

        [Test]
        public void Increments()
        {
            Assert.AreEqual(int.MinValue, NumericHelper.UncheckedIncrement(int.MaxValue));
            Assert.AreEqual(short.MinValue, NumericHelper.UncheckedIncrement(short.MaxValue));
            Assert.AreEqual(long.MinValue, NumericHelper.UncheckedIncrement(long.MaxValue));

            Assert.Throws<OverflowException>(() => NumericHelper.CheckedIncrement(int.MaxValue));
            Assert.Throws<OverflowException>(() => NumericHelper.CheckedIncrement(short.MaxValue));
            Assert.Throws<OverflowException>(() => NumericHelper.CheckedIncrement(long.MaxValue));
        }

#endif

        [Test]
        public void Log2Tests()
        {
            var input = new int[] { 22141034, 146009798, 106447544, 66083576, 28048294, 3848650, 119601527, 182384611, 160860217, 52726162 };
            var output = new int[] { 24, 27, 26, 25, 24, 21, 26, 27, 27, 25 };
            for (int index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], NumericHelper.Log2(input[index]));
            }
            Assert.AreEqual(0, NumericHelper.Log2(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumericHelper.Log2(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => NumericHelper.Log2(-1));
        }

        [Test]
        public void NextPowerOf2Int()
        {
            var input = new int[] { -1,  835, 246444, 448905, 610268, 855701, 1017047, 1228188, 1717470, 1917025, 2130772 };
            var output = new int[] { 1, 1024, 262144, 524288, 1048576, 1048576, 1048576, 2097152, 2097152, 2097152, 4194304 };
            for (int index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], NumericHelper.NextPowerOf2(input[index]));
            }
        }

        [Test]
        public void NextPowerOf2Uint()
        {
            var input = new uint[] { 835, 246444, 448905, 610268, 855701, 1017047, 1228188, 1717470, 1917025, 2130772 };
            var output = new uint[] { 1024, 262144, 524288, 1048576, 1048576, 1048576, 2097152, 2097152, 2097152, 4194304 };
            for (int index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], NumericHelper.NextPowerOf2(input[index]));
            }
        }

        [Test]
        public void Sqrt()
        {
            var input = new int[] { 835, 246444, 448905, 610268, 855701, 1017047, 1228188, 1717470, 1917025, 2130772 };
            var output = new int[] { 28, 496, 670, 781, 925, 1008, 1108, 1310, 1384, 1459 };
            for (int index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], NumericHelper.Sqrt(input[index]));
            }
        }
    }
}