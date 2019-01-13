#if FAT

using NUnit.Framework;
using System;
using System.IO;
using Theraot.Core;

namespace Test
{
    [TestFixture]
    internal class RamStreamTests
    {
        [Test]
        public void DisposeTest()
        {
            var stream = CreateStream();
            stream.Dispose();
            Assert.IsTrue(stream.IsDisposed());
            var buffer = new byte[] { };
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                Assert.Fail();
            }
            catch (ObjectDisposedException exception)
            {
                Theraot.No.Op(exception);
            }
            try
            {
                stream.Read(buffer, 0, 0);
                Assert.Fail();
            }
            catch (ObjectDisposedException exception)
            {
                Theraot.No.Op(exception);
            }
            try
            {
                stream.Write(buffer, 0, 0);
                Assert.Fail();
            }
            catch (ObjectDisposedException exception)
            {
                Theraot.No.Op(exception);
            }
        }

        [Test]
        public void LargeRead()
        {
            var stream = CreateStream();
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 10);
            stream.Write(new byte[]
            {
                0, 2, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 3, 0, 0, 0, 0, 0, 0, 0
            }, 0, 20);
            stream.Write(new byte[]
            {
                0, 0, 0, 4, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 5, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 6, 0, 0, 0, 0,
            }, 0, 30);
            stream.Write(new byte[]
            {
                0, 0, 0, 0, 0, 0, 7, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 8, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 9, 0,
            }, 0, 30);
            stream.Write(new byte[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 2, 0, 0,
            }, 0, 30);
            stream.Write(new byte[]
            {
                0, 0, 0, 0, 0, 1, 2, 3, 4, 5
            }, 0, 10);
            var total = new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0 - 10
                                     0, 2, 0, 0, 0, 0, 0, 0, 0, 0, // 10 - 20
                                     0, 0, 3, 0, 0, 0, 0, 0, 0, 0, // 20 - 30
                                     0, 0, 0, 4, 0, 0, 0, 0, 0, 0, // 30 - 40
                                     0, 0, 0, 0, 5, 0, 0, 0, 0, 0, // 40 - 50
                                     0, 0, 0, 0, 0, 6, 0, 0, 0, 0, // 50 - 60
                                     0, 0, 0, 0, 0, 0, 7, 0, 0, 0, // 60 - 70
                                     0, 0, 0, 0, 0, 0, 0, 8, 0, 0, // 70 - 80
                                     0, 0, 0, 0, 0, 0, 0, 0, 9, 0, // 80 - 90
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 90 - 100
                                     0, 0, 0, 0, 0, 0, 0, 0, 1, 0, // 100 - 110
                                     0, 0, 0, 0, 0, 0, 0, 2, 0, 0, // 110 - 120
                                     0, 0, 0, 0, 0, 1, 2, 3, 4, 5 };
            var got = new byte[130];
            stream.Seek(0, SeekOrigin.Begin);
            var count = stream.Read(got, 0, 130);
            Assert.AreEqual(130, count);
            ArrayEquals(count, total, got);
        }

        [Test]
        public void LargeWrite()
        {
            var stream = CreateStream();
            stream.Seek(0, SeekOrigin.Begin);
            var total = new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0 - 10
                                     0, 2, 0, 0, 0, 0, 0, 0, 0, 0, // 10 - 20
                                     0, 0, 3, 0, 0, 0, 0, 0, 0, 0, // 20 - 30
                                     0, 0, 0, 4, 0, 0, 0, 0, 0, 0, // 30 - 40
                                     0, 0, 0, 0, 5, 0, 0, 0, 0, 0, // 40 - 50
                                     0, 0, 0, 0, 0, 6, 0, 0, 0, 0, // 50 - 60
                                     0, 0, 0, 0, 0, 0, 7, 0, 0, 0, // 60 - 70
                                     0, 0, 0, 0, 0, 0, 0, 8, 0, 0, // 70 - 80
                                     0, 0, 0, 0, 0, 0, 0, 0, 9, 0, // 80 - 90
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 90 - 100
                                     0, 0, 0, 0, 0, 0, 0, 0, 1, 0, // 100 - 110
                                     0, 0, 0, 0, 0, 0, 0, 2, 0, 0, // 110 - 120
                                     0, 0, 0, 0, 0, 1, 2, 3, 4, 5 };
            stream.Write(total, 0, 130);

            var got = new byte[30];

            stream.Seek(0, SeekOrigin.Begin);
            var expected = new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var count = stream.Read(got, 0, 10);
            Assert.AreEqual(10, count);
            ArrayEquals(count, expected, got);

            stream.Seek(30, SeekOrigin.Begin);
            expected = new byte[]
            {
                0, 0, 0, 4, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 5, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 6, 0, 0, 0, 0,
            };
            count = stream.Read(got, 0, 30);
            Assert.AreEqual(30, count);
            ArrayEquals(count, expected, got);

            expected = new byte[]
            {
                0, 0, 0, 0, 0, 0, 7, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 8, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 9, 0,
            };
            count = stream.Read(got, 0, 30);
            Assert.AreEqual(30, count);
            ArrayEquals(count, expected, got);

            expected = new byte[]
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 0, 0, 2, 0, 0,
            };
            count = stream.Read(got, 0, 30);
            Assert.AreEqual(30, count);
            ArrayEquals(count, expected, got);

            expected = new byte[]
            {
                0, 0, 0, 0, 0, 1, 2, 3, 4, 5
            };
            count = stream.Read(got, 0, 10);
            Assert.AreEqual(10, count);
            ArrayEquals(count, expected, got);

            stream.SetLength(20);

            stream.Seek(0, SeekOrigin.Begin);
            expected = new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            count = stream.Read(got, 0, 10);
            Assert.AreEqual(10, count);
            ArrayEquals(count, expected, got);

            expected = new byte[] { 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
            count = stream.Read(got, 0, 10);
            Assert.AreEqual(10, count);
            ArrayEquals(count, expected, got);
            count = stream.Read(got, 0, 10);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ReadBefore()
        {
            var stream = CreateStream();
            stream.Seek(10, SeekOrigin.Begin);
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);

            var got = new byte[16];
            var empty = new byte[10];
            //                       0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15
            var total = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5 };
            //                         0  1  2  3  4  5  6  7  8
            var partial = new byte[] { 0, 0, 0, 0, 1, 2, 3, 4, 5 };

            stream.Seek(0, SeekOrigin.Begin);
            var count = stream.Read(got, 0, 6);
            Assert.AreEqual(6, count);
            ArrayEquals(count, empty, got);

            stream.Seek(0, SeekOrigin.Begin);
            count = stream.Read(got, 0, 10);
            Assert.AreEqual(10, count);
            ArrayEquals(count, empty, got);

            stream.Seek(0, SeekOrigin.Begin);
            count = stream.Read(got, 0, 16);
            Assert.AreEqual(16, count);
            ArrayEquals(count, total, got);

            stream.Seek(0, SeekOrigin.Begin);
            try
            {
                stream.Read(got, 0, 20);
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                Theraot.No.Op(exception);
            }

            stream.Seek(0, SeekOrigin.Begin);
            count = stream.Read(got, 0, 6);
            Assert.AreEqual(6, count);
            ArrayEquals(count, empty, got);

            count = stream.Read(got, 0, 4);
            Assert.AreEqual(4, count);
            ArrayEquals(count, empty, got);

            count = stream.Read(got, 0, 6);
            Assert.AreEqual(6, count);
            ArrayEquals(count, data, got);

            stream.Seek(7, SeekOrigin.Begin);
            count = stream.Read(got, 0, 9);
            Assert.AreEqual(9, count);
            ArrayEquals(count, partial, got);

            stream.Seek(7, SeekOrigin.Begin);
            count = stream.Read(got, 0, 16);
            Assert.AreEqual(9, count);
            ArrayEquals(count, partial, got);
        }

        [Test]
        public void ReadBefore2()
        {
            var stream = CreateStream();
            stream.Seek(74, SeekOrigin.Begin);
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);

            var got = new byte[80];
            var empty = new byte[64];
            //                       0  1  2  3  4  5  6  7  8  9
            var total = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0 - 10
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 10 - 20
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 20 - 30
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 30 - 40
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 40 - 50
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 50 - 60
                                     0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 60 - 70
                                     0, 0, 0, 0, 0, 1, 2, 3, 4, 5 };
            //                         0  1  2  3  4  5  6  7  8
            var partial = new byte[] { 0, 0, 0, 0, 1, 2, 3, 4, 5 };

            stream.Seek(0, SeekOrigin.Begin);
            var count = stream.Read(got, 0, 6);
            Assert.AreEqual(6, count);
            ArrayEquals(count, empty, got);

            stream.Seek(0, SeekOrigin.Begin);
            count = stream.Read(got, 0, 10);
            Assert.AreEqual(10, count);
            ArrayEquals(count, empty, got);

            stream.Seek(0, SeekOrigin.Begin);
            count = stream.Read(got, 0, 64);
            Assert.AreEqual(64, count);
            ArrayEquals(count, empty, got);

            stream.Seek(0, SeekOrigin.Begin);
            count = stream.Read(got, 0, 80);
            Assert.AreEqual(80, count);
            ArrayEquals(count, total, got);

            stream.Seek(0, SeekOrigin.Begin);
            try
            {
                stream.Read(got, 0, 90);
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                Theraot.No.Op(exception);
            }

            stream.Seek(0, SeekOrigin.Begin);
            count = stream.Read(got, 0, 30);
            Assert.AreEqual(30, count);
            ArrayEquals(count, empty, got);

            count = stream.Read(got, 0, 44);
            Assert.AreEqual(44, count);
            ArrayEquals(count, empty, got);

            count = stream.Read(got, 0, 4);
            Assert.AreEqual(4, count);
            ArrayEquals(count, data, got);

            stream.Seek(71, SeekOrigin.Begin);
            count = stream.Read(got, 0, 9);
            Assert.AreEqual(9, count);
            ArrayEquals(count, partial, got);

            stream.Seek(71, SeekOrigin.Begin);
            count = stream.Read(got, 0, 16);
            Assert.AreEqual(9, count);
            ArrayEquals(count, partial, got);
        }

        [Test]
        public void ReadBeyondEnd()
        {
            var stream = CreateStream();
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);
            var got = new byte[6];
            // No Seek
            var count = stream.Read(got, 0, 6);
            Assert.AreEqual(0, count);
            stream.Seek(0, SeekOrigin.Begin); // <- Seek and read more than there is
            try
            {
                stream.Read(got, 0, 10);
                Assert.Fail();
            }
            catch (ArgumentException exception)
            {
                Theraot.No.Op(exception);
            }
        }

        [Test]
        public void ReadEmpty()
        {
            var stream = CreateStream();
            var got = new byte[6];
            var count = stream.Read(got, 0, 6);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ReadMiddle()
        {
            var stream = CreateStream();
            stream.Seek(10, SeekOrigin.Begin);
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(data, 0, 6);

            var got = new byte[16];

            stream.Seek(6, SeekOrigin.Begin);
            var count = stream.Read(got, 0, 5);
            Assert.AreEqual(5, count);
            ArrayEquals(count, new byte[] { 0, 0, 0, 0, 0 }, got);

            stream.Seek(7, SeekOrigin.Begin);
            count = stream.Read(got, 0, 9);
            Assert.AreEqual(9, count);
            ArrayEquals(count, new byte[] { 0, 0, 0, 0, 1, 2, 3, 4, 5 }, got);
        }

        [Test]
        public void ReadMiddle2()
        {
            var stream = CreateStream();
            stream.Seek(74, SeekOrigin.Begin);
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(data, 0, 6);

            var got = new byte[80];

            stream.Seek(6, SeekOrigin.Begin);
            var count = stream.Read(got, 0, 5);
            Assert.AreEqual(5, count);
            ArrayEquals(count, new byte[] { 0, 0, 0, 0, 0 }, got);

            stream.Seek(71, SeekOrigin.Begin);
            count = stream.Read(got, 0, 9);
            Assert.AreEqual(9, count);
            ArrayEquals(count, new byte[] { 0, 0, 0, 0, 1, 2, 3, 4, 5 }, got);
        }

        [Test]
        public void SeekTest()
        {
            var stream = CreateStream();
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);
            Assert.AreEqual(6, stream.Position);
            stream.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(0, stream.Position);
            try
            {
                stream.Seek(-1, SeekOrigin.Begin);
                Assert.Fail();
            }
            catch (IOException exception)
            {
                Theraot.No.Op(exception);
            }
            Assert.AreEqual(int.MaxValue + 1L, stream.Seek(int.MaxValue + 1L, SeekOrigin.Begin));
            Assert.AreEqual(int.MaxValue + 1L, stream.Position);
        }

        [Test]
        public void SimpleWriteRead()
        {
            var stream = CreateStream();
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);
            var got = new byte[6];
            stream.Seek(0, SeekOrigin.Begin); // <- remember to seek
            var count = stream.Read(got, 0, 6);
            Assert.AreEqual(6, count);
            ArrayEquals(count, data, got);
        }

        [Test]
        public void WriteBefore()
        {
            var stream = CreateStream();
            stream.Seek(10, SeekOrigin.Begin);
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(data, 0, 6);
            stream.Seek(0, SeekOrigin.Begin);
            //                         0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15
            var expected = new byte[] { 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5 };
            var got = new byte[16];
            var count = stream.Read(got, 0, 16);
            Assert.AreEqual(16, count);
            ArrayEquals(count, expected, got);
        }

        [Test]
        public void WriteBefore2()
        {
            var stream = CreateStream();
            stream.Seek(74, SeekOrigin.Begin);
            var data = new byte[] { 0, 1, 2, 3, 4, 5 };
            stream.Write(data, 0, 6);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(data, 0, 6);
            stream.Seek(0, SeekOrigin.Begin);
            //                          0  1  2  3  4  5  6  7  8  9
            var expected = new byte[] { 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, // 0 - 10
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 10 - 20
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 20 - 30
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 30 - 40
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 40 - 50
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 50 - 60
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 60 - 70
                                        0, 0, 0, 0, 0, 1, 2, 3, 4, 5 };
            var got = new byte[80];
            var count = stream.Read(got, 0, 80);
            Assert.AreEqual(80, count);
            ArrayEquals(count, expected, got);
        }

        private static void ArrayEquals(int count, byte[] data, byte[] got)
        {
            for (var index = 0; index < count; index++)
            {
                Assert.AreEqual(data[index], got[index]);
            }
        }

        private static Stream CreateStream()
        {
            var stream = new RamStream();
            return stream;
        }
    }
}

#endif