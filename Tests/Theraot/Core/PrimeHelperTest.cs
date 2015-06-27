using NUnit.Framework;
using System;
using System.Linq;
using Theraot.Core;

namespace Tests.Theraot.Core
{
    [TestFixture]
    internal partial class PrimeHelperTest
    {
        [Test]
        public void TestIsPrime()
        {
            var input = new int[]
            {
                18, 13, 68, 99, 93, 34, 11, 47, 64, 10,
                809, 3420, 9391, 8999, 5154, 8934, 5416, 5669, 2755, 3159,
                285080, 727881, 846232, 665370, 153194, 157833, 585896, 914054, 505284, 333258,
                49086597, 78119724, 76928802, 23260170, 1955743, 39360664, 10885879, 30169506, 65889970, 95425647,
                179424551, 179424571, 179424577, 179424601, 179424611, 179424617, 179424629, 179424667, 179424671, 179424673,
                1683899352, 883641873, 114883641, 1000000007, 1000000009, 1000000021, 1000000033, 1000000087, 1000000093, 2147483629
            };
            var output = new int[]
            {
                0, 1, 0, 0, 0, 0, 1, 1, 0, 0,
                1, 0, 1, 1, 0, 0, 0, 1, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                0, 0, 0, 1, 1, 1, 1, 1, 1, 1
            };
            Assert.AreEqual(input.Length, output.Length, "setup");
            for (int index = 0; index < output.Length; index++)
            {
                Assert.AreEqual(output[index] == 1, PrimeHelper.IsPrime(input[index]));
            }
        }

        [Test]
        public void TestNextPrimeBounds()
        {
            // 2147483629 is tha last prime below int.MaxValue
            Assert.AreEqual(2, PrimeHelper.NextPrime(-1));
            Assert.Throws<OverflowException>(() => PrimeHelper.NextPrime(2147483629));
        }
    }
}