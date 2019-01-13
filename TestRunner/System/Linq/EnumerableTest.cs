using System;
using System.Linq;

namespace TestRunner.System.Linq
{
    [TestFixture]
    public static class EnumerableTest
    {
        [Test]
        public static void MaxOfEmptyNullableEnumerableIsNull()
        {
            Assert.AreEqual(null, Enumerable.Max(new double?[]{}));
            Assert.AreEqual(null, Enumerable.Max(new float?[]{}));
        }

        [Test]
        public static void MaxOfEmptyEnumerableThrows()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidOperationException>(() => Enumerable.Max(new double[]{}));
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidOperationException>(() => Enumerable.Max(new float[]{}));
        }

        [Test]
        public static void MaxOfNaNisNaN()
        {
            Assert.IsTrue(double.IsNaN(Enumerable.Max(new []{double.NaN})));
            Assert.IsTrue(float.IsNaN(Enumerable.Max(new []{float.NaN})));
        }

        [Test]
        public static void MaxOfNullableNaNisNaN()
        {
            // ReSharper disable once PossibleInvalidOperationException
            Assert.IsTrue(double.IsNaN(Enumerable.Max(new double?[]{double.NaN}).Value));
            // ReSharper disable once PossibleInvalidOperationException
            Assert.IsTrue(float.IsNaN(Enumerable.Max(new float?[]{float.NaN}).Value));
        }

        [Test]
        public static void MaxOfNullableNaNAndNullIsNaN()
        {
            // ReSharper disable once PossibleInvalidOperationException
            Assert.IsTrue(double.IsNaN(Enumerable.Max(new double?[]{double.NaN, null}).Value));
            // ReSharper disable once PossibleInvalidOperationException
            Assert.IsTrue(float.IsNaN(Enumerable.Max(new float?[]{float.NaN, null}).Value));
            // ReSharper disable once PossibleInvalidOperationException
            Assert.IsTrue(double.IsNaN(Enumerable.Max(new double?[]{null, double.NaN}).Value));
            // ReSharper disable once PossibleInvalidOperationException
            Assert.IsTrue(float.IsNaN(Enumerable.Max(new float?[]{null, float.NaN}).Value));
        }

        [Test]
        public static void MaxOfNaNMinValueIsMinValue()
        {
            Assert.AreEqual(double.MinValue, Enumerable.Max(new []{double.NaN, double.MinValue}));
            Assert.AreEqual(float.MinValue, Enumerable.Max(new []{float.NaN, float.MinValue}));
            Assert.AreEqual(double.MinValue, Enumerable.Max(new []{double.MinValue, double.NaN}));
            Assert.AreEqual(float.MinValue, Enumerable.Max(new []{float.MinValue, float.NaN}));
        }

        [Test]
        public static void MaxOfNaNMinValueWithNullsIsMinValue()
        {
            Assert.AreEqual(double.MinValue, Enumerable.Max(new double?[]{null, double.NaN, double.MinValue}));
            Assert.AreEqual(float.MinValue, Enumerable.Max(new float?[]{null, float.NaN, float.MinValue}));
            Assert.AreEqual(double.MinValue, Enumerable.Max(new double?[]{null, double.MinValue, double.NaN}));
            Assert.AreEqual(float.MinValue, Enumerable.Max(new float?[]{null, float.MinValue, float.NaN}));

            Assert.AreEqual(double.MinValue, Enumerable.Max(new double?[]{double.NaN, null, double.MinValue}));
            Assert.AreEqual(float.MinValue, Enumerable.Max(new float?[]{float.NaN, null, float.MinValue}));
            Assert.AreEqual(double.MinValue, Enumerable.Max(new double?[]{double.MinValue, null, double.NaN}));
            Assert.AreEqual(float.MinValue, Enumerable.Max(new float?[]{float.MinValue, null, float.NaN}));

            Assert.AreEqual(double.MinValue, Enumerable.Max(new double?[]{double.NaN, double.MinValue, null}));
            Assert.AreEqual(float.MinValue, Enumerable.Max(new float?[]{float.NaN, float.MinValue, null}));
            Assert.AreEqual(double.MinValue, Enumerable.Max(new double?[]{double.MinValue, double.NaN, null}));
            Assert.AreEqual(float.MinValue, Enumerable.Max(new float?[]{float.MinValue, float.NaN, null}));
        }
    }
}
