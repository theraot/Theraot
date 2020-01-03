#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//
// Authors:
//      Federico Di Gregorio <fog@initd.org>

using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Tests.Helpers;

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestAddChecked
    {
        [Test]
        public void Arg1Null()
        {
            const int value = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.AddChecked(null, Expression.Constant(value)));
        }

        [Test]
        public void Arg2Null()
        {
            const int value = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.AddChecked(Expression.Constant(value), null));
        }

        [Test]
        public void ArgTypesDifferent()
        {
            const int valueLeft = 1;
            const double valueRight = 2.0;

            AssertEx.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(valueLeft), Expression.Constant(valueRight)));
        }

        [Test]
        public void Boolean()
        {
            const bool valueLeft = true;
            const bool valueRight = false;

            AssertEx.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(valueLeft), Expression.Constant(valueRight)));
        }

        [Test]
        public void NoOperatorClass()
        {
            var valueLeft = new NoOpClass();
            var valueRight = new NoOpClass();

            AssertEx.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(valueLeft), Expression.Constant(valueRight)));
        }

        [Test]
        public void Nullable()
        {
            int? valueLeft = 1;
            int? valueRight = 2;

            var binaryExpression = Expression.AddChecked(Expression.Constant(valueLeft), Expression.Constant(valueRight));
            Assert.AreEqual(ExpressionType.AddChecked, binaryExpression.NodeType, "AddChecked#04");
            Assert.AreEqual(typeof(int), binaryExpression.Type, "AddChecked#05");
            Assert.IsNull(binaryExpression.Method, null, "AddChecked#06");
            Assert.AreEqual($"({valueLeft} + {valueRight})", binaryExpression.ToString(), "AddChecked#16");
        }

        [Test]
        public void Numeric()
        {
            const int valueLeft = 1;
            const int valueRight = 2;

            var binaryExpression = Expression.AddChecked(Expression.Constant(valueLeft), Expression.Constant(valueRight));
            Assert.AreEqual(ExpressionType.AddChecked, binaryExpression.NodeType, "AddChecked#01");
            Assert.AreEqual(typeof(int), binaryExpression.Type, "AddChecked#02");
            Assert.IsNull(binaryExpression.Method, "AddChecked#03");
            Assert.AreEqual($"({valueLeft} + {valueRight})", binaryExpression.ToString(), "AddChecked#15");
        }

        [Test]
        public void TestNoOverflow()
        {
            // These should not overflow
            // Simple stuff
            MustNotOverflow(10, 20);

            // These are invalid:
            InvalidOperation<byte>(byte.MaxValue, 2);
            InvalidOperation<sbyte>(sbyte.MaxValue, 2);
            // Doubles, floats, do not overflow
            MustNotOverflow(float.MaxValue, 1);
            MustNotOverflow(double.MaxValue, 1);
        }

        [Test]
        public void TestOverflows()
        {
            // These should overflow, check the various types and code paths
            // in BinaryExpression:
            MustOverflow(int.MaxValue, 1);
            MustOverflow(int.MinValue, -11);
            MustOverflow(long.MaxValue, 1);
            MustOverflow(long.MinValue, -1);

            // unsigned values use Add_Ovf_Un, check that too:
            MustOverflow<ulong>(ulong.MaxValue, 1);
            MustOverflow<uint>(uint.MaxValue, 1);
        }

        [Test]
        public void UserDefinedClass()
        {
            var type = typeof(OpClass);

            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var method = type.GetMethod("op_Addition");

            var binaryExpression = Expression.AddChecked(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.AddChecked, binaryExpression.NodeType, "AddChecked#07");
            Assert.AreEqual(type, binaryExpression.Type, "AddChecked#08");
            Assert.AreEqual(method, binaryExpression.Method, "AddChecked#09");
            Assert.AreEqual
            (
                $"(value({type.FullName}) + value({type.FullName}))",
                binaryExpression.ToString(),
                "AddChecked#17"
            );
        }

        [Test]
        public void UserDefinedStruct()
        {
            var type = typeof(OpStruct);

            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var method = type.GetMethod("op_Addition");

            var additionExpression = Expression.AddChecked(Expression.Constant(new OpStruct()), Expression.Constant(new OpStruct()));
            Assert.AreEqual(ExpressionType.AddChecked, additionExpression.NodeType, "AddChecked#11");
            Assert.AreEqual(type, additionExpression.Type, "AddChecked#12");
            Assert.AreEqual(method, additionExpression.Method, "AddChecked#13");
            Assert.AreEqual
            (
                $"(value({type.FullName}) + value({type.FullName}))",
                additionExpression.ToString(),
                "AddChecked#18"
            );
        }

        private static void InvalidOperation<T>(T v1, T v2)
        {
            // SubtractChecked is not defined for small types (byte, sbyte)
            AssertEx.Throws<InvalidOperationException>
            (
                () => Expression.Lambda<Func<T>>
                (
                    Expression.AddChecked(Expression.Constant(v1), Expression.Constant(v2))
                )
            );
        }

        private static void MustNotOverflow<T>(T v1, T v2)
        {
            // This routine should execute the code, but not throw an
            // overflow exception
            var lambda = Expression.Lambda<Func<T>>
            (
                Expression.AddChecked(Expression.Constant(v1), Expression.Constant(v2))
            );
            var compiled = lambda.Compile();
            compiled();
        }

        private static void MustOverflow<T>(T v1, T v2)
        {
            // This method makes sure that compiling an AddChecked on two values
            // throws an OverflowException, if it does not, it fails
            var lambda = Expression.Lambda<Func<T>>
            (
                Expression.AddChecked(Expression.Constant(v1), Expression.Constant(v2))
            );
            var compiled = lambda.Compile();
            AssertEx.Throws<OverflowException, T>(compiled);
        }
    }
}