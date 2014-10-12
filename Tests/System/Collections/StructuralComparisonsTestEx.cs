using NUnit.Framework;
using System;
using System.Collections;

namespace MonoTests.System.Collections
{
    [TestFixture]
    public class StructuralComparisonsTestEx
    {
        [Test]
        public void DifferentArrays()
        {
            Assert.IsFalse(StructuralComparisons.StructuralEqualityComparer.Equals(new int[] { 0, 1, 2, 3 }, new int[] { 1, 1, 2, 2 }));
            Assert.IsFalse(StructuralComparisons.StructuralEqualityComparer.Equals(new int[] { 0, 1 }, new int[] { 3, 3, 3 }));
        }

        [Test]
        public void DifferentRank()
        {
            // Anything that's not Rank = 1 must fail
            Assert.Throws(typeof(ArgumentException), () => StructuralComparisons.StructuralEqualityComparer.Equals(new[,] { { 1, 1 }, { 2, 2 } }, new[] { 1, 1, 2, 2 }));
            Assert.Throws(typeof(ArgumentException), () => StructuralComparisons.StructuralEqualityComparer.Equals(new[,] { { 1, 1 }, { 2, 2 } }, new[,] { { 1, 1 }, { 2, 2 } }));
        }

        [Test]
        public void TupleToArray()
        {
            var a1 = new Tuple<int, int>(1, 2);
            var a2 = new int[] { 1, 2 };

            Assert.IsFalse(StructuralComparisons.StructuralEqualityComparer.Equals(a1, a2), "#1");
            // Tuple against null
            Assert.IsFalse(StructuralComparisons.StructuralEqualityComparer.Equals(null, a1), "#2");
            Assert.IsFalse(StructuralComparisons.StructuralEqualityComparer.Equals(a1, null), "#3");
        }
    }
}