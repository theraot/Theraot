// ReSharper disable InvokeAsExtensionMethod

using System;
using NUnit.Framework;
using Theraot.Core;

namespace Tests.Theraot.Core
{
    [TestFixture]
    internal class StringHelperTest
    {
        [Test]
        public void AppendTests()
        {
            Assert.AreEqual("A", StringHelper.Append("A"));
            Assert.AreEqual("AB", StringHelper.Append("A", "B"));
            Assert.AreEqual("ABC", StringHelper.Append("A", "B", "C"));
            Assert.AreEqual("ABCD", StringHelper.Append("A", "B", "C", "D"));
            Assert.AreEqual("ABCDE", StringHelper.Append("A", "B", "C", "D", "E"));
        }

        [Test]
        public void EndTests()
        {
            Assert.AreEqual("", StringHelper.End("ABCDE", 0));
            Assert.AreEqual("E", StringHelper.End("ABCDE", 1));
            Assert.AreEqual("DE", StringHelper.End("ABCDE", 2));
            Assert.AreEqual("CDE", StringHelper.End("ABCDE", 3));
            Assert.AreEqual("BCDE", StringHelper.End("ABCDE", 4));
            Assert.AreEqual("ABCDE", StringHelper.End("ABCDE", 5));
            Assert.AreEqual("ABCDE", StringHelper.End("ABCDE", 6));
            Assert.AreEqual("ABCDE", StringHelper.End("ABCDE", 7));
        }

        [Test]
        public void EnsureEndTests()
        {
            Assert.AreEqual("ABA", StringHelper.EnsureEnd("AB", "A", StringComparison.Ordinal));
            Assert.AreEqual("AB", StringHelper.EnsureEnd("AB", "B", StringComparison.Ordinal));
            Assert.AreEqual("ABC", StringHelper.EnsureEnd("AB", "C", StringComparison.Ordinal));
        }

        [Test]
        public void EnsureStartTests()
        {
            Assert.AreEqual("AB", StringHelper.EnsureStart("AB", "A"));
            Assert.AreEqual("BAB", StringHelper.EnsureStart("AB", "B"));
            Assert.AreEqual("CAB", StringHelper.EnsureStart("AB", "C"));
        }

        [Test]
        public void ExceptEndTests()
        {
            Assert.AreEqual("ABCDE", StringHelper.ExceptEnd("ABCDE", 0));
            Assert.AreEqual("ABCD", StringHelper.ExceptEnd("ABCDE", 1));
            Assert.AreEqual("ABC", StringHelper.ExceptEnd("ABCDE", 2));
            Assert.AreEqual("AB", StringHelper.ExceptEnd("ABCDE", 3));
            Assert.AreEqual("A", StringHelper.ExceptEnd("ABCDE", 4));
            Assert.AreEqual("", StringHelper.ExceptEnd("ABCDE", 5));
            Assert.AreEqual("", StringHelper.ExceptEnd("ABCDE", 6));
        }

        [Test]
        public void ExceptStartTests()
        {
            Assert.AreEqual("ABCDE", StringHelper.ExceptStart("ABCDE", 0));
            Assert.AreEqual("BCDE", StringHelper.ExceptStart("ABCDE", 1));
            Assert.AreEqual("CDE", StringHelper.ExceptStart("ABCDE", 2));
            Assert.AreEqual("DE", StringHelper.ExceptStart("ABCDE", 3));
            Assert.AreEqual("E", StringHelper.ExceptStart("ABCDE", 4));
            Assert.AreEqual("", StringHelper.ExceptStart("ABCDE", 5));
            Assert.AreEqual("", StringHelper.ExceptStart("ABCDE", 6));
        }

        [Test]
        public void NeglectEndTests()
        {
            Assert.AreEqual("AB", StringHelper.NeglectEnd("AB", "A", StringComparison.Ordinal));
            Assert.AreEqual("A", StringHelper.NeglectEnd("AB", "B", StringComparison.Ordinal));
            Assert.AreEqual("AB", StringHelper.NeglectEnd("AB", "C", StringComparison.Ordinal));
        }

        [Test]
        public void NeglectStartTests()
        {
            Assert.AreEqual("B", StringHelper.NeglectStart("AB", "A", StringComparison.Ordinal));
            Assert.AreEqual("AB", StringHelper.NeglectStart("AB", "B", StringComparison.Ordinal));
            Assert.AreEqual("AB", StringHelper.NeglectStart("AB", "C", StringComparison.Ordinal));
        }

        [Test]
        public void StartTests()
        {
            Assert.AreEqual("", StringHelper.Start("ABCDE", 0));
            Assert.AreEqual("A", StringHelper.Start("ABCDE", 1));
            Assert.AreEqual("AB", StringHelper.Start("ABCDE", 2));
            Assert.AreEqual("ABC", StringHelper.Start("ABCDE", 3));
            Assert.AreEqual("ABCD", StringHelper.Start("ABCDE", 4));
            Assert.AreEqual("ABCDE", StringHelper.Start("ABCDE", 5));
            Assert.AreEqual("ABCDE", StringHelper.Start("ABCDE", 6));
            Assert.AreEqual("ABCDE", StringHelper.Start("ABCDE", 7));
        }
    }
}