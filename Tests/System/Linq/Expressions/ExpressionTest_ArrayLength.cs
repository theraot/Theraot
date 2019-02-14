#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_ArrayLength.cs
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

using System;
using NUnit.Framework;
using System.Linq.Expressions;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestArrayLength
    {
        [Test]
        public void Arg1NotArray()
        {
            Assert.Throws<ArgumentException>(() => Expression.ArrayLength(Expression.Constant("This is not an array!")));
        }

        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ArrayLength(null));
        }

        [Test]
        public void CompileObjectArrayLength()
        {
            var p = Expression.Parameter(typeof(object[]), "ary");
            var compiled = Expression.Lambda<Func<object[], int>>
            (
                Expression.ArrayLength(p),
                p
            ).Compile();

            Assert.AreEqual(0, compiled(new object[0]));
            Assert.AreEqual(2, compiled(new object[] {"jb", "evain"}));
        }

        [Test]
        public void CompileStringArrayLength()
        {
            var p = Expression.Parameter(typeof(string[]), "ary");
            var compiled = Expression.Lambda<Func<string[], int>>
            (
                Expression.ArrayLength(p),
                p
            ).Compile();

            Assert.AreEqual(0, compiled(new string[0]));
            Assert.AreEqual(2, compiled(new[] {"jb", "evain"}));
        }

        [Test]
        public void Rank1String()
        {
            string[] array = {"a", "b", "c"};

            var expr = Expression.ArrayLength(Expression.Constant(array));
            Assert.AreEqual(ExpressionType.ArrayLength, expr.NodeType, "ArrayLength#01");
            Assert.AreEqual(typeof(int), expr.Type, "ArrayLength#02");
            Assert.IsNull(expr.Method, "ArrayLength#03");
            Assert.AreEqual("ArrayLength(value(System.String[]))", expr.ToString(), "ArrayLength#04");
        }

        [Test]
        public void Rank2String()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    string[,] array = {{ }, { }};

                    Expression.ArrayLength(Expression.Constant(array));
                }
            );
        }
    }
}