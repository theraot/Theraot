using NUnit.Framework;
using System;
using System.Linq;
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

        [Test]
        public void Binary()
        {
            var input = new int[] { 22141034, 146009798, 106447544, 66083576, 28048294, 3848650, 119601527, 182384611, 160860217, 52726162 };
            var output = new string[]
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
            for (int index = 0; index < input.Length; index++)
            {
                Assert.AreEqual(output[index], NumericHelper.ToStringBinary(input[index]));
                var characters = output[index].ToCharArray();
                var bitsBinary = NumericHelper.BitsBinary(input[index]).ToArray();
                var bits = NumericHelper.Bits(input[index]).ToArray();
                var bitsLog2 = NumericHelper.BitsLog2(input[index]).ToArray();
                /*var reverse = NumericHelper.BinaryReverse(input[index]);
                var rbitsBinary = NumericHelper.BitsBinary(reverse).ToArray();*/
                var bitIndex = 0;
                for (int subindex = 0; subindex < 32; subindex++)
                {
                    Assert.AreEqual(characters[subindex].ToString(), bitsBinary[subindex].ToString());
                    //Assert.AreEqual(characters[31 - subindex].ToString(), bitsBinary[subindex].ToString());
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
    }
}