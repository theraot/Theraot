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

using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestMultiply
    {
        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => { Expression.Multiply(null, Expression.Constant(1)); });
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => { Expression.Multiply(Expression.Constant(1), null); });
        }

        [Test]
        public void ArgTypesDifferent()
        {
            Assert.Throws<InvalidOperationException>(() => { Expression.Multiply(Expression.Constant(1), Expression.Constant(2.0)); });
        }

        [Test]
        public void NoOperatorClass()
        {
            Assert.Throws<InvalidOperationException>(() => { Expression.Multiply(Expression.Constant(new NoOpClass()), Expression.Constant(new NoOpClass())); });
        }

        [Test]
        public void Boolean()
        {
            Assert.Throws<InvalidOperationException>(() => { Expression.Multiply(Expression.Constant(true), Expression.Constant(false)); });
        }

        [Test]
        public void Numeric()
        {
            var expr = Expression.Multiply(Expression.Constant(1), Expression.Constant(2));
            Assert.AreEqual(ExpressionType.Multiply, expr.NodeType, "Multiply#01");
            Assert.AreEqual(typeof(int), expr.Type, "Multiply#02");
            Assert.IsNull(expr.Method, "Multiply#03");
            Assert.AreEqual("(1 * 2)", expr.ToString(), "Multiply#04");
        }

        [Test]
        public void Nullable()
        {
            int? a = 1;
            int? b = 2;

            var expr = Expression.Multiply(Expression.Constant(a), Expression.Constant(b));
            Assert.AreEqual(ExpressionType.Multiply, expr.NodeType, "Multiply#05");
            Assert.AreEqual(typeof(int), expr.Type, "Multiply#06");
            Assert.IsNull(expr.Method, "Multiply#07");
            Assert.AreEqual("(1 * 2)", expr.ToString(), "Multiply#08");
        }

        [Test]
        public void UserDefinedClass()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var mi = typeof(OpClass).GetMethod("op_Multiply");

            var expr = Expression.Multiply(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.Multiply, expr.NodeType, "Multiply#09");
            Assert.AreEqual(typeof(OpClass), expr.Type, "Multiply#10");
            Assert.AreEqual(mi, expr.Method, "Multiply#11");
            Assert.AreEqual("op_Multiply", expr.Method.Name, "Multiply#12");
            Assert.AreEqual("(value(MonoTests.System.Linq.Expressions.OpClass) * value(MonoTests.System.Linq.Expressions.OpClass))",
                expr.ToString(), "Multiply#13");
        }

        [Test]
        public void Compile()
        {
            var left = Expression.Parameter(typeof(int), "l");
            var right = Expression.Parameter(typeof(int), "r");
            var l = Expression.Lambda<Func<int, int, int>>(
                Expression.Multiply(left, right), left, right);

            var be = l.Body as BinaryExpression;
            Assert.IsNotNull(be);
            Assert.AreEqual(typeof(int), be.Type);
            Assert.IsFalse(be.IsLifted);
            Assert.IsFalse(be.IsLiftedToNull);

            var c = l.Compile();

            Assert.AreEqual(36, c(6, 6));
            Assert.AreEqual(-1, c(-1, 1));
            Assert.AreEqual(-3, c(1, -3));
        }
    }
}