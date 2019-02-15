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
//		Jb Evain <jbevain@novell.com>

using System;
using System.Linq.Expressions;
using NUnit.Framework;

#if TARGETS_NETCORE || TARGETS_NETSTANDARD
using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestAnd
    {
        [Test]
        public void AndBoolItem()
        {
            const string Name = "i";
            const string NameLeft = nameof(Item<bool>.Left);
            const string NameRight = nameof(Item<bool>.Right);

            var parameter = Expression.Parameter(typeof(Item<bool>), Name);
            var compiled = Expression.Lambda<Func<Item<bool>, bool>>
            (
                Expression.And
                (
                    Expression.Property(parameter, NameLeft),
                    Expression.Property(parameter, NameRight)
                ),
                parameter
            ).Compile();

            var itemA = new Item<bool>(true, true);
            Assert.AreEqual(true, compiled(itemA));
            Assert.IsTrue(itemA.LeftCalled);
            Assert.IsTrue(itemA.RightCalled);

            var itemB = new Item<bool>(true, false);
            Assert.AreEqual(false, compiled(itemB));
            Assert.IsTrue(itemB.LeftCalled);
            Assert.IsTrue(itemB.RightCalled);

            var itemC = new Item<bool>(false, true);
            Assert.AreEqual(false, compiled(itemC));
            Assert.IsTrue(itemC.LeftCalled);
            Assert.IsTrue(itemC.RightCalled);

            var itemD = new Item<bool>(false, false);
            Assert.AreEqual(false, compiled(itemD));
            Assert.IsTrue(itemD.LeftCalled);
            Assert.IsTrue(itemD.RightCalled);
        }

        [Test]
        public void AndBoolNullableTest()
        {
            const string NameLeft = "a";
            const string NameRight = "b";

            var parameterLeft = Expression.Parameter(typeof(bool?), NameLeft);
            var parameterRight = Expression.Parameter(typeof(bool?), NameRight);
            var lambda = Expression.Lambda<Func<bool?, bool?, bool?>>(Expression.And(parameterLeft, parameterRight), parameterLeft, parameterRight);

            var binaryExpression = lambda.Body as BinaryExpression;
            Assert.IsNotNull(binaryExpression);
            Assert.AreEqual(typeof(bool?), binaryExpression.Type);
            Assert.IsTrue(binaryExpression.IsLifted);
            Assert.IsTrue(binaryExpression.IsLiftedToNull);

            var compiled = lambda.Compile();

            Assert.AreEqual(true, compiled(true, true), "a1");
            Assert.AreEqual(false, compiled(true, false), "a2");
            Assert.AreEqual(false, compiled(false, true), "a3");
            Assert.AreEqual(false, compiled(false, false), "a4");

            Assert.AreEqual(null, compiled(true, null), "a5");
            Assert.AreEqual(false, compiled(false, null), "a6");
            Assert.AreEqual(false, compiled(null, false), "a7");
            Assert.AreEqual(null, compiled(null, true), "a8");
            Assert.AreEqual(null, compiled(null, null), "a9");
        }

        [Test]
        public void AndBoolTest()
        {
            const string NameLeft = "a";
            const string NameRight = "b";

            var parameterLeft = Expression.Parameter(typeof(bool), NameLeft);
            var parameterRight = Expression.Parameter(typeof(bool), NameRight);
            var lambda = Expression.Lambda<Func<bool, bool, bool>>(Expression.And(parameterLeft, parameterRight), parameterLeft, parameterRight);

            var binaryExpression = lambda.Body as BinaryExpression;
            Assert.IsNotNull(binaryExpression);
            Assert.AreEqual(typeof(bool), binaryExpression.Type);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);

            var compiled = lambda.Compile();

            Assert.AreEqual(true, compiled(true, true), "t1");
            Assert.AreEqual(false, compiled(true, false), "t2");
            Assert.AreEqual(false, compiled(false, true), "t3");
            Assert.AreEqual(false, compiled(false, false), "t4");
        }

        [Test]
        public void AndIntNullableTest()
        {
            const string NameLeft = "a";
            const string NameRight = "b";

            var parameterLeft = Expression.Parameter(typeof(int?), NameLeft);
            var parameterRight = Expression.Parameter(typeof(int?), NameRight);
            var compiled = Expression.Lambda<Func<int?, int?, int?>>(Expression.And(parameterLeft, parameterRight), parameterLeft, parameterRight).Compile();

            Assert.AreEqual((int?)1, compiled(1, 1), "a1");
            Assert.AreEqual((int?)0, compiled(1, 0), "a2");
            Assert.AreEqual((int?)0, compiled(0, 1), "a3");
            Assert.AreEqual((int?)0, compiled(0, 0), "a4");

            Assert.AreEqual(null, compiled(1, null), "a5");
            Assert.AreEqual(null, compiled(0, null), "a6");
            Assert.AreEqual(null, compiled(null, 0), "a7");
            Assert.AreEqual(null, compiled(1, null), "a8");
            Assert.AreEqual(null, compiled(null, null), "a9");
        }

        [Test]
        public void AndIntTest()
        {
            const string NameLeft = "a";
            const string NameRight = "b";

            var parameterLeft = Expression.Parameter(typeof(int), NameLeft);
            var parameterRight = Expression.Parameter(typeof(int), NameRight);
            var compiled = Expression.Lambda<Func<int, int, int>>(Expression.And(parameterLeft, parameterRight), parameterLeft, parameterRight).Compile();

            Assert.AreEqual(0, compiled(0, 0), "t1");
            Assert.AreEqual(0, compiled(0, 1), "t2");
            Assert.AreEqual(0, compiled(1, 0), "t3");
            Assert.AreEqual(1, compiled(1, 1), "t4");
        }

        [Test]
        public void AndLifted()
        {
            var additionExpression = Expression.And
            (
                Expression.Constant(null, typeof(bool?)),
                Expression.Constant(null, typeof(bool?))
            );

            Assert.AreEqual(typeof(bool?), additionExpression.Type);
            Assert.IsTrue(additionExpression.IsLifted);
            Assert.IsTrue(additionExpression.IsLiftedToNull);
        }

        [Test]
        public void AndNullableBoolItem()
        {
            const string Name = "i";

            var parameter = Expression.Parameter(typeof(Item<bool?>), Name);
            var compiled = Expression.Lambda<Func<Item<bool?>, bool?>>
            (
                Expression.And
                (
                    Expression.Property(parameter, nameof(Item<bool?>.Left)),
                    Expression.Property(parameter, nameof(Item<bool?>.Right))
                ),
                parameter
            ).Compile();

            var itemA = new Item<bool?>(true, true);
            Assert.AreEqual((bool?)true, compiled(itemA));
            Assert.IsTrue(itemA.LeftCalled);
            Assert.IsTrue(itemA.RightCalled);

            var itemB = new Item<bool?>(true, false);
            Assert.AreEqual((bool?)false, compiled(itemB));
            Assert.IsTrue(itemB.LeftCalled);
            Assert.IsTrue(itemB.RightCalled);

            var itemC = new Item<bool?>(false, true);
            Assert.AreEqual((bool?)false, compiled(itemC));
            Assert.IsTrue(itemC.LeftCalled);
            Assert.IsTrue(itemC.RightCalled);

            var itemD = new Item<bool?>(false, false);
            Assert.AreEqual((bool?)false, compiled(itemD));
            Assert.IsTrue(itemD.LeftCalled);
            Assert.IsTrue(itemD.RightCalled);
        }

        [Test]
        public void Arg1Null()
        {
            const int Value = 1;

            Assert.Throws<ArgumentNullException>(() => Expression.And(null, Expression.Constant(Value)));
        }

        [Test]
        public void Arg2Null()
        {
            const int Value = 1;

            Assert.Throws<ArgumentNullException>(() => Expression.And(Expression.Constant(Value), null));
        }

        [Test]
        public void ArgTypesDifferent()
        {
            const int ValueLeft = 1;
            const bool ValueRight = true;

            Assert.Throws<InvalidOperationException>(() => Expression.And(Expression.Constant(ValueLeft), Expression.Constant(ValueRight)));
        }

        [Test]
        public void Boolean()
        {
            const bool ValueLeft = true;
            const bool ValueRight = false;

            var binaryExpression = Expression.And(Expression.Constant(ValueLeft), Expression.Constant(ValueRight));
            Assert.AreEqual(ExpressionType.And, binaryExpression.NodeType, "And#05");
            Assert.AreEqual(typeof(bool), binaryExpression.Type, "And#06");
            Assert.IsNull(binaryExpression.Method, "And#07");
            Assert.AreEqual($"({ValueLeft} And {ValueRight})", binaryExpression.ToString(), "And#08");
        }

        [Test]
        public void Double()
        {
            const double ValueLeft = 1.0;
            const double ValueRight = 2.0;

            Assert.Throws<InvalidOperationException>(() => Expression.And(Expression.Constant(ValueLeft), Expression.Constant(ValueRight)));
        }

        [Test]
        public void Integer()
        {
            const int ValueLeft = 1;
            const int ValueRight = 2;

            var binaryExpression = Expression.And(Expression.Constant(ValueLeft), Expression.Constant(ValueRight));
            Assert.AreEqual(ExpressionType.And, binaryExpression.NodeType, "And#01");
            Assert.AreEqual(typeof(int), binaryExpression.Type, "And#02");
            Assert.IsNull(binaryExpression.Method, "And#03");
            Assert.AreEqual($"({ValueLeft} & {ValueRight})", binaryExpression.ToString(), "And#04");
        }

        [Test]
        public void NoOperatorClass()
        {
            var valueLeft = new NoOpClass();
            var valueRight = new NoOpClass();

            Assert.Throws<InvalidOperationException>(() => Expression.And(Expression.Constant(valueLeft), Expression.Constant(valueRight)));
        }

        [Test]
        public void UserDefinedClass()
        {
            var type = typeof(OpClass);

            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var method = type.GetMethod("op_BitwiseAnd");

            var binaryExpression = Expression.And(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.And, binaryExpression.NodeType, "And#09");
            Assert.AreEqual(type, binaryExpression.Type, "And#10");
            Assert.AreEqual(method, binaryExpression.Method, "And#11");
            Assert.AreEqual
            (
                $"(value({type.FullName}) & value({type.FullName}))",
                binaryExpression.ToString(), "And#13"
            );
        }
    }
}