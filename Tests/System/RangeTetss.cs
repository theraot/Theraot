#if TARGETS_NET || LESSTHAN_NETSTANDARD21 || LESSTHAN_NETCOREAPP30

// ReSharper disable RedundantRangeBound

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq;
using NUnit.Framework;

namespace System.Tests
{
    public static class RangeTests
    {
        [Test]
        public static void CreationTest()
        {
            Range range = new Range(new Index(10, fromEnd: false), new Index(2, fromEnd: true));
            Assert.AreEqual(10, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.AreEqual(2, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = Range.StartAt(new Index(7, fromEnd: false));
            Assert.AreEqual(7, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.AreEqual(0, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = Range.EndAt(new Index(3, fromEnd: true));
            Assert.AreEqual(0, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.AreEqual(3, range.End.Value);
            Assert.True(range.End.IsFromEnd);

            range = Range.All;
            Assert.AreEqual(0, range.Start.Value);
            Assert.False(range.Start.IsFromEnd);
            Assert.AreEqual(0, range.End.Value);
            Assert.True(range.End.IsFromEnd);
        }

        [Test]
        public static void CustomTypeTest()
        {
            CustomRangeTester crt = new CustomRangeTester(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            for (int i = 0; i < crt.Length; i++)
            {
                Assert.AreEqual(crt[i], crt[Index.FromStart(i)]);
                Assert.AreEqual(crt[crt.Length - i - 1], crt[^(i + 1)]);

                Assert.True(crt.Slice(i, crt.Length - i).Equals(crt[i..^0]), $"Index = {i} and {crt.Slice(i, crt.Length - i)} != {crt[i..^0]}");
            }
        }

        [Test]
        public static void EqualityTest()
        {
            Range range1 = new Range(new Index(10, fromEnd: false), new Index(20, fromEnd: false));
            Range range2 = new Range(new Index(10, fromEnd: false), new Index(20, fromEnd: false));
            Assert.True(range1.Equals(range2));
            Assert.True(range1.Equals((object)range2));

            range2 = new Range(new Index(10, fromEnd: false), new Index(20, fromEnd: true));
            Assert.False(range1.Equals(range2));
            Assert.False(range1.Equals((object)range2));

            range2 = new Range(new Index(10, fromEnd: false), new Index(21, fromEnd: false));
            Assert.False(range1.Equals(range2));
            Assert.False(range1.Equals((object)range2));
        }

        [Test]
        public static void GetOffsetAndLengthTest()
        {
            Range range = Range.StartAt(new Index(5));
            (int offset, int length) = range.GetOffsetAndLength(20);
            Assert.AreEqual(5, offset);
            Assert.AreEqual(15, length);

            (offset, length) = range.GetOffsetAndLength(5);
            Assert.AreEqual(5, offset);
            Assert.AreEqual(0, length);

            // we don't validate the length in the GetOffsetAndLength so passing negative length will just return the regular calculation according to the length value.
            (offset, length) = range.GetOffsetAndLength(-10);
            Assert.AreEqual(5, offset);
            Assert.AreEqual(-15, length);

            Assert.Throws<ArgumentOutOfRangeException>(() => range.GetOffsetAndLength(4));

            range = Range.EndAt(new Index(4));
            (offset, length) = range.GetOffsetAndLength(20);
            Assert.AreEqual(0, offset);
            Assert.AreEqual(4, length);
            Assert.Throws<ArgumentOutOfRangeException>(() => range.GetOffsetAndLength(1));
        }

        [Test]
        public static void HashCodeTest()
        {
            Range range1 = new Range(new Index(10, fromEnd: false), new Index(20, fromEnd: false));
            Range range2 = new Range(new Index(10, fromEnd: false), new Index(20, fromEnd: false));
            Assert.AreEqual(range1.GetHashCode(), range2.GetHashCode());

            range2 = new Range(new Index(10, fromEnd: false), new Index(20, fromEnd: true));
            Assert.AreNotEqual(range1.GetHashCode(), range2.GetHashCode());

            range2 = new Range(new Index(10, fromEnd: false), new Index(21, fromEnd: false));
            Assert.AreNotEqual(range1.GetHashCode(), range2.GetHashCode());
        }

        [Test]
        public static void ToStringTest()
        {
            Range range1 = new Range(new Index(10, fromEnd: false), new Index(20, fromEnd: false));
            Assert.AreEqual(10.ToString() + ".." + 20.ToString(), range1.ToString());

            range1 = new Range(new Index(10, fromEnd: false), new Index(20, fromEnd: true));
            Assert.AreEqual(10.ToString() + "..^" + 20.ToString(), range1.ToString());
        }

        // CustomRangeTester is a custom class which containing the members Length, Slice and int indexer.
        // Having these members allow the C# compiler to support
        //      this[Index]
        //      this[Range]
        private sealed class CustomRangeTester : IEquatable<CustomRangeTester>
        {
            public CustomRangeTester(int[] data) => Data = data;

            public int Length => Data.Length;
            private int[] Data { get; }
            public int this[int index] => Data[index];

            public bool Equals(CustomRangeTester other)
            {
                if (Data.Length != other.Data.Length)
                {
                    return false;
                }

                for (int i = 0; i < Data.Length; i++)
                {
                    if (Data[i] != other.Data[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public CustomRangeTester Slice(int start, int length) => new CustomRangeTester(Data.Skip(start).Take(length).ToArray());

            public override string ToString()
            {
                if (Length == 0)
                {
                    return "[]";
                }

                string s = "[" + Data[0];

                for (int i = 1; i < Length; i++)
                {
                    s = s + ", " + Data[i];
                }

                return s + "]";
            }
        }
    }
}

#endif