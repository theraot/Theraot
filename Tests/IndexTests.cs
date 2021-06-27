#if TARGETS_NET || LESSTHAN_NETSTANDARD21 || LESSTHAN_NETCOREAPP30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace System.Tests
{
    public static class IndexTests
    {
        [Test]
        public static void CreationTest()
        {
            Index index = new Index(1, fromEnd: false);
            Assert.AreEqual(1, index.Value);
            Assert.False(index.IsFromEnd);

            index = new Index(11, fromEnd: true);
            Assert.AreEqual(11, index.Value);
            Assert.True(index.IsFromEnd);

            index = Index.Start;
            Assert.AreEqual(0, index.Value);
            Assert.False(index.IsFromEnd);

            index = Index.End;
            Assert.AreEqual(0, index.Value);
            Assert.True(index.IsFromEnd);

            index = Index.FromStart(3);
            Assert.AreEqual(3, index.Value);
            Assert.False(index.IsFromEnd);

            index = Index.FromEnd(10);
            Assert.AreEqual(10, index.Value);
            Assert.True(index.IsFromEnd);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Index(-1, fromEnd: false));
            Assert.Throws<ArgumentOutOfRangeException>(() => Index.FromStart(-3));
            Assert.Throws<ArgumentOutOfRangeException>(() => Index.FromEnd(-1));
        }

        [Test]
        public static void GetOffsetTest()
        {
            Index index = Index.FromStart(3);
            Assert.AreEqual(3, index.GetOffset(3));
            Assert.AreEqual(3, index.GetOffset(10));
            Assert.AreEqual(3, index.GetOffset(20));

            // we don't validate the length in the GetOffset so passing short length will just return the regular calculation according to the length value.
            Assert.AreEqual(3, index.GetOffset(2));

            index = Index.FromEnd(3);
            Assert.AreEqual(0, index.GetOffset(3));
            Assert.AreEqual(7, index.GetOffset(10));
            Assert.AreEqual(17, index.GetOffset(20));

            // we don't validate the length in the GetOffset so passing short length will just return the regular calculation according to the length value.
            Assert.AreEqual(-1, index.GetOffset(2));
        }

        [Test]
        public static void ImplicitCastTest()
        {
            Index index = 10;
            Assert.AreEqual(10, index.Value);
            Assert.False(index.IsFromEnd);

            Assert.Throws<ArgumentOutOfRangeException>(() => index = -10 );
        }

        [Test]
        public static void EqualityTest()
        {
            Index index1 = 10;
            Index index2 = 10;
            Assert.True(index1.Equals(index2));
            Assert.True(index1.Equals((object)index2));

            index2 = new Index(10, fromEnd: true);
            Assert.False(index1.Equals(index2));
            Assert.False(index1.Equals((object)index2));

            index2 = new Index(9, fromEnd: false);
            Assert.False(index1.Equals(index2));
            Assert.False(index1.Equals((object)index2));
        }

        [Test]
        public static void HashCodeTest()
        {
            Index index1 = 10;
            Index index2 = 10;
            Assert.AreEqual(index1.GetHashCode(), index2.GetHashCode());

            index2 = new Index(10, fromEnd: true);
            Assert.AreNotEqual(index1.GetHashCode(), index2.GetHashCode());

            index2 = new Index(99999, fromEnd: false);
            Assert.AreNotEqual(index1.GetHashCode(), index2.GetHashCode());
        }

        [Test]
        public static void ToStringTest()
        {
            Index index1 = 100;
            Assert.AreEqual(100.ToString(), index1.ToString());

            index1 = new Index(50, fromEnd: true);
            Assert.AreEqual("^" + 50.ToString(), index1.ToString());
        }
    }
}

#endif