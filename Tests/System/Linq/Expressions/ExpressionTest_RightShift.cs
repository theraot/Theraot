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
//      Jb Evain <jbevain@novell.com>

using System;
using System.Linq.Expressions;
using NUnit.Framework;

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestRightShift
    {
        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.RightShift(null, Expression.Constant(1)));
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.RightShift(Expression.Constant(1), null));
        }

        [Test]
        public void Arg2WrongType()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.RightShift(Expression.Constant(1), Expression.Constant(2.0)));
        }

        [Test]
        public void Boolean()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.RightShift(Expression.Constant(true), Expression.Constant(1)));
        }

        [Test]
        public void CompileRightShift()
        {
            var l = Expression.Parameter(typeof(int), "l");
            var r = Expression.Parameter(typeof(int), "r");

            var compiled = Expression.Lambda<Func<int, int, int>>
            (
                Expression.RightShift(l, r), l, r
            ).Compile();

            Assert.AreEqual(3, compiled(6, 1));
            Assert.AreEqual(1, compiled(12, 3));
        }

        [Test]
        public void Double()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.RightShift(Expression.Constant(2.0), Expression.Constant(1)));
        }

        [Test]
        public void Integer()
        {
            var expr = Expression.RightShift(Expression.Constant(2), Expression.Constant(1));
            Assert.AreEqual(ExpressionType.RightShift, expr.NodeType, "RightShift#01");
            Assert.AreEqual(typeof(int), expr.Type, "RightShift#02");
            Assert.IsNull(expr.Method, "RightShift#03");
            Assert.AreEqual("(2 >> 1)", expr.ToString(), "RightShift#04");
        }

        [Test]
        public void NoOperatorClass()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.RightShift(Expression.Constant(new NoOpClass()), Expression.Constant(1)));
        }

        [Test]
        public void Nullable()
        {
            int? a = 1;
            int? b = 2;

            var expr = Expression.RightShift(Expression.Constant(a), Expression.Constant(b));
            Assert.AreEqual(ExpressionType.RightShift, expr.NodeType, "RightShift#05");
            Assert.AreEqual(typeof(int), expr.Type, "RightShift#06");
            Assert.IsNull(expr.Method, "RightShift#07");
            Assert.AreEqual("(1 >> 2)", expr.ToString(), "RightShift#08");
        }

        [Test]
        public void RightShiftNullableLongAndInt()
        {
            var l = Expression.Parameter(typeof(long?), "l");
            var r = Expression.Parameter(typeof(int), "r");

            var node = Expression.RightShift(l, r);
            Assert.IsTrue(node.IsLifted);
            Assert.IsTrue(node.IsLiftedToNull);
            Assert.AreEqual(typeof(long?), node.Type);

            var compiled = Expression.Lambda<Func<long?, int, long?>>(node, l, r).Compile();

            Assert.AreEqual(null, compiled(null, 2));
            Assert.AreEqual(512, compiled(1024, 1));
        }

        [Test]
        public void UserDefinedClass()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var method = typeof(OpClass).GetMethod("op_RightShift");

            var expr = Expression.RightShift(Expression.Constant(new OpClass()), Expression.Constant(1));
            Assert.AreEqual(ExpressionType.RightShift, expr.NodeType, "RightShift#09");
            Assert.AreEqual(typeof(OpClass), expr.Type, "RightShift#10");
            Assert.AreEqual(method, expr.Method, "RightShift#11");
            Assert.AreEqual
            (
                "(value(MonoTests.System.Linq.Expressions.OpClass) >> 1)",
                expr.ToString(), "RightShift#13"
            );
        }
    }
}