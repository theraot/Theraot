//
// ExpressionTest_TypeAs.cs
//
// Author:
//   Federico Di Gregorio <fog@initd.org>
//   Jb Evain (jbevain@novell.com)
//
// (C) 2008 Novell, Inc. (http://www.novell.com)
//
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
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestTypeAs
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Arg1Null()
        {
            Expression.TypeAs(null, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Arg2Null()
        {
            Expression.TypeAs(Expression.Constant(1), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Arg2NotReferenceNorNullable()
        {
            Expression.TypeAs(Expression.Constant(1), typeof(int));
        }

        [Test]
        public void NullableNumeric()
        {
            var expr = Expression.TypeAs(Expression.Constant(1), typeof(int?));
            Assert.AreEqual(ExpressionType.TypeAs, expr.NodeType, "TypeAs#01");
            Assert.AreEqual(typeof(int?), expr.Type, "TypeAs#02");
            Assert.AreEqual("(1 As Nullable`1)", expr.ToString(), "TypeAs#03");
        }

        [Test]
        public void String()
        {
            var expr = Expression.TypeAs(Expression.Constant(1), typeof(string));
            Assert.AreEqual(ExpressionType.TypeAs, expr.NodeType, "TypeAs#04");
            Assert.AreEqual(typeof(string), expr.Type, "TypeAs#05");
            Assert.AreEqual("(1 As String)", expr.ToString(), "TypeAs#06");
        }

        [Test]
        public void UserDefinedClass()
        {
            var expr = Expression.TypeAs(Expression.Constant(new OpClass()), typeof(OpClass));
            Assert.AreEqual(ExpressionType.TypeAs, expr.NodeType, "TypeAs#07");
            Assert.AreEqual(typeof(OpClass), expr.Type, "TypeAs#08");
            Assert.AreEqual("(value(MonoTests.System.Linq.Expressions.OpClass) As OpClass)", expr.ToString(), "TypeAs#09");
        }

        private static Func<object, TType> CreateTypeAs<TType>()
        {
            var obj = Expression.Parameter(typeof(object), "obj");

            return Expression.Lambda<Func<object, TType>>(
                Expression.TypeAs(obj, typeof(TType)), obj).Compile();
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

        [Test]
        public void CompiledTypeAs()
        {
            var asbar = CreateTypeAs<Bar>();
            var asbaz = CreateTypeAs<Baz>();

            Assert.IsNotNull(asbar(new Bar()));
            Assert.IsNull(asbar(new Foo()));
            Assert.IsNotNull(asbar(new Baz()));
            Assert.IsNull(asbaz(new Bar()));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TypeAsVoid()
        {
            Expression.TypeAs("yoyo".ToConstant(), typeof(void));
        }
    }
}