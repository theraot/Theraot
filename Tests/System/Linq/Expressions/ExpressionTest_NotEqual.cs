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
//	Miguel de Icaza (miguel@novell.com)
//	Jb Evain (jbevain@novell.com)
//

using System;
using System.Linq.Expressions;
using NUnit.Framework;

#if TARGETS_NETCORE || TARGETS_NETSTANDARD
using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestNotEqual
    {
        public struct D
        {
            // Empty
        }

        public enum Foo
        {
            Bar,
            Baz
        }

        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.NotEqual(null, Expression.Constant(1)));
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.NotEqual(Expression.Constant(1), null));
        }

        [Test]
        public void ArgTypesDifferent()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.NotEqual(Expression.Constant(1), Expression.Constant(2.0)));
        }

        [Test]
        public void EnumNotEqual()
        {
            var l = Expression.Parameter(typeof(Foo), "l");
            var r = Expression.Parameter(typeof(Foo), "r");

            var node = Expression.NotEqual(l, r);
            Assert.IsFalse(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNull(node.Method);

            var compiled = Expression.Lambda<Func<Foo, Foo, bool>>(node, l, r).Compile();

            Assert.AreEqual(false, compiled(Foo.Bar, Foo.Bar));
            Assert.AreEqual(true, compiled(Foo.Bar, Foo.Baz));
        }

        [Test]
        public void LiftedEnumNotEqual()
        {
            var l = Expression.Parameter(typeof(Foo?), "l");
            var r = Expression.Parameter(typeof(Foo?), "r");

            var node = Expression.NotEqual(l, r);
            Assert.IsTrue(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNull(node.Method);

            var compiled = Expression.Lambda<Func<Foo?, Foo?, bool>>(node, l, r).Compile();

            Assert.AreEqual(false, compiled(Foo.Bar, Foo.Bar));
            Assert.AreEqual(true, compiled(Foo.Bar, Foo.Baz));
            Assert.AreEqual(true, compiled(Foo.Bar, null));
            Assert.AreEqual(false, compiled(null, null));
        }

        [Test]
        public void NoOperatorClass()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.NotEqual(Expression.Constant(new D()), Expression.Constant(new D())));
        }

        [Test]
        public void Nullable_LiftToNull_SetToFalse()
        {
            int? a = 1;
            int? b = 2;

            var expr = Expression.NotEqual
            (
                Expression.Constant(a, typeof(int?)),
                Expression.Constant(b, typeof(int?)),
                false, null
            );
            Assert.AreEqual(ExpressionType.NotEqual, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.AreEqual(true, expr.IsLifted);
            Assert.AreEqual(false, expr.IsLiftedToNull);
            Assert.IsNull(expr.Method);
            Assert.AreEqual("(1 != 2)", expr.ToString());
        }

        [Test]
        public void Nullable_LiftToNull_SetToTrue()
        {
            int? a = 1;
            int? b = 2;

            var expr = Expression.NotEqual
            (
                Expression.Constant(a, typeof(int?)),
                Expression.Constant(b, typeof(int?)),
                true, null
            );
            Assert.AreEqual(ExpressionType.NotEqual, expr.NodeType);
            Assert.AreEqual(typeof(bool?), expr.Type);
            Assert.AreEqual(true, expr.IsLifted);
            Assert.AreEqual(true, expr.IsLiftedToNull);
            Assert.IsNull(expr.Method);
            Assert.AreEqual("(1 != 2)", expr.ToString());
        }

        [Test]
        public void Nullable_Mixed()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    int? a = 1;
                    const int B = 2;

                    Expression.NotEqual
                    (
                        Expression.Constant(a, typeof(int?)),
                        Expression.Constant(B, typeof(int))
                    );
                }
            );
        }

        [Test]
        public void NullableInt32NotEqual()
        {
            var l = Expression.Parameter(typeof(int?), "l");
            var r = Expression.Parameter(typeof(int?), "r");

            var compiled = Expression.Lambda<Func<int?, int?, bool>>
            (
                Expression.NotEqual(l, r), l, r
            ).Compile();

            Assert.IsFalse(compiled(null, null));
            Assert.IsTrue(compiled(null, 1));
            Assert.IsTrue(compiled(1, null));
            Assert.IsTrue(compiled(1, 2));
            Assert.IsFalse(compiled(1, 1));
            Assert.IsTrue(compiled(null, 0));
            Assert.IsTrue(compiled(0, null));
        }

        [Test]
        public void NullableInt32NotEqualLiftedToNull()
        {
            var l = Expression.Parameter(typeof(int?), "l");
            var r = Expression.Parameter(typeof(int?), "r");

            var compiled = Expression.Lambda<Func<int?, int?, bool?>>
            (
                Expression.NotEqual(l, r, true, null), l, r
            ).Compile();

            Assert.AreEqual(null, compiled(null, null));
            Assert.AreEqual(null, compiled(null, 1));
            Assert.AreEqual(null, compiled(1, null));
            Assert.AreEqual((bool?)true, compiled(1, 2));
            Assert.AreEqual((bool?)false, compiled(1, 1));
            Assert.AreEqual(null, compiled(null, 0));
            Assert.AreEqual(null, compiled(0, null));
        }

        [Test]
        public void Numeric()
        {
            var expr = Expression.NotEqual(Expression.Constant(1), Expression.Constant(2));
            Assert.AreEqual(ExpressionType.NotEqual, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.IsNull(expr.Method);
            Assert.AreEqual("(1 != 2)", expr.ToString());
        }

        [Test]
        public void ReferenceCompare()
        {
            Expression.NotEqual(Expression.Constant(new NoOpClass()), Expression.Constant(new NoOpClass()));
        }

        [Test]
        public void UserDefinedClass()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var method = typeof(OpClass).GetMethod("op_Inequality");

            var expr = Expression.NotEqual(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.NotEqual, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.AreEqual(method, expr.Method);

            Assert.AreEqual("(value(MonoTests.System.Linq.Expressions.OpClass) != value(MonoTests.System.Linq.Expressions.OpClass))", expr.ToString());
        }
    }
}