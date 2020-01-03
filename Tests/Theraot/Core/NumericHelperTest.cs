using NUnit.Framework;
using System;
using Theraot.Core;

namespace Tests.Theraot.Core
{
    [TestFixture]
    internal partial class NumericHelperTest
    {
        [Test]
        public void Log2Tests()
        {
            var input = new[] { 22141034, 146009798, 106447544, 66083576, 28048294, 3848650, 119601527, 182384611, 160860217, 52726162 };
            var output = new[] { 24, 27, 26, 25, 24, 21, 26, 27, 27, 25 };
            for (var index = 0; index < input.Length; index++)
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
            var input = new[] { -1, 835, 246444, 448905, 610268, 855701, 1017047, 1228188, 1717470, 1917025, 2130772 };
            var output = new[] { 1, 1024, 262144, 524288, 1048576, 1048576, 1048576, 2097152, 2097152, 2097152, 4194304 };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], NumericHelper.NextPowerOf2(input[index]));
            }
        }

        [Test]
        public void NextPowerOf2Uint()
        {
            var input = new uint[] { 835, 246444, 448905, 610268, 855701, 1017047, 1228188, 1717470, 1917025, 2130772 };
            var output = new uint[] { 1024, 262144, 524288, 1048576, 1048576, 1048576, 2097152, 2097152, 2097152, 4194304 };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], NumericHelper.NextPowerOf2(input[index]));
            }
        }

        [Test]
        public void Sqrt()
        {
            var input = new[] { 835, 246444, 448905, 610268, 855701, 1017047, 1228188, 1717470, 1917025, 2130772 };
            var output = new[] { 28, 496, 670, 781, 925, 1008, 1108, 1310, 1384, 1459 };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], NumericHelper.Sqrt(input[index]));
            }
        }
    }

    internal partial class NumericHelperTest
    {
        [Test]
        public void BuildDouble()
        {
            var inputLong = new[]
            {
                2318709486335437829,
                5863966414194722965,
                1901433884841312373,
                2370638483762077987,
                942754748826369726,
                -6658171829963888942,
                -7008354965981861999,
                -4082161363149237830,
                -3043789911413372093,
                -6411351321497233834
            };
            foreach (var x in inputLong)
            {
                CheckValue(NumericHelper.Int64AsDouble(x));
            }

            var inputDouble = new[]
            {
                2318709486335437829,
                5863966414194722965,
                1901433884841312373,
                2370638483762077987,
                942754748826369726,
                -6658171829963888942,
                -7008354965981861999,
                -4082161363149237830,
                -3043789911413372093,
                -6411351321497233834,
                double.PositiveInfinity,
                double.NegativeInfinity,
                double.NaN
            };
            foreach (var x in inputDouble)
            {
                CheckValue(x);
            }
        }

        private static void CheckValue(double value)
        {
            NumericHelper.GetParts(value, out int sign, out long mantissa, out int exponent, out bool finite);
            var check = NumericHelper.BuildDouble(sign, mantissa, exponent);
            Assert.AreEqual(finite, !double.IsInfinity(value) && !double.IsNaN(value));
            if (finite)
            {
                Assert.AreEqual(value, check);
            }
        }
    }

    internal partial class NumericHelperTest
    {
        [Test]
        public static void GCDIgnoresSign()
        {
            Assert.AreEqual(12, NumericHelper.GCD(48, 180));
            Assert.AreEqual(12, NumericHelper.GCD(-48, 180));
            Assert.AreEqual(12, NumericHelper.GCD(48, -180));
            Assert.AreEqual(12, NumericHelper.GCD(-48, -180));

            Assert.AreEqual(6, NumericHelper.GCD(48, 13456489866));
            Assert.AreEqual(6, NumericHelper.GCD(-48, 13456489866));
            Assert.AreEqual(6, NumericHelper.GCD(48, -13456489866));
            Assert.AreEqual(6, NumericHelper.GCD(-48, -13456489866));
        }

        [Test]
        public static void GCDMaxValue()
        {
            Assert.AreEqual(15, NumericHelper.GCD(ulong.MaxValue, 180));
        }

        [Test]
        public static void GCDMics()
        {
            Assert.AreEqual(1, NumericHelper.GCD(43154552, 521995751));
            Assert.AreEqual(3, NumericHelper.GCD(467216955, 136307028));
            Assert.AreEqual(2, NumericHelper.GCD(676733084, 883191742));
            Assert.AreEqual(1, NumericHelper.GCD(461854585, 503034297));
            Assert.AreEqual(2, NumericHelper.GCD(423541676, 978926918));
            Assert.AreEqual(17, NumericHelper.GCD(883027226, 914620757));
            Assert.AreEqual(2, NumericHelper.GCD(843545372, 614288570));
            Assert.AreEqual(2, NumericHelper.GCD(339473804, 955338346));
            Assert.AreEqual(1, NumericHelper.GCD(266469934, 670394525));
            Assert.AreEqual(2, NumericHelper.GCD(412756198, 678862818));
        }

        [Test]
        public static void GCDMinValue()
        {
            Assert.AreEqual(4, NumericHelper.GCD(long.MinValue, 180));
        }

        [Test]
        public static void GCDZero()
        {
            Assert.AreEqual(180, NumericHelper.GCD(0, 180));
            Assert.AreEqual(180, NumericHelper.GCD(0, -180));

            Assert.AreEqual(13456489866, NumericHelper.GCD(0, 13456489866));
            Assert.AreEqual(13456489866, NumericHelper.GCD(0, -13456489866));
        }
    }
}