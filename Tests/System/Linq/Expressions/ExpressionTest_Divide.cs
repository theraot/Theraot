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

#if TARGETS_NETCORE || TARGETS_NETSTANDARD
using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestDivide
    {
        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Divide(null, Expression.Constant(1)));
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Divide(Expression.Constant(1), null));
        }

        [Test]
        public void ArgTypesDifferent()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Divide(Expression.Constant(1), Expression.Constant(2.0)));
        }

        [Test]
        public void Boolean()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Divide(Expression.Constant(true), Expression.Constant(false)));
        }

        [Test]
        public void NoOperatorClass()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Divide(Expression.Constant(new NoOpClass()), Expression.Constant(new NoOpClass())));
        }

        [Test]
        public void Nullable()
        {
            int? a = 1;
            int? b = 2;

            var expr = Expression.Divide(Expression.Constant(a), Expression.Constant(b));
            Assert.AreEqual(ExpressionType.Divide, expr.NodeType, "Divide#05");
            Assert.AreEqual(typeof(int), expr.Type, "Divide#06");
            Assert.IsNull(expr.Method, "Divide#07");
            Assert.AreEqual("(1 / 2)", expr.ToString(), "Divide#08");
        }

        [Test]
        public void Numeric()
        {
            var expr = Expression.Divide(Expression.Constant(1), Expression.Constant(2));
            Assert.AreEqual(ExpressionType.Divide, expr.NodeType, "Divide#01");
            Assert.AreEqual(typeof(int), expr.Type, "Divide#02");
            Assert.IsNull(expr.Method, "Divide#03");
            Assert.AreEqual("(1 / 2)", expr.ToString(), "Divide#04");
        }

        [Test]
        public void UserDefinedClass()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var mi = typeof(OpClass).GetMethod("op_Division");

            var expr = Expression.Divide(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.Divide, expr.NodeType, "Divide#09");
            Assert.AreEqual(typeof(OpClass), expr.Type, "Divide#10");
            Assert.AreEqual(mi, expr.Method, "Divide#11");
            Assert.AreEqual("op_Division", expr.Method.Name, "Divide#12");
            Assert.AreEqual
            (
                "(value(MonoTests.System.Linq.Expressions.OpClass) / value(MonoTests.System.Linq.Expressions.OpClass))",
                expr.ToString(), "Divide#13"
            );
        }
    }
}