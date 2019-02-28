using NUnit.Framework;
using System;
using Theraot.Core;

namespace Tests.Theraot.Core
{
    [TestFixture]
    internal partial class NumericHelperTest
    {
#if FAT

        [Test]
        public void Decrements()
        {
            Assert.AreEqual(int.MaxValue, int.MinValue.UncheckedDecrement());
            Assert.AreEqual(short.MaxValue, short.MinValue.UncheckedDecrement());
            Assert.AreEqual(long.MaxValue, long.MinValue.UncheckedDecrement());

            Assert.Throws<OverflowException>(() => int.MinValue.CheckedDecrement());
            Assert.Throws<OverflowException>(() => short.MinValue.CheckedDecrement());
            Assert.Throws<OverflowException>(() => long.MinValue.CheckedDecrement());
        }

        [Test]
        public void Increments()
        {
            Assert.AreEqual(int.MinValue, int.MaxValue.UncheckedIncrement());
            Assert.AreEqual(short.MinValue, short.MaxValue.UncheckedIncrement());
            Assert.AreEqual(long.MinValue, long.MaxValue.UncheckedIncrement());

            Assert.Throws<OverflowException>(() => int.MaxValue.CheckedIncrement());
            Assert.Throws<OverflowException>(() => short.MaxValue.CheckedIncrement());
            Assert.Throws<OverflowException>(() => long.MaxValue.CheckedIncrement());
        }

#endif

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

#if FAT
    internal partial class NumericHelperTest
    {
        [Test]
        public void BinaryByte()
        {
            var input = new byte[] { 157, 155, 92, 129, 166, 100, 17, 168, 158, 128 };
            var output = new[]
            {
                "10011101",
                "10011011",
                "01011100",
                "10000001",
                "10100110",
                "01100100",
                "00010001",
                "10101000",
                "10011110",
                "10000000"
            };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], input[index].ToStringBinary());
                var characters = output[index].ToCharArray();
                var bitsBinary = input[index].BitsBinary().ToArray();
                var bits = input[index].Bits().ToArray();
                var bitsLog2 = input[index].BitsLog2().ToArray();
                var reverse = NumericHelper.BinaryReverse(input[index]);
                var rbitsBinary = reverse.BitsBinary().ToArray();
                var bitIndex = 0;
                for (var subindex = 0; subindex < 8; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    Assert.AreEqual(characters[7 - subindex].ToString(), rbitsBinary[subindex].ToString());
                    if (bitIndex < bits.Length)
                    {
                        var log2 = NumericHelper.Log2(bits[bitIndex]);
                        Assert.AreEqual(log2, bitsLog2[bitIndex]);
                        if (log2 == 7 - subindex)
                        {
                            if (characters[subindex] == '0')
                            {
                                Assert.Fail();
                            }
                            bitIndex++;
                        }
                    }
                }
            }
        }

        [Test]
        public void BinaryInt()
        {
            var input = new[] { 22141034, -46009798, 106447544, -6083576, 28048294, -848650, 119601527, -82384611, 160860217, -2726162 };
            var output = new[]
            {
                "00000001010100011101100001101010",
                "11111101010000011111001000111010",
                "00000110010110000100001010111000",
                "11111111101000110010110000001000",
                "00000001101010111111101110100110",
                "11111111111100110000110011110110",
                "00000111001000001111100101110111",
                "11111011000101101110100100011101",
                "00001001100101101000100000111001",
                "11111111110101100110011011101110"
            };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], input[index].ToStringBinary());
                var characters = output[index].ToCharArray();
                var bitsBinary = input[index].BitsBinary().ToArray();
                var bits = input[index].Bits().ToArray();
                var bitsLog2 = input[index].BitsLog2().ToArray();
                var reverse = input[index].BinaryReverse();
                var rbitsBinary = reverse.BitsBinary().ToArray();
                var bitIndex = 0;
                for (var subindex = 0; subindex < 32; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    Assert.AreEqual(characters[31 - subindex].ToString(), rbitsBinary[subindex].ToString());
                    if (bitIndex < bits.Length)
                    {
                        var log2 = NumericHelper.Log2(unchecked((uint)bits[bitIndex]));
                        Assert.AreEqual(log2, bitsLog2[bitIndex]);
                        if (log2 == 31 - subindex)
                        {
                            if (characters[subindex] == '0')
                            {
                                Assert.Fail();
                            }
                            bitIndex++;
                        }
                    }
                }
            }
        }

        [Test]
        public void BinaryLong()
        {
            var input = new[]
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
            var output = new[]
            {
                "0010000000101101101101011111111000100110010110100010110000000101",
                "0101000101100000111111100110001010010100111001011100100010010101",
                "0001101001100011010000000000101101000101110110111000000001110101",
                "0010000011100110001100110010010000001011100000100110000100100011",
                "0000110100010101010101100111001011111010000110111011111010111110",
                "1010001110011001011010111111111101000101011111010110011011010010",
                "1001111010111101010100100100101001010000110011100110011110010001",
                "1100011101011001001111111101111001010001101010100101000110111010",
                "1101010111000010010010010011001111000111010010011100101101000011",
                "1010011100000110010011011111000100011111111011010001011001010110"
            };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], input[index].ToStringBinary());
                var characters = output[index].ToCharArray();
                var bitsBinary = input[index].BitsBinary().ToArray();
                var bits = input[index].Bits().ToArray();
                var bitsLog2 = input[index].BitsLog2().ToArray();
                var reverse = input[index].BinaryReverse();
                var rbitsBinary = reverse.BitsBinary().ToArray();
                var bitIndex = 0;
                for (var subindex = 0; subindex < 64; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    Assert.AreEqual(characters[63 - subindex].ToString(), rbitsBinary[subindex].ToString());
                    if (bitIndex < bits.Length)
                    {
                        var log2 = NumericHelper.Log2(unchecked((ulong)bits[bitIndex]));
                        Assert.AreEqual(log2, bitsLog2[bitIndex]);
                        if (log2 == 63 - subindex)
                        {
                            if (characters[subindex] == '0')
                            {
                                Assert.Fail();
                            }
                            bitIndex++;
                        }
                    }
                }
            }
        }

        [Test]
        public void BinarySByte()
        {
            var input = new sbyte[] { -57, -55, 92, -29, -66, 0, 17, -68, -58, -28 };
            var output = new[]
            {
                "11000111",
                "11001001",
                "01011100",
                "11100011",
                "10111110",
                "00000000",
                "00010001",
                "10111100",
                "11000110",
                "11100100"
            };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], input[index].ToStringBinary());
                var characters = output[index].ToCharArray();
                var bitsBinary = input[index].BitsBinary().ToArray();
                var bits = input[index].Bits().ToArray();
                var bitsLog2 = input[index].BitsLog2().ToArray();
                var reverse = NumericHelper.BinaryReverse(input[index]);
                var rbitsBinary = reverse.BitsBinary().ToArray();
                var bitIndex = 0;
                for (var subindex = 0; subindex < 8; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    Assert.AreEqual(characters[7 - subindex].ToString(), rbitsBinary[subindex].ToString());
                    if (bitIndex < bits.Length)
                    {
                        var log2 = NumericHelper.Log2(unchecked((byte)bits[bitIndex]));
                        Assert.AreEqual(log2, bitsLog2[bitIndex]);
                        if (log2 == 7 - subindex)
                        {
                            if (characters[subindex] == '0')
                            {
                                Assert.Fail();
                            }
                            bitIndex++;
                        }
                    }
                }
            }
        }

        [Test]
        public void BinaryShort()
        {
            var input = new short[] { 22141, -4600, 10644, -6083, 28048, -848, 11960, -8238, 16086, -2726 };
            var output = new[]
            {
                "0101011001111101",
                "1110111000001000",
                "0010100110010100",
                "1110100000111101",
                "0110110110010000",
                "1111110010110000",
                "0010111010111000",
                "1101111111010010",
                "0011111011010110",
                "1111010101011010"
            };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], input[index].ToStringBinary());
                var characters = output[index].ToCharArray();
                var bitsBinary = input[index].BitsBinary().ToArray();
                var bits = input[index].Bits().ToArray();
                var bitsLog2 = input[index].BitsLog2().ToArray();
                var reverse = NumericHelper.BinaryReverse(input[index]);
                var rbitsBinary = reverse.BitsBinary().ToArray();
                var bitIndex = 0;
                for (var subindex = 0; subindex < 16; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    Assert.AreEqual(characters[15 - subindex].ToString(), rbitsBinary[subindex].ToString());
                    if (bitIndex < bits.Length)
                    {
                        var log2 = NumericHelper.Log2(unchecked((ushort)bits[bitIndex]));
                        Assert.AreEqual(log2, bitsLog2[bitIndex]);
                        if (log2 == 15 - subindex)
                        {
                            if (characters[subindex] == '0')
                            {
                                Assert.Fail();
                            }
                            bitIndex++;
                        }
                    }
                }
            }
        }

        [Test]
        public void BinaryUInt()
        {
            var input = new uint[] { 22141034, 146009798, 106447544, 66083576, 28048294, 3848650, 119601527, 182384611, 160860217, 52726162 };
            var output = new[]
            {
                "00000001010100011101100001101010",
                "00001000101100111110111011000110",
                "00000110010110000100001010111000",
                "00000011111100000101101011111000",
                "00000001101010111111101110100110",
                "00000000001110101011100111001010",
                "00000111001000001111100101110111",
                "00001010110111101111011111100011",
                "00001001100101101000100000111001",
                "00000011001001001000100110010010"
            };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], input[index].ToStringBinary());
                var characters = output[index].ToCharArray();
                var bitsBinary = input[index].BitsBinary().ToArray();
                var bits = input[index].Bits().ToArray();
                var bitsLog2 = input[index].BitsLog2().ToArray();
                var reverse = input[index].BinaryReverse();
                var rbitsBinary = reverse.BitsBinary().ToArray();
                var bitIndex = 0;
                for (var subindex = 0; subindex < 32; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    Assert.AreEqual(characters[31 - subindex].ToString(), rbitsBinary[subindex].ToString());
                    if (bitIndex < bits.Length)
                    {
                        var log2 = NumericHelper.Log2(bits[bitIndex]);
                        Assert.AreEqual(log2, bitsLog2[bitIndex]);
                        if (log2 == 31 - subindex)
                        {
                            if (characters[subindex] == '0')
                            {
                                Assert.Fail();
                            }
                            bitIndex++;
                        }
                    }
                }
            }
        }

        [Test]
        public void BinaryULong()
        {
            var input = new ulong[]
            {
                2318709486335437829,
                5863966414194722965,
                1901433884841312373,
                2370638483762077987,
                942754748826369726,
                6658171829963888942,
                7008354965981861999,
                4082161363149237830,
                3043789911413372093,
                6411351321497233834
            };
            var output = new[]
            {
                "0010000000101101101101011111111000100110010110100010110000000101",
                "0101000101100000111111100110001010010100111001011100100010010101",
                "0001101001100011010000000000101101000101110110111000000001110101",
                "0010000011100110001100110010010000001011100000100110000100100011",
                "0000110100010101010101100111001011111010000110111011111010111110",
                "0101110001100110100101000000000010111010100000101001100100101110",
                "0110000101000010101011011011010110101111001100011001100001101111",
                "0011100010100110110000000010000110101110010101011010111001000110",
                "0010101000111101101101101100110000111000101101100011010010111101",
                "0101100011111001101100100000111011100000000100101110100110101010"
            };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], input[index].ToStringBinary());
                var characters = output[index].ToCharArray();
                var bitsBinary = input[index].BitsBinary().ToArray();
                var bits = input[index].Bits().ToArray();
                var bitsLog2 = input[index].BitsLog2().ToArray();
                var reverse = input[index].BinaryReverse();
                var rbitsBinary = reverse.BitsBinary().ToArray();
                var bitIndex = 0;
                for (var subindex = 0; subindex < 64; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    Assert.AreEqual(characters[63 - subindex].ToString(), rbitsBinary[subindex].ToString());
                    if (bitIndex < bits.Length)
                    {
                        var log2 = NumericHelper.Log2(bits[bitIndex]);
                        Assert.AreEqual(log2, bitsLog2[bitIndex]);
                        if (log2 == 63 - subindex)
                        {
                            if (characters[subindex] == '0')
                            {
                                Assert.Fail();
                            }
                            bitIndex++;
                        }
                    }
                }
            }
        }

        [Test]
        public void BinaryUShort()
        {
            var input = new ushort[] { 22141, 14600, 10644, 16083, 28048, 1848, 11960, 18238, 16086, 12726 };
            var output = new[]
            {
                "0101011001111101",
                "0011100100001000",
                "0010100110010100",
                "0011111011010011",
                "0110110110010000",
                "0000011100111000",
                "0010111010111000",
                "0100011100111110",
                "0011111011010110",
                "0011000110110110"
            };
            for (var index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], input[index].ToStringBinary());
                var characters = output[index].ToCharArray();
                var bitsBinary = input[index].BitsBinary().ToArray();
                var bits = input[index].Bits().ToArray();
                var bitsLog2 = input[index].BitsLog2().ToArray();
                var reverse = NumericHelper.BinaryReverse(input[index]);
                var rbitsBinary = reverse.BitsBinary().ToArray();
                var bitIndex = 0;
                for (var subindex = 0; subindex < 16; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    Assert.AreEqual(characters[15 - subindex].ToString(), rbitsBinary[subindex].ToString());
                    if (bitIndex < bits.Length)
                    {
                        var log2 = NumericHelper.Log2(bits[bitIndex]);
                        Assert.AreEqual(log2, bitsLog2[bitIndex]);
                        if (log2 == 15 - subindex)
                        {
                            if (characters[subindex] == '0')
                            {
                                Assert.Fail();
                            }
                            bitIndex++;
                        }
                    }
                }
            }
        }
    }
#endif

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
        public static void GCDZero()
        {
            Assert.AreEqual(180, NumericHelper.GCD(0, 180));
            Assert.AreEqual(180, NumericHelper.GCD(0, -180));

            Assert.AreEqual(13456489866, NumericHelper.GCD(0, 13456489866));
            Assert.AreEqual(13456489866, NumericHelper.GCD(0, -13456489866));
        }

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
        public static void GCDMaxValue()
        {
            Assert.AreEqual(15, NumericHelper.GCD(ulong.MaxValue, 180));
        }

        [Test]
        public static void GCDMinValue()
        {
            Assert.AreEqual(4, NumericHelper.GCD(long.MinValue, 180));
        }
    }
}