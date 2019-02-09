#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Collections;

namespace Tests.Theraot.Collections
{
    [TestFixture]
    public class ProgressiveLookupTest
    {
        [Test]
        public void EmptyResult()
        {
            var lookup = ProgressiveLookup<string, int>.Create
                (
                    GetColors(),
                    c => c.Name,
                    c => c.Value,
                    StringComparer.OrdinalIgnoreCase
                );

            var l = lookup["notExist"];
            Assert.IsNotNull(l);
            var values = l.AsArray();
            Assert.AreEqual(values.Length, 0);
        }

        [Test]
        public void LookupContains()
        {
            var lookup = ProgressiveLookup<string, string>.Create
                (
                    new[] { "hi", "bye" },
                    c => c[0].ToString()
                );

            Assert.IsTrue(lookup.Contains("h"));
            Assert.IsFalse(lookup.Contains("d"));
            Assert.IsFalse(lookup.Contains(null));
        }

        [Test]
        public void LookupContainsNull()
        {
            var lookup = ProgressiveLookup<string, string>.Create
                (
                    new[] { "hi", "bye", "42" },
                    c => char.IsNumber(c[0]) ? null : c[0].ToString()
                );

            Assert.IsTrue(lookup.Contains("h"));
            Assert.IsTrue(lookup.Contains(null));
            Assert.IsFalse(lookup.Contains("d"));
        }

        [Test]
        public void LookupEnumeratorWithNull()
        {
            var lookup = ProgressiveLookup<string, string>.Create
                (
                    new[] { "hi", "bye", "42" },
                    c => char.IsNumber(c[0]) ? null : c[0].ToString()
                );

            Assert.IsTrue(lookup.Any(g => g.Key == "h"));
            Assert.IsTrue(lookup.Any(g => g.Key == "b"));
            Assert.IsTrue(lookup.Any(g => g.Key == null));
        }

        [Test]
        public void LookupEnumeratorWithoutNull()
        {
            var lookup = ProgressiveLookup<string, string>.Create
                (
                    new[] { "hi", "bye" },
                    c => c[0].ToString()
                );

            Assert.IsTrue(lookup.Any(g => g.Key == "h"));
            Assert.IsTrue(lookup.Any(g => g.Key == "b"));
            Assert.IsFalse(lookup.Any(g => g.Key == null));
        }

        [Test]
        public void LookupIgnoreCase()
        {
            var lookup = ProgressiveLookup<string, int>.Create
                (
                    GetColors(),
                    c => c.Name,
                    c => c.Value,
                    StringComparer.OrdinalIgnoreCase
                );

            Assert.AreEqual(0xff0000, lookup["red"].First());
            Assert.AreEqual(0x00ff00, lookup["GrEeN"].First());
            Assert.AreEqual(0x0000ff, lookup["Blue"].First());
        }

        [Test]
        public void LookupNullKeyNone()
        {
            var lookup = ProgressiveLookup<string, string>.Create
                (
                    new[] { "hi", "bye" },
                    c => c[0].ToString()
                );

            Assert.AreEqual(2, lookup.Count);
            Assert.AreEqual(0, lookup[null].Count());
        }

        private static IEnumerable<Color> GetColors()
        {
            yield return new Color("Red", 0xff0000);
            yield return new Color("Green", 0x00ff00);
            yield return new Color("Blue", 0x0000ff);
        }

        private class Color
        {
            public Color(string name, int value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; }

            public int Value { get; }
        }
    }
}