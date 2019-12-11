#pragma warning disable CA1305 // Specify IFormatProvider

using System;
using NUnit.Framework;

namespace Tests.SystemTests
{
    [TestFixture]
    public static class FormattableStringTests
    {
        [Test]
        public static void ReadAsyncReads()
        {
            TestMe($"{123.34}");
        }

        private static void TestMe(FormattableString s)
        {
            Console.WriteLine(s.ToString());
        }
    }
}