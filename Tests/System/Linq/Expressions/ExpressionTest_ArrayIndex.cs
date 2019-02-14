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
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestArrayIndex
    {
        private enum Months
        {
            Jan,
            Feb,
            Mar,
            Apr
        }

        private static Func<T[], int, T> CreateArrayAccess<T>()
        {
            var a = Expression.Parameter(typeof(T[]), "a");
            var i = Expression.Parameter(typeof(int), "i");

            return Expression.Lambda<Func<T[], int, T>>(Expression.ArrayIndex(a, i), a, i).Compile();
        }

        private struct Bar
        {
            public readonly int Value;

            public Bar(int value)
            {
                Value = value;
            }
        }

        private class Foo
        {
            // Empty
        }

        [Test]
        public void Arg1NotArray()
        {
            Assert.Throws<ArgumentException>(() => Expression.ArrayIndex(Expression.Constant("This is not an array!"), Expression.Constant(1)));
        }

        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ArrayIndex(null, Expression.Constant(1)));
        }

        [Test]
        public void Arg2Null1()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ArrayIndex(Expression.Constant(new int[1]), (Expression)null));
        }

        [Test]
        public void Arg2Null2()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ArrayIndex(Expression.Constant(new int[1]), (IEnumerable<Expression>)null));
        }

        [Test]
        public void Arg2Null3()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ArrayIndex(Expression.Constant(new int[1]), (Expression[])null));
        }

        [Test]
        public void Arg2WrongNumber1()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    Expression[] indexes = {Expression.Constant(1), Expression.Constant(0)};

                    Expression.ArrayIndex(Expression.Constant(new int[1]), indexes);
                }
            );
        }

        [Test]
        public void Arg2WrongType1()
        {
            Assert.Throws<ArgumentException>(() => Expression.ArrayIndex(Expression.Constant(new int[1]), Expression.Constant(true)));
        }

        [Test]
        public void Arg2WrongType2()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    Expression[] indexes = {Expression.Constant(1), Expression.Constant(1L)};

                    Expression.ArrayIndex(Expression.Constant(new int[1, 1]), indexes);
                }
            );
        }

        [Test]
        public void CompileClassArrayAccess()
        {
            var array = new[] {new Foo(), new Foo(), new Foo(), new Foo()};
            var at = CreateArrayAccess<Foo>();

            Assert.AreEqual(array[0], at(array, 0));
            Assert.AreEqual(array[3], at(array, 3));
        }

        [Test]
        public void CompileEnumArrayAccess()
        {
            var array = new[] {Months.Jan, Months.Feb, Months.Mar, Months.Apr};
            var at = CreateArrayAccess<Months>();

            Assert.AreEqual(array[0], at(array, 0));
            Assert.AreEqual(array[3], at(array, 3));
        }

        [Test]
        public void CompileIntArrayAccess()
        {
            var array = new[] {1, 2, 3, 4};
            var at = CreateArrayAccess<int>();

            Assert.AreEqual(1, at(array, 0));
            Assert.AreEqual(4, at(array, 3));
        }

        [Test]
        public void CompileShortArrayAccess()
        {
            var array = new short[] {1, 2, 3, 4};
            var at = CreateArrayAccess<short>();

            Assert.AreEqual(array[0], at(array, 0));
            Assert.AreEqual(array[3], at(array, 3));
        }

        [Test]
        public void CompileStructArrayAccess()
        {
            var array = new[] {new Bar(0), new Bar(1), new Bar(2), new Bar(3)};
            var at = CreateArrayAccess<Bar>();

            Assert.AreEqual(array[0], at(array, 0));
            Assert.AreEqual(array[3], at(array, 3));
            Assert.AreEqual(array[1], at(array, 1));
            Assert.AreEqual(array[2], at(array, 2));
            Assert.AreEqual(0, at(array, 0).Value);
            Assert.AreEqual(3, at(array, 3).Value);
            Assert.AreEqual(1, at(array, 1).Value);
            Assert.AreEqual(2, at(array, 2).Value);
        }

        [Test]
        public void Rank1Struct()
        {
            int[] array = {42};

            var expr = Expression.ArrayIndex(Expression.Constant(array), Expression.Constant(0));
            Assert.AreEqual(ExpressionType.ArrayIndex, expr.NodeType, "ArrayIndex#01");
            Assert.AreEqual(typeof(int), expr.Type, "ArrayIndex#02");
            Assert.IsNull(expr.Method, "ArrayIndex#03");
            Assert.AreEqual("value(System.Int32[])[0]", expr.ToString(), "ArrayIndex#04");
        }

        [Test]
        public void Rank1UserDefinedClass()
        {
            NoOpClass[] array = {new NoOpClass()};

            var expr = Expression.ArrayIndex(Expression.Constant(array), Expression.Constant(0));
            Assert.AreEqual(ExpressionType.ArrayIndex, expr.NodeType, "ArrayIndex#05");
            Assert.AreEqual(typeof(NoOpClass), expr.Type, "ArrayIndex#06");
            Assert.IsNull(expr.Method, "ArrayIndex#07");
            Assert.AreEqual($"value({typeof(NoOpClass).FullName}[])[0]", expr.ToString(), "ArrayIndex#08");
        }

        [Test]
        public void Rank2Struct()
        {
            int[,] array = {{42}, {42}};
            Expression[] indexes = {Expression.Constant(1), Expression.Constant(0)};

            var expr = Expression.ArrayIndex(Expression.Constant(array), indexes);
            Assert.AreEqual(ExpressionType.Call, expr.NodeType, "ArrayIndex#09");
            Assert.AreEqual(typeof(int), expr.Type, "ArrayIndex#10");
            Assert.AreEqual("value(System.Int32[,]).Get(1, 0)", expr.ToString(), "ArrayIndex#12");
        }

        [Test]
        public void Rank2UserDefinedClass()
        {
            NoOpClass[,] array = {{new NoOpClass()}, {new NoOpClass()}};
            Expression[] indexes = {Expression.Constant(1), Expression.Constant(0)};

            var expr = Expression.ArrayIndex(Expression.Constant(array), indexes);
            Assert.AreEqual(ExpressionType.Call, expr.NodeType, "ArrayIndex#13");
            Assert.AreEqual(typeof(NoOpClass), expr.Type, "ArrayIndex#14");
            Assert.AreEqual($"value({typeof(NoOpClass).FullName}[,]).Get(1, 0)", expr.ToString(), "ArrayIndex#16");
        }
    }
}