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
using System.Reflection;
using NUnit.Framework;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestAddChecked
    {
        //
        // This method makes sure that compiling an AddChecked on two values
        // throws an OverflowException, if it doesnt, it fails
        //
        private static void MustOverflow<T>(T v1, T v2)
        {
            var l = Expression.Lambda<Func<T>>
            (
                Expression.AddChecked(Expression.Constant(v1), Expression.Constant(v2))
            );
            var del = l.Compile();
            T res;
            try
            {
                res = del();
            }
            catch (OverflowException)
            {
                // OK
                return;
            }

            throw new Exception
            (
                string.Format
                (
                    "AddChecked on {2} should have thrown an exception with values {0} {1}, result was: {3}",
                    v1, v2, v1.GetType(), res
                )
            );
        }

        //
        // This routine should execute the code, but not throw an
        // overflow exception
        //
        private static void MustNotOverflow<T>(T v1, T v2)
        {
            var l = Expression.Lambda<Func<T>>
            (
                Expression.AddChecked(Expression.Constant(v1), Expression.Constant(v2))
            );
            var del = l.Compile();
            del();
        }

        //
        // SubtractChecked is not defined for small types (byte, sbyte)
        //
        private static void InvalidOperation<T>(T v1, T v2)
        {
            try
            {
                Expression.Lambda<Func<T>>
                (
                    Expression.AddChecked(Expression.Constant(v1), Expression.Constant(v2))
                );
            }
            catch (InvalidOperationException)
            {
                // OK
                return;
            }

            throw new Exception(string.Format("AddChecked should have thrown for the creation of a tree with {0} operands", v1.GetType()));
        }

        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.AddChecked(null, Expression.Constant(1)));
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.AddChecked(Expression.Constant(1), null));
        }

        [Test]
        public void ArgTypesDifferent()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(1), Expression.Constant(2.0)));
        }

        [Test]
        public void Boolean()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(true), Expression.Constant(false)));
        }

        [Test]
        public void NoOperatorClass()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.AddChecked(Expression.Constant(new NoOpClass()), Expression.Constant(new NoOpClass())));
        }

        [Test]
        public void Nullable()
        {
            int? a = 1;
            int? b = 2;

            var expr = Expression.AddChecked(Expression.Constant(a), Expression.Constant(b));
            Assert.AreEqual(ExpressionType.AddChecked, expr.NodeType, "AddChecked#04");
            Assert.AreEqual(typeof(int), expr.Type, "AddChecked#05");
            Assert.IsNull(expr.Method, null, "AddChecked#06");
            Assert.AreEqual("(1 + 2)", expr.ToString(), "AddChecked#16");
        }

        [Test]
        public void Numeric()
        {
            var expr = Expression.AddChecked(Expression.Constant(1), Expression.Constant(2));
            Assert.AreEqual(ExpressionType.AddChecked, expr.NodeType, "AddChecked#01");
            Assert.AreEqual(typeof(int), expr.Type, "AddChecked#02");
            Assert.IsNull(expr.Method, "AddChecked#03");
            Assert.AreEqual("(1 + 2)", expr.ToString(), "AddChecked#15");
        }

        //
        // These should not overflow
        //
        [Test]
        public void TestNoOverflow()
        {
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
            // These should overflow, check the various types and codepaths
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
            var mi = typeof(OpClass).GetMethod("op_Addition");

            var expr = Expression.AddChecked(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.AddChecked, expr.NodeType, "AddChecked#07");
            Assert.AreEqual(typeof(OpClass), expr.Type, "AddChecked#08");
            Assert.AreEqual(mi, expr.Method, "AddChecked#09");
            Assert.AreEqual("op_Addition", expr.Method.Name, "AddChecked#10");
            Assert.AreEqual
            (
                "(value(MonoTests.System.Linq.Expressions.OpClass) + value(MonoTests.System.Linq.Expressions.OpClass))",
                expr.ToString(), "AddChecked#17"
            );
        }

        [Test]
        public void UserDefinedStruct()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var mi = typeof(OpStruct).GetMethod("op_Addition");

            var expr = Expression.AddChecked(Expression.Constant(new OpStruct()), Expression.Constant(new OpStruct()));
            Assert.AreEqual(ExpressionType.AddChecked, expr.NodeType, "AddChecked#11");
            Assert.AreEqual(typeof(OpStruct), expr.Type, "AddChecked#12");
            Assert.AreEqual(mi, expr.Method, "AddChecked#13");
            Assert.AreEqual("op_Addition", expr.Method.Name, "AddChecked#14");
            Assert.AreEqual
            (
                "(value(MonoTests.System.Linq.Expressions.OpStruct) + value(MonoTests.System.Linq.Expressions.OpStruct))",
                expr.ToString(), "AddChecked#18"
            );
        }
    }
}