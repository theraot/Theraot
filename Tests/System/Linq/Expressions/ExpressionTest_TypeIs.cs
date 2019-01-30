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
using System.Reflection;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestTypeIs
    {
        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => { Expression.TypeIs(null, typeof(int)); });
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => { Expression.TypeIs(Expression.Constant(1), null); });
        }

        [Test]
        public void Numeric()
        {
            var expr = Expression.TypeIs(Expression.Constant(1), typeof(int));
            Assert.AreEqual(ExpressionType.TypeIs, expr.NodeType, "TypeIs#01");
            Assert.AreEqual(typeof(bool), expr.Type, "TypeIs#02");
            Assert.AreEqual("(1 Is Int32)", expr.ToString(), "TypeIs#03");
        }

        [Test]
        public void String()
        {
            var expr = Expression.TypeIs(Expression.Constant(1), typeof(string));
            Assert.AreEqual(ExpressionType.TypeIs, expr.NodeType, "TypeIs#04");
            Assert.AreEqual(typeof(bool), expr.Type, "TypeIs#05");
            Assert.AreEqual("(1 Is String)", expr.ToString(), "TypeIs#06");
        }

        [Test]
        public void UserDefinedClass()
        {
            var expr = Expression.TypeIs(Expression.Constant(new OpClass()), typeof(OpClass));
            Assert.AreEqual(ExpressionType.TypeIs, expr.NodeType, "TypeIs#07");
            Assert.AreEqual(typeof(bool), expr.Type, "TypeIs#08");
            Assert.AreEqual("(value(MonoTests.System.Linq.Expressions.OpClass) Is OpClass)", expr.ToString(), "TypeIs#09");
        }

        private struct Foo
        {
            // Empty
        }

        private class Bar
        {
            // Empty
        }

        private class Baz : Bar
        {
            // Empty
        }

        private static Func<TType, bool> CreateTypeIs<TType, TCandidate>()
        {
            var p = Expression.Parameter(typeof(TType), "p");

            return Expression.Lambda<Func<TType, bool>>(
                Expression.TypeIs(p, typeof(TCandidate)), p).Compile();
        }

        [Test]
        public void CompiledTypeIs()
        {
            var fooIsBar = CreateTypeIs<Foo, Bar>();
            var fooIsFoo = CreateTypeIs<Foo, Foo>();
            var barIsBar = CreateTypeIs<Bar, Bar>();
            var barIsFoo = CreateTypeIs<Bar, Foo>();
            var bazIsBar = CreateTypeIs<Baz, Bar>();

            Assert.IsTrue(fooIsFoo(new Foo()));
            Assert.IsFalse(fooIsBar(new Foo()));
            Assert.IsTrue(barIsBar(new Bar()));
            Assert.IsFalse(barIsFoo(new Bar()));
            Assert.IsTrue(bazIsBar(new Baz()));
        }

        public static void TacTac()
        {
            // Empty
        }

        [Test]
        public void VoidIsObject()
        {
            var vio = Expression.Lambda<Func<bool>>(
                Expression.TypeIs(
                    Expression.Call(GetType().GetMethod("TacTac")),
                    typeof(object))).Compile();

            Assert.IsFalse(vio());
        }
    }
}