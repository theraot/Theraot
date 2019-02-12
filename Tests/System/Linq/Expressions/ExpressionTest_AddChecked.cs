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
//		Federico Di Gregorio <fog@initd.org>

using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Tests.Helpers;

#if TARGETS_NETCORE || TARGETS_NETSTANDARD
using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestAddChecked
    {
        private static void InvalidOperation<T>(T v1, T v2)
        {
            // SubtractChecked is not defined for small types (byte, sbyte)
            Assert.Throws<InvalidOperationException>
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
            var method = lambda.Compile();
            method();
        }

        private static void MustOverflow<T>(T v1, T v2)
        {
            // This method makes sure that compiling an AddChecked on two values
            // throws an OverflowException, if it does not, it fails
            var lambda = Expression.Lambda<Func<T>>
            (
                Expression.AddChecked(Expression.Constant(v1), Expression.Constant(v2))
            );
            var method = lambda.Compile();
            AssertEx.Throws<OverflowException, T>(method);
        }

        [Test]
        public void Arg1Null()
        {
            const int Value = 1;

            Assert.Throws<ArgumentNullException>(() => Expression.AddChecked(null, Expression.Constant(Value)));
        }

        [Test]
        public void Arg2Null()
        {
            const int Value = 1;

            Assert.Throws<ArgumentNullException>(() => Expression.AddChecked(Expression.Constant(Value), null));
        }

        [Test]
        public void ArgTypesDifferent()
        {
            const int ValueLeft = 1;
            const double ValueRight = 2.0;

            Assert.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(ValueLeft), Expression.Constant(ValueRight)));
        }

        [Test]
        public void Boolean()
        {
            const bool ValueLeft = true;
            const bool ValueRight = false;

            Assert.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(ValueLeft), Expression.Constant(ValueRight)));
        }

        [Test]
        public void NoOperatorClass()
        {
            var valueLeft = new NoOpClass();
            var valueRight = new NoOpClass();

            Assert.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(valueLeft), Expression.Constant(valueRight)));
        }

        [Test]
        public void Nullable()
        {
            int? valueLeft = 1;
            int? valueRight = 2;

            var additionExpression = Expression.AddChecked(Expression.Constant(valueLeft), Expression.Constant(valueRight));
            Assert.AreEqual(ExpressionType.AddChecked, additionExpression.NodeType, "AddChecked#04");
            Assert.AreEqual(typeof(int), additionExpression.Type, "AddChecked#05");
            Assert.IsNull(additionExpression.Method, null, "AddChecked#06");
            Assert.AreEqual($"({valueLeft} + {valueRight})", additionExpression.ToString(), "AddChecked#16");
        }

        [Test]
        public void Numeric()
        {
            const int ValueLeft = 1;
            const int ValueRight = 2;

            var additionExpression = Expression.AddChecked(Expression.Constant(ValueLeft), Expression.Constant(ValueRight));
            Assert.AreEqual(ExpressionType.AddChecked, additionExpression.NodeType, "AddChecked#01");
            Assert.AreEqual(typeof(int), additionExpression.Type, "AddChecked#02");
            Assert.IsNull(additionExpression.Method, "AddChecked#03");
            Assert.AreEqual($"({ValueLeft} + {ValueRight})", additionExpression.ToString(), "AddChecked#15");
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
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var additionOperator = typeof(OpClass).GetMethod("op_Addition");

            var additionExpression = Expression.AddChecked(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.AddChecked, additionExpression.NodeType, "AddChecked#07");
            Assert.AreEqual(typeof(OpClass), additionExpression.Type, "AddChecked#08");
            Assert.AreEqual(additionOperator, additionExpression.Method, "AddChecked#09");
            Assert.AreEqual("op_Addition", additionExpression.Method.Name, "AddChecked#10");
            Assert.AreEqual
            (
                $"(value({typeof(OpClass).FullName}) + value({typeof(OpClass).FullName}))",
                additionExpression.ToString(),
                "AddChecked#17"
            );
        }

        [Test]
        public void UserDefinedStruct()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var additionOperator = typeof(OpStruct).GetMethod("op_Addition");

            var additionExpression = Expression.AddChecked(Expression.Constant(new OpStruct()), Expression.Constant(new OpStruct()));
            Assert.AreEqual(ExpressionType.AddChecked, additionExpression.NodeType, "AddChecked#11");
            Assert.AreEqual(typeof(OpStruct), additionExpression.Type, "AddChecked#12");
            Assert.AreEqual(additionOperator, additionExpression.Method, "AddChecked#13");
            Assert.AreEqual("op_Addition", additionExpression.Method.Name, "AddChecked#14");
            Assert.AreEqual
            (
                $"(value({typeof(OpStruct).FullName}) + value({typeof(OpStruct).FullName}))",
                additionExpression.ToString(),
                "AddChecked#18"
            );
        }
    }
}