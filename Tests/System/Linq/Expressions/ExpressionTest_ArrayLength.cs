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
using System.Linq.Expressions;
using NUnit.Framework;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestArrayLength
    {
        [Test]
        public void Arg1NotArray()
        {
            const string Value = "This is not an array!";

            Assert.Throws<ArgumentException>(() => Expression.ArrayLength(Expression.Constant(Value)));
        }

        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ArrayLength(null));
        }

        [Test]
        public void CompileObjectArrayLengthA()
        {
            const int Size = 0;
            const string Name = "ary";

            var parameter = Expression.Parameter(typeof(object[]), Name);
            var compiled = Expression.Lambda<Func<object[], int>>
            (
                Expression.ArrayLength(parameter),
                parameter
            ).Compile();

            Assert.AreEqual(Size, compiled(new object[Size]));
        }

        [Test]
        public void CompileObjectArrayLengthB()
        {
            const string ValueA = "jb";
            const string ValueB = "evain";
            const string Name = "ary";

            var parameter = Expression.Parameter(typeof(object[]), Name);
            var compiled = Expression.Lambda<Func<object[], int>>
            (
                Expression.ArrayLength(parameter),
                parameter
            ).Compile();

            Assert.AreEqual(2, compiled(new object[] {ValueA, ValueB}));
        }

        [Test]
        public void CompileStringArrayLengthA()
        {
            const int Size = 0;
            const string Name = "ary";

            var parameter = Expression.Parameter(typeof(string[]), Name);
            var compiled = Expression.Lambda<Func<string[], int>>
            (
                Expression.ArrayLength(parameter),
                parameter
            ).Compile();

            Assert.AreEqual(Size, compiled(new string[Size]));
        }

        [Test]
        public void CompileStringArrayLengthB()
        {
            const string ValueA = "jb";
            const string ValueB = "evain";
            const string Name = "ary";

            var parameter = Expression.Parameter(typeof(string[]), Name);
            var compiled = Expression.Lambda<Func<string[], int>>
            (
                Expression.ArrayLength(parameter),
                parameter
            ).Compile();

            Assert.AreEqual(2, compiled(new[] {ValueA, ValueB}));
        }

        [Test]
        public void Rank1String()
        {
            string[] array = {"a", "b", "c"};

            var unaryExpression = Expression.ArrayLength(Expression.Constant(array));
            Assert.AreEqual(ExpressionType.ArrayLength, unaryExpression.NodeType, "ArrayLength#01");
            Assert.AreEqual(typeof(int), unaryExpression.Type, "ArrayLength#02");
            Assert.IsNull(unaryExpression.Method, "ArrayLength#03");
            Assert.AreEqual($"ArrayLength(value({typeof(string).FullName}[]))", unaryExpression.ToString(), "ArrayLength#04");
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