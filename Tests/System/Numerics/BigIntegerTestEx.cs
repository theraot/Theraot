#if NET35

using NUnit.Framework;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace MonoTests.System.Numerics
{
    [TestFixture]
    public class BigIntegerTestEx
    {
        [Test]
        public void FormatC()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("C", CultureInfo.InvariantCulture), "¤123,456,789.00");
            Assert.AreEqual(first.ToString("c", CultureInfo.InvariantCulture), "¤123,456,789.00");
            Assert.AreEqual(first.ToString("C3", CultureInfo.InvariantCulture), "¤123,456,789.000");
            Assert.AreEqual(first.ToString("c3", CultureInfo.InvariantCulture), "¤123,456,789.000");

            Assert.AreEqual(first.ToString("C3", CultureInfo.GetCultureInfo("en-US")), "$123,456,789.000");
            Assert.AreEqual(first.ToString("c3", CultureInfo.GetCultureInfo("en-US")), "$123,456,789.000");

            Assert.AreEqual(first.ToString("C3", CultureInfo.GetCultureInfo("fr-FR")), "123\u00A0456\u00A0789,000 €");
            Assert.AreEqual(first.ToString("c3", CultureInfo.GetCultureInfo("fr-FR")), "123\u00A0456\u00A0789,000 €");

            Assert.AreEqual(first.ToString("C3", CultureInfo.GetCultureInfo("ja-JP")), "¥123,456,789.000");
            Assert.AreEqual(first.ToString("c3", CultureInfo.GetCultureInfo("ja-JP")), "¥123,456,789.000");

            var second = BigInteger.Parse("100000090000000010", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("C", CultureInfo.InvariantCulture), "¤100,000,090,000,000,010.00");
            Assert.AreEqual(second.ToString("c", CultureInfo.InvariantCulture), "¤100,000,090,000,000,010.00");
            Assert.AreEqual(second.ToString("C3", CultureInfo.InvariantCulture), "¤100,000,090,000,000,010.000");
            Assert.AreEqual(second.ToString("c3", CultureInfo.InvariantCulture), "¤100,000,090,000,000,010.000");

            Assert.AreEqual(second.ToString("C3", CultureInfo.GetCultureInfo("en-US")), "$100,000,090,000,000,010.000");
            Assert.AreEqual(second.ToString("c3", CultureInfo.GetCultureInfo("en-US")), "$100,000,090,000,000,010.000");

            Assert.AreEqual(second.ToString("C3", CultureInfo.GetCultureInfo("fr-FR")), "100\u00A0000\u00A0090\u00A0000\u00A0000\u00A0010,000 €");
            Assert.AreEqual(second.ToString("c3", CultureInfo.GetCultureInfo("fr-FR")), "100\u00A0000\u00A0090\u00A0000\u00A0000\u00A0010,000 €");

            Assert.AreEqual(second.ToString("C3", CultureInfo.GetCultureInfo("ja-JP")), "¥100,000,090,000,000,010.000");
            Assert.AreEqual(second.ToString("c3", CultureInfo.GetCultureInfo("ja-JP")), "¥100,000,090,000,000,010.000");
        }

        [Test]
        public void FormatD()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("D", CultureInfo.InvariantCulture), "123456789");
            Assert.AreEqual(first.ToString("d", CultureInfo.InvariantCulture), "123456789");
            Assert.AreEqual(first.ToString("D20", CultureInfo.InvariantCulture), "00000000000123456789");
            Assert.AreEqual(first.ToString("d20", CultureInfo.InvariantCulture), "00000000000123456789");

            var second = BigInteger.Parse("100000090000000010", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("D", CultureInfo.InvariantCulture), "100000090000000010");
            Assert.AreEqual(second.ToString("d", CultureInfo.InvariantCulture), "100000090000000010");
            Assert.AreEqual(second.ToString("D20", CultureInfo.InvariantCulture), "00100000090000000010");
            Assert.AreEqual(second.ToString("d20", CultureInfo.InvariantCulture), "00100000090000000010");
        }

        [Test]
        public void FormatE1()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("E", CultureInfo.InvariantCulture), "1.234568E+008");
            Assert.AreEqual(first.ToString("e", CultureInfo.InvariantCulture), "1.234568e+008");

            Debug.Print("BigInteger Format First ok");

            var second = BigInteger.Parse("10000009000000001", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("E", CultureInfo.InvariantCulture), "1.000001E+016");
            Assert.AreEqual(second.ToString("e", CultureInfo.InvariantCulture), "1.000001e+016");

            Debug.Print("BigInteger Format Second ok");

            var third = BigInteger.Parse("10000005000000001", CultureInfo.InvariantCulture);
            Assert.AreEqual(third.ToString("E", CultureInfo.InvariantCulture), "1.000001E+016");
            Assert.AreEqual(third.ToString("e", CultureInfo.InvariantCulture), "1.000001e+016");

            Debug.Print("BigInteger Format Third ok");

            var fourth = BigInteger.Parse("10000004000000001", CultureInfo.InvariantCulture);
            Assert.AreEqual(fourth.ToString("E", CultureInfo.InvariantCulture), "1.000000E+016");
            Assert.AreEqual(fourth.ToString("e", CultureInfo.InvariantCulture), "1.000000e+016");

            Debug.Print("BigInteger Format Fourth ok");

            var fifth = BigInteger.Parse("10000095000000001", CultureInfo.InvariantCulture);
            Assert.AreEqual(fifth.ToString("E", CultureInfo.InvariantCulture), "1.000010E+016");
            Assert.AreEqual(fifth.ToString("e", CultureInfo.InvariantCulture), "1.000010e+016");

            Debug.Print("BigInteger Format Fifth ok");
        }

        [Test]
        public void FormatE2()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("E2", CultureInfo.InvariantCulture), "1.23E+008");
            Assert.AreEqual(first.ToString("e2", CultureInfo.InvariantCulture), "1.23e+008");

            Debug.Print("BigInteger Format First ok");

            var second = BigInteger.Parse("10000009000000001", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("E20", CultureInfo.InvariantCulture), "1.00000090000000010000E+016");
            Assert.AreEqual(second.ToString("e20", CultureInfo.InvariantCulture), "1.00000090000000010000e+016");

            Debug.Print("BigInteger Format Second ok");

            var third = BigInteger.Parse("10000005000000001", CultureInfo.InvariantCulture);
            Assert.AreEqual(third.ToString("E17", CultureInfo.InvariantCulture), "1.00000050000000010E+016");
            Assert.AreEqual(third.ToString("e17", CultureInfo.InvariantCulture), "1.00000050000000010e+016");

            Debug.Print("BigInteger Format Third ok");

            var fourth = BigInteger.Parse("10000004000000001", CultureInfo.InvariantCulture);
            Assert.AreEqual(fourth.ToString("E16", CultureInfo.InvariantCulture), "1.0000004000000001E+016");
            Assert.AreEqual(fourth.ToString("e16", CultureInfo.InvariantCulture), "1.0000004000000001e+016");

            Debug.Print("BigInteger Format Fourth ok");

            var fifth = BigInteger.Parse("10000095000000001", CultureInfo.InvariantCulture);
            Assert.AreEqual(fifth.ToString("E15", CultureInfo.InvariantCulture), "1.000009500000000E+016");
            Assert.AreEqual(fifth.ToString("e15", CultureInfo.InvariantCulture), "1.000009500000000e+016");

            Debug.Print("BigInteger Format Fifth ok");
        }

        [Test]
        public void FormatE3()
        {
            var first = new BigInteger(123456789000000000000000000d);
            Assert.AreEqual(first.ToString("E", CultureInfo.InvariantCulture), "1.234568E+026");
            Assert.AreEqual(first.ToString("e", CultureInfo.InvariantCulture), "1.234568e+026");

            Debug.Print("BigInteger Format First ok");

            var second = BigInteger.Parse("10000009000000001000000000000000000", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("E", CultureInfo.InvariantCulture), "1.000001E+034");
            Assert.AreEqual(second.ToString("e", CultureInfo.InvariantCulture), "1.000001e+034");

            Debug.Print("BigInteger Format Second ok");

            var third = BigInteger.Parse("1000000900000000100000000000000000000000", CultureInfo.InvariantCulture);
            Assert.AreEqual(third.ToString("E", CultureInfo.InvariantCulture), "1.000001E+039");
            Assert.AreEqual(third.ToString("e", CultureInfo.InvariantCulture), "1.000001e+039");

            Debug.Print("BigInteger Format Third ok");

            var fourth = BigInteger.Parse("1000000900000000100000000000000000085161812", CultureInfo.InvariantCulture);
            Assert.AreEqual(fourth.ToString("E", CultureInfo.InvariantCulture), "1.000001E+042");
            Assert.AreEqual(fourth.ToString("e", CultureInfo.InvariantCulture), "1.000001e+042");

            Debug.Print("BigInteger Format Fourth ok");

            var fifth = BigInteger.Parse("54161605161103534310841634874874621308503540", CultureInfo.InvariantCulture);
            Assert.AreEqual(fifth.ToString("E", CultureInfo.InvariantCulture), "5.416161E+043");
            Assert.AreEqual(fifth.ToString("e", CultureInfo.InvariantCulture), "5.416161e+043");

            Debug.Print("BigInteger Format Fifth ok");
        }

        [Test]
        public void FormatF()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("F", CultureInfo.InvariantCulture), "123456789.00");
            Assert.AreEqual(first.ToString("f", CultureInfo.InvariantCulture), "123456789.00");
            Assert.AreEqual(first.ToString("F20", CultureInfo.InvariantCulture), "123456789.00000000000000000000");
            Assert.AreEqual(first.ToString("f20", CultureInfo.InvariantCulture), "123456789.00000000000000000000");

            var second = BigInteger.Parse("100000090000000010", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("F", CultureInfo.InvariantCulture), "100000090000000010.00");
            Assert.AreEqual(second.ToString("f", CultureInfo.InvariantCulture), "100000090000000010.00");
            Assert.AreEqual(second.ToString("F20", CultureInfo.InvariantCulture), "100000090000000010.00000000000000000000");
            Assert.AreEqual(second.ToString("f20", CultureInfo.InvariantCulture), "100000090000000010.00000000000000000000");
        }

        [Test]
        public void FormatG()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("G", CultureInfo.InvariantCulture), "123456789");
            Assert.AreEqual(first.ToString("g", CultureInfo.InvariantCulture), "123456789");
            Assert.AreEqual(first.ToString("G20", CultureInfo.InvariantCulture), "00000000000123456789");
            Assert.AreEqual(first.ToString("g20", CultureInfo.InvariantCulture), "00000000000123456789");

            var second = BigInteger.Parse("100000090000000010", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("G", CultureInfo.InvariantCulture), "100000090000000010");
            Assert.AreEqual(second.ToString("g", CultureInfo.InvariantCulture), "100000090000000010");
            Assert.AreEqual(second.ToString("G20", CultureInfo.InvariantCulture), "00100000090000000010");
            Assert.AreEqual(second.ToString("g20", CultureInfo.InvariantCulture), "00100000090000000010");
        }

        [Test]
        public void FormatN()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("N", CultureInfo.InvariantCulture), "123,456,789.00");
            Assert.AreEqual(first.ToString("n", CultureInfo.InvariantCulture), "123,456,789.00");
            Assert.AreEqual(first.ToString("N20", CultureInfo.InvariantCulture), "123,456,789.00000000000000000000");
            Assert.AreEqual(first.ToString("n20", CultureInfo.InvariantCulture), "123,456,789.00000000000000000000");

            var second = BigInteger.Parse("100000090000000010", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("N", CultureInfo.InvariantCulture), "100,000,090,000,000,010.00");
            Assert.AreEqual(second.ToString("n", CultureInfo.InvariantCulture), "100,000,090,000,000,010.00");
            Assert.AreEqual(second.ToString("N20", CultureInfo.InvariantCulture), "100,000,090,000,000,010.00000000000000000000");
            Assert.AreEqual(second.ToString("n20", CultureInfo.InvariantCulture), "100,000,090,000,000,010.00000000000000000000");
        }

        [Test]
        public void FormatP()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("P", CultureInfo.InvariantCulture), "12,345,678,900.00 %");
            Assert.AreEqual(first.ToString("p", CultureInfo.InvariantCulture), "12,345,678,900.00 %");
            Assert.AreEqual(first.ToString("P20", CultureInfo.InvariantCulture), "12,345,678,900.00000000000000000000 %");
            Assert.AreEqual(first.ToString("p20", CultureInfo.InvariantCulture), "12,345,678,900.00000000000000000000 %");

            var second = BigInteger.Parse("100000090000000010", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("P", CultureInfo.InvariantCulture), "10,000,009,000,000,001,000.00 %");
            Assert.AreEqual(second.ToString("p", CultureInfo.InvariantCulture), "10,000,009,000,000,001,000.00 %");
            Assert.AreEqual(second.ToString("P20", CultureInfo.InvariantCulture), "10,000,009,000,000,001,000.00000000000000000000 %");
            Assert.AreEqual(second.ToString("p20", CultureInfo.InvariantCulture), "10,000,009,000,000,001,000.00000000000000000000 %");
        }

        [Test]
        public void FormatR()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("R", CultureInfo.InvariantCulture), "123456789");
            Assert.AreEqual(first.ToString("r", CultureInfo.InvariantCulture), "123456789");
            Assert.AreEqual(first.ToString("R20", CultureInfo.InvariantCulture), "00000000000123456789");
            Assert.AreEqual(first.ToString("r20", CultureInfo.InvariantCulture), "00000000000123456789");

            var second = BigInteger.Parse("100000090000000010", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("R", CultureInfo.InvariantCulture), "100000090000000010");
            Assert.AreEqual(second.ToString("r", CultureInfo.InvariantCulture), "100000090000000010");
            Assert.AreEqual(second.ToString("R20", CultureInfo.InvariantCulture), "00100000090000000010");
            Assert.AreEqual(second.ToString("r20", CultureInfo.InvariantCulture), "00100000090000000010");
        }

        [Test]
        public void FormatX()
        {
            var first = new BigInteger(123456789);
            Assert.AreEqual(first.ToString("X", CultureInfo.InvariantCulture), "75BCD15");
            Assert.AreEqual(first.ToString("x", CultureInfo.InvariantCulture), "75bcd15");
            Assert.AreEqual(first.ToString("X20", CultureInfo.InvariantCulture), "000000000000075BCD15");
            Assert.AreEqual(first.ToString("x20", CultureInfo.InvariantCulture), "000000000000075bcd15");

            var second = BigInteger.Parse("100000090000000010", CultureInfo.InvariantCulture);
            Assert.AreEqual(second.ToString("X", CultureInfo.InvariantCulture), "163458D51F5040A");
            Assert.AreEqual(second.ToString("x", CultureInfo.InvariantCulture), "163458d51f5040a");
            Assert.AreEqual(second.ToString("X20", CultureInfo.InvariantCulture), "00000163458D51F5040A");
            Assert.AreEqual(second.ToString("x20", CultureInfo.InvariantCulture), "00000163458d51f5040a");
        }

        [Test]
        public void FromByteArrayTest()
        {
            long[] values =
            {
                0,
                long.MinValue,
                long.MaxValue,
                -1,
                1L + int.MaxValue,
                -1L + int.MinValue,
                0x1234,
                0xFFFFFFFFL,
                0x1FFFFFFFFL,
                -0xFFFFFFFFL,
                -0x1FFFFFFFFL,
                0x100000000L,
                -0x100000000L,
                0x100000001L,
                -0x100000001L,
                4294967295L,
                -4294967295L,
                4294967296L,
                -4294967296L
            };
            byte[][] arrays =
            {
                new byte[]
                {
                    0
                },
                new byte[]
                {
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    128
                },
                new byte[]
                {
                    255,
                    255,
                    255,
                    255,
                    255,
                    255,
                    255,
                    127
                },
                new byte[]
                {
                    255
                },
                new byte[]
                {
                    0,
                    0,
                    0,
                    128,
                    0
                },
                new byte[]
                {
                    255,
                    255,
                    255,
                    127,
                    255
                },
                new byte[]
                {
                    52,
                    18
                },
                new byte[]
                {
                    255,
                    255,
                    255,
                    255,
                    0
                },
                new byte[]
                {
                    255,
                    255,
                    255,
                    255,
                    1
                },
                new byte[]
                {
                    1,
                    0,
                    0,
                    0,
                    255
                },
                new byte[]
                {
                    1,
                    0,
                    0,
                    0,
                    254
                },
                new byte[]
                {
                    0,
                    0,
                    0,
                    0,
                    1
                },
                new byte[]
                {
                    0,
                    0,
                    0,
                    0,
                    255
                },
                new byte[]
                {
                    1,
                    0,
                    0,
                    0,
                    1
                },
                new byte[]
                {
                    255,
                    255,
                    255,
                    255,
                    254
                },
                new byte[]
                {
                    255,
                    255,
                    255,
                    255,
                    0
                },
                new byte[]
                {
                    1,
                    0,
                    0,
                    0,
                    255
                },
                new byte[]
                {
                    0,
                    0,
                    0,
                    0,
                    1
                },
                new byte[]
                {
                    0,
                    0,
                    0,
                    0,
                    255
                }
            };
            for (int index = 0; index < values.Length; index++)
            {
                Check(values[index], arrays[index]);
            }
        }

        [Test]
        public void ToByteArrayTest()
        {
            var a = new BigInteger(0);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            0
                        }
                    )
                );
            a = new BigInteger(long.MinValue);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            128
                        }
                    )
                );
            a = new BigInteger(long.MaxValue);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            255,
                            255,
                            255,
                            255,
                            255,
                            255,
                            255,
                            127
                        }
                    )
                );
            a = new BigInteger(-1);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            255
                        }
                    )
                );
            a = new BigInteger(1L + int.MaxValue);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            0,
                            0,
                            0,
                            128,
                            0
                        }
                    )
                );
            a = new BigInteger(-1L + int.MinValue);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            255,
                            255,
                            255,
                            127,
                            255
                        }
                    )
                );
            a = new BigInteger(0x1234);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            52,
                            18
                        }
                    )
                );
            a = new BigInteger(0xFFFFFFFFL);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            255,
                            255,
                            255,
                            255,
                            0
                        }
                    )
                );
            a = new BigInteger(0x1FFFFFFFFL);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            255,
                            255,
                            255,
                            255,
                            1
                        }
                    )
                );
            a = new BigInteger(-0xFFFFFFFFL);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            1,
                            0,
                            0,
                            0,
                            255
                        }
                    )
                );
            a = new BigInteger(-0x1FFFFFFFFL);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            1,
                            0,
                            0,
                            0,
                            254
                        }
                    )
                );
            a = new BigInteger(0x100000000L);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            0,
                            0,
                            0,
                            0,
                            1
                        }
                    )
                );
            a = new BigInteger(-0x100000000L);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            0,
                            0,
                            0,
                            0,
                            255
                        }
                    )
                );
            a = new BigInteger(0x100000001L);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            1,
                            0,
                            0,
                            0,
                            1
                        }
                    )
                );
            a = new BigInteger(-0x100000001L);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            255,
                            255,
                            255,
                            255,
                            254
                        }
                    )
                );
            a = new BigInteger(4294967295L);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            255,
                            255,
                            255,
                            255,
                            0
                        }
                    )
                );
            a = new BigInteger(-4294967295L);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            1,
                            0,
                            0,
                            0,
                            255
                        }
                    )
                );
            a = new BigInteger(4294967296L);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            0,
                            0,
                            0,
                            0,
                            1
                        }
                    )
                );
            a = new BigInteger(-4294967296L);
            Assert.IsTrue
                (
                    a.ToByteArray().SequenceEqual
                    (
                        new byte[]
                        {
                            0,
                            0,
                            0,
                            0,
                            255
                        }
                    )
                );
        }

        private static void Check(long value, byte[] array)
        {
            // Created by value
            Check(array, new BigInteger(value), "Created by Value: " + value);
            Check(array, new BigInteger(array), "Created by Array for value: " + value);
        }

        private static void Check(byte[] array, BigInteger a, string msg)
        {
            var result = a.ToByteArray();
            if (!result.SequenceEqual(array))
            {
                Debug.Print(" - Failed - ");
                Debug.Print(msg);
                Debug.Print("Value = " + a);
                Debug.Print("ToByteArray returned:");
                foreach (var item in result)
                {
                    Debug.Print(item.ToString());
                }
                Debug.Print("Expected:");
                foreach (var item in array)
                {
                    Debug.Print(item.ToString());
                }
                Assert.Fail();
            }
        }
    }
}

#endif