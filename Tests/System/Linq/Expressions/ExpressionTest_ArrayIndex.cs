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
using Tests.Helpers;

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

        [Test]
        public void Arg1NotArray()
        {
            const string Value = "This is not an array!";
            const int Index = 1;

            AssertEx.Throws<ArgumentException>(() => Expression.ArrayIndex(Expression.Constant(Value), Expression.Constant(Index)));
        }

        [Test]
        public void Arg1Null()
        {
            const int Index = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.ArrayIndex(null, Expression.Constant(Index)));
        }

        [Test]
        public void Arg2Null1()
        {
            const int Size = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.ArrayIndex(Expression.Constant(new int[Size]), (Expression)null));
        }

        [Test]
        public void Arg2Null2()
        {
            const int Size = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.ArrayIndex(Expression.Constant(new int[Size]), (IEnumerable<Expression>)null));
        }

        [Test]
        public void Arg2Null3()
        {
            const int Size = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.ArrayIndex(Expression.Constant(new int[Size]), (Expression[])null));
        }

        [Test]
        public void Arg2WrongNumber1()
        {
            const int Size = 1;
            const int IndexA = 1;
            const int IndexB = 1;

            Expression[] indexes = { Expression.Constant(IndexA), Expression.Constant(IndexB) };

            AssertEx.Throws<ArgumentException>(() => Expression.ArrayIndex(Expression.Constant(new int[Size]), indexes));
        }

        [Test]
        public void Arg2WrongType1()
        {
            const int size = 1;
            const bool Index = true;

            AssertEx.Throws<ArgumentException>(() => Expression.ArrayIndex(Expression.Constant(new int[size]), Expression.Constant(Index)));
        }

        [Test]
        public void Arg2WrongType2()
        {
            const int SizeA = 1;
            const int SizeB = 1;
            const int IndexA = 1;
            const long IndexB = 1L;

            Expression[] indexes = { Expression.Constant(IndexA), Expression.Constant(IndexB) };

            AssertEx.Throws<ArgumentException>(() => Expression.ArrayIndex(Expression.Constant(new int[SizeA, SizeB]), indexes));
        }

        [Test]
        public void CompileClassArrayAccess()
        {
            var array = new[] { new Foo(), new Foo(), new Foo(), new Foo() };
            var compiled = CreateArrayAccess<Foo>();

            Assert.AreEqual(array[0], compiled(array, 0));
            Assert.AreEqual(array[1], compiled(array, 1));
            Assert.AreEqual(array[2], compiled(array, 2));
            Assert.AreEqual(array[3], compiled(array, 3));
        }

        [Test]
        public void CompileEnumArrayAccess()
        {
            var array = new[] { Months.Jan, Months.Feb, Months.Mar, Months.Apr };
            var compiled = CreateArrayAccess<Months>();

            Assert.AreEqual(array[0], compiled(array, 0));
            Assert.AreEqual(array[1], compiled(array, 1));
            Assert.AreEqual(array[2], compiled(array, 2));
            Assert.AreEqual(array[3], compiled(array, 3));
        }

        [Test]
        public void CompileIntArrayAccess()
        {
            var array = new[] { 1, 2, 3, 4 };
            var compiled = CreateArrayAccess<int>();

            Assert.AreEqual(array[0], compiled(array, 0));
            Assert.AreEqual(array[1], compiled(array, 1));
            Assert.AreEqual(array[2], compiled(array, 2));
            Assert.AreEqual(array[3], compiled(array, 3));
        }

        [Test]
        public void CompileShortArrayAccess()
        {
            var array = new short[] { 1, 2, 3, 4 };
            var compiled = CreateArrayAccess<short>();

            Assert.AreEqual(array[0], compiled(array, 0));
            Assert.AreEqual(array[1], compiled(array, 1));
            Assert.AreEqual(array[2], compiled(array, 2));
            Assert.AreEqual(array[3], compiled(array, 3));
        }

        [Test]
        public void CompileStructArrayAccess()
        {
            var array = new[] { new Bar(0), new Bar(1), new Bar(2), new Bar(3) };
            var compiled = CreateArrayAccess<Bar>();

            Assert.AreEqual(array[0], compiled(array, 0));
            Assert.AreEqual(array[3], compiled(array, 3));
            Assert.AreEqual(array[1], compiled(array, 1));
            Assert.AreEqual(array[2], compiled(array, 2));
            Assert.AreEqual(array[0].Value, compiled(array, 0).Value);
            Assert.AreEqual(array[3].Value, compiled(array, 3).Value);
            Assert.AreEqual(array[1].Value, compiled(array, 1).Value);
            Assert.AreEqual(array[2].Value, compiled(array, 2).Value);
        }

        [Test]
        public void Rank1Struct()
        {
            const int Value = 42;
            const int Index = 0;
            var type = typeof(int);

            int[] array = { Value };

            var binaryExpression = Expression.ArrayIndex(Expression.Constant(array), Expression.Constant(Index));
            Assert.AreEqual(ExpressionType.ArrayIndex, binaryExpression.NodeType, "ArrayIndex#01");
            Assert.AreEqual(type, binaryExpression.Type, "ArrayIndex#02");
            Assert.IsNull(binaryExpression.Method, "ArrayIndex#03");
            Assert.AreEqual($"value({type.FullName}[])[{Index}]", binaryExpression.ToString(), "ArrayIndex#04");
        }

        [Test]
        public void Rank1UserDefinedClass()
        {
            var value = new NoOpClass();
            const int Index = 0;
            var type = typeof(NoOpClass);

            NoOpClass[] array = { value };

            var binaryExpression = Expression.ArrayIndex(Expression.Constant(array), Expression.Constant(Index));
            Assert.AreEqual(ExpressionType.ArrayIndex, binaryExpression.NodeType, "ArrayIndex#05");
            Assert.AreEqual(type, binaryExpression.Type, "ArrayIndex#06");
            Assert.IsNull(binaryExpression.Method, "ArrayIndex#07");
            Assert.AreEqual($"value({type.FullName}[])[{Index}]", binaryExpression.ToString(), "ArrayIndex#08");
        }

        [Test]
        public void Rank2Struct()
        {
            const int Value = 42;
            const int IndexA = 1;
            const int IndexB = 0;
            var type = typeof(int);

            int[,] array = { { Value }, { Value } };
            Expression[] indexes = { Expression.Constant(IndexA), Expression.Constant(IndexB) };

            var binaryExpression = Expression.ArrayIndex(Expression.Constant(array), indexes);
            Assert.AreEqual(ExpressionType.Call, binaryExpression.NodeType, "ArrayIndex#09");
            Assert.AreEqual(type, binaryExpression.Type, "ArrayIndex#10");
            Assert.AreEqual($"value({type.FullName}[,]).Get({IndexA}, {IndexB})", binaryExpression.ToString(), "ArrayIndex#12");
        }

        [Test]
        public void Rank2UserDefinedClass()
        {
            var ValueA = new NoOpClass();
            var ValueB = new NoOpClass();
            const int IndexA = 1;
            const int IndexB = 0;
            var type = typeof(NoOpClass);

            NoOpClass[,] array = { { ValueA }, { ValueB } };
            Expression[] indexes = { Expression.Constant(IndexA), Expression.Constant(IndexB) };

            var binaryExpression = Expression.ArrayIndex(Expression.Constant(array), indexes);
            Assert.AreEqual(ExpressionType.Call, binaryExpression.NodeType, "ArrayIndex#13");
            Assert.AreEqual(type, binaryExpression.Type, "ArrayIndex#14");
            Assert.AreEqual($"value({type.FullName}[,]).Get(1, 0)", binaryExpression.ToString(), "ArrayIndex#16");
        }

        private static Func<T[], int, T> CreateArrayAccess<T>()
        {
            const string NameArray = "a";
            const string NameIndex = "b";

            var parameterArray = Expression.Parameter(typeof(T[]), NameArray);
            var parameterIndex = Expression.Parameter(typeof(int), NameIndex);

            return Expression.Lambda<Func<T[], int, T>>(Expression.ArrayIndex(parameterArray, parameterIndex), parameterArray, parameterIndex).Compile();
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
    }
}