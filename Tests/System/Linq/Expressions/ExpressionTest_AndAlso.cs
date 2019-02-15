#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

#pragma warning disable IDE1006 // Estilos de nombres
#pragma warning disable RECS0014 // If all fields, properties and methods members are static, the class can be made static.

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
using Theraot;
using NUnit.Framework;

#if TARGETS_NETCORE || TARGETS_NETSTANDARD
using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestAndAlso
    {
        private struct Incomplete
        {
            public readonly int Value;

            public Incomplete(int val)
            {
                Value = val;
            }

            public static Incomplete operator &(Incomplete a, Incomplete b)
            {
                return new Incomplete(a.Value & b.Value);
            }
        }

        private struct Slot
        {
            public readonly int Value;

            public Slot(int val)
            {
                Value = val;
            }

            public static Slot operator &(Slot a, Slot b)
            {
                return new Slot(a.Value & b.Value);
            }

            public static bool operator true(Slot a)
            {
                return a.Value != 0;
            }

            public static bool operator false(Slot a)
            {
                return a.Value == 0;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        private class A // Should not be static, inheritance is needed for testing
        {
            public static bool operator true(A x)
            {
                No.Op(x);
                return true;
            }

            public static bool operator false(A x)
            {
                No.Op(x);
                return false;
            }
        }

        private class B : A
        {
            public static B operator &(B x, B y)
            {
                No.Op(x);
                No.Op(y);
                return new B();
            }

            // ReSharper disable once UnusedMember.Local
            public static bool op_True<T>(B x)
            {
                No.Op(x);
                No.Op(typeof(T));
                return true;
            }

            // ReSharper disable once UnusedMember.Local
            public static bool op_False(B x)
            {
                No.Op(x);
                return false;
            }
        }

        [Test]
        public void AndAlsoBoolItem()
        {
            const string Name = "i";

            var parameter = Expression.Parameter(typeof(Item<bool>), Name);
            var compiled = Expression.Lambda<Func<Item<bool>, bool>>
            (
                Expression.AndAlso
                (
                    Expression.Property(parameter, nameof(Item<bool>.Left)),
                    Expression.Property(parameter, nameof(Item<bool>.Right))
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
            Assert.IsFalse(itemC.RightCalled);

            var itemD = new Item<bool>(false, false);
            Assert.AreEqual(false, compiled(itemD));
            Assert.IsTrue(itemD.LeftCalled);
            Assert.IsFalse(itemD.RightCalled);
        }

        [Test]
        public void AndAlsoLifted()
        {
            var type = typeof(bool?);

            var binaryExpression = Expression.AndAlso
            (
                Expression.Constant(null, type),
                Expression.Constant(null, type)
            );

            Assert.AreEqual(type, binaryExpression.Type);
            Assert.IsTrue(binaryExpression.IsLifted);
            Assert.IsTrue(binaryExpression.IsLiftedToNull);
        }

        [Test]
        public void AndAlsoNotLifted()
        {
            var type = typeof(bool?);

            var binaryExpression = Expression.AndAlso
            (
                Expression.Constant(true, type),
                Expression.Constant(true, type)
            );

            Assert.AreEqual(type, binaryExpression.Type);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
        }

        [Test]
        public void AndAlsoNullableBoolItem()
        {
            const string Name = "i";

            var parameter = Expression.Parameter(typeof(Item<bool?>), Name);
            var compiled = Expression.Lambda<Func<Item<bool?>, bool?>>
            (
                Expression.AndAlso
                (
                    Expression.Property(parameter, nameof(Item<bool>.Left)),
                    Expression.Property(parameter, nameof(Item<bool>.Right))
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
            Assert.IsFalse(itemC.RightCalled);

            var itemD = new Item<bool?>(false, false);
            Assert.AreEqual((bool?)false, compiled(itemD));
            Assert.IsTrue(itemD.LeftCalled);
            Assert.IsFalse(itemD.RightCalled);
        }

        [Test]
        public void AndAlsoTest()
        {
            const string NameLeft = "a";
            const string NameRight = "b";

            var type = typeof(bool);

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);
            var lambda = Expression.Lambda<Func<bool, bool, bool>>(Expression.AndAlso(parameterLeft, parameterRight), parameterLeft, parameterRight);

            var binaryExpression = lambda.Body as BinaryExpression;
            Assert.IsNotNull(binaryExpression);
            Assert.AreEqual(type, binaryExpression.Type);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);

            var compiled = lambda.Compile();

            Assert.AreEqual(true, compiled(true, true), "a1");
            Assert.AreEqual(false, compiled(true, false), "a2");
            Assert.AreEqual(false, compiled(false, true), "a3");
            Assert.AreEqual(false, compiled(false, false), "a4");
        }

        [Test]
        public void AndAlsoTestNullable()
        {
            const string NameLeft = "a";
            const string NameRight = "b";

            var type = typeof(bool?);

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);
            var lambda = Expression.Lambda<Func<bool?, bool?, bool?>>(Expression.AndAlso(parameterLeft, parameterRight), parameterLeft, parameterRight);

            var binaryExpression = lambda.Body as BinaryExpression;
            Assert.IsNotNull(binaryExpression);
            Assert.AreEqual(type, binaryExpression.Type);
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
            Assert.AreEqual(null, compiled(true, null), "a8");
            Assert.AreEqual(null, compiled(null, null), "a9");
        }

        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.AndAlso(null, Expression.Constant(1)));
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.AndAlso(Expression.Constant(1), null));
        }

        [Test]
        public void Boolean()
        {
            var binaryExpression = Expression.AndAlso(Expression.Constant(true), Expression.Constant(false));
            Assert.AreEqual(ExpressionType.AndAlso, binaryExpression.NodeType, "AndAlso#01");
            Assert.AreEqual(typeof(bool), binaryExpression.Type, "AndAlso#02");
            Assert.IsNull(binaryExpression.Method, "AndAlso#03");
        }

        [Test]
        public void Connect350487()
        {
            const string Name = "b";

            var parameter = Expression.Parameter(typeof(B), Name);
            var compiled = Expression.Lambda<Func<B, A>>
            (
                Expression.AndAlso(parameter, parameter),
                parameter
            ).Compile();

            Assert.IsNotNull(compiled(null));
        }

        [Test]
        public void Double()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.AndAlso(Expression.Constant(1.0), Expression.Constant(2.0)));
        }

        [Test]
        public void IncompleteUserDefinedAndAlso()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var parameterLeft = Expression.Parameter(typeof(Incomplete), NameLeft);
                    var parameterRight = Expression.Parameter(typeof(Incomplete), NameRight);

                    var method = typeof(Incomplete).GetMethod("op_BitwiseAnd");

                    Expression.AndAlso(parameterLeft, parameterRight, method);
                }
            );
        }

        [Test]
        public void Integer()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.AndAlso(Expression.Constant(1), Expression.Constant(2)));
        }

        [Test]
        public void MismatchedTypes()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.AndAlso(Expression.Constant(new OpClass()), Expression.Constant(true)));
        }

        [Test]
        public void NoOperatorClass()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.AndAlso(Expression.Constant(new NoOpClass()), Expression.Constant(new NoOpClass())));
        }

        [Test]
        public void UserDefinedAndAlso()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            const int ValueA = 64;
            const int ValueB = 32;

            const int ResultA = ValueA & ValueA;
            const int ResultB = ValueA & ValueB;
            const int ResultC = ValueB & ValueA;
            const int ResultD = ValueB & ValueB;

            var parameterLeft = Expression.Parameter(typeof(Slot), NameLeft);
            var parameterRight = Expression.Parameter(typeof(Slot), NameRight);

            var method = typeof(Slot).GetMethod("op_BitwiseAnd");

            var binaryExpression = Expression.AndAlso(parameterLeft, parameterRight, method);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(method, binaryExpression.Method);

            var compiled = Expression.Lambda<Func<Slot, Slot, Slot>>(binaryExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(new Slot(ResultA), compiled(new Slot(ValueA), new Slot(ValueA)));
            Assert.AreEqual(new Slot(ResultB), compiled(new Slot(ValueA), new Slot(ValueB)));
            Assert.AreEqual(new Slot(ResultC), compiled(new Slot(ValueB), new Slot(ValueA)));
            Assert.AreEqual(new Slot(ResultD), compiled(new Slot(ValueB), new Slot(ValueB)));
        }

        [Test]
        public void UserDefinedAndAlsoLiftedToNull()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            const int ValueA = 64;
            const int ValueB = 32;

            const int ResultA = ValueA & ValueA;
            const int ResultB = ValueA & ValueB;
            const int ResultC = ValueB & ValueA;
            const int ResultD = ValueB & ValueB;

            var parameterLeft = Expression.Parameter(typeof(Slot?), NameLeft);
            var parameterRight = Expression.Parameter(typeof(Slot?), NameRight);

            var method = typeof(Slot).GetMethod("op_BitwiseAnd");

            var node = Expression.AndAlso(parameterLeft, parameterRight, method);
            Assert.IsTrue(node.IsLifted);
            Assert.IsTrue(node.IsLiftedToNull);
            Assert.AreEqual(method, node.Method);

            var compiled = Expression.Lambda<Func<Slot?, Slot?, Slot?>>(node, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(new Slot(ResultA), compiled(new Slot(ValueA), new Slot(ValueA)));
            Assert.AreEqual(new Slot(ResultB), compiled(new Slot(ValueA), new Slot(ValueB)));
            Assert.AreEqual(new Slot(ResultC), compiled(new Slot(ValueB), new Slot(ValueA)));
            Assert.AreEqual(new Slot(ResultD), compiled(new Slot(ValueB), new Slot(ValueB)));
            Assert.AreEqual(null, compiled(null, new Slot(ValueA)));
            Assert.AreEqual(null, compiled(new Slot(ValueB), null));
            Assert.AreEqual(null, compiled(null, null));
        }

        [Test]
        public void UserDefinedAndAlsoShortCircuit()
        {
            const string Name = "i";

            var parameter = Expression.Parameter(typeof(Item<Slot>), Name);
            var compiled = Expression.Lambda<Func<Item<Slot>, Slot>>
            (
                Expression.AndAlso
                (
                    Expression.Property(parameter, nameof(Item<bool>.Left)),
                    Expression.Property(parameter, nameof(Item<bool>.Right))
                ),
                parameter
            ).Compile();

            var itemA = new Item<Slot>(new Slot(0), new Slot(0));
            Assert.AreEqual(new Slot(0), compiled(itemA));
            Assert.IsTrue(itemA.LeftCalled);
            Assert.IsFalse(itemA.RightCalled);

            var itemB = new Item<Slot>(new Slot(0), new Slot(1));
            Assert.AreEqual(new Slot(0), compiled(itemB));
            Assert.IsTrue(itemB.LeftCalled);
            Assert.IsFalse(itemB.RightCalled);

            var itemC = new Item<Slot>(new Slot(1), new Slot(0));
            Assert.AreEqual(new Slot(0), compiled(itemC));
            Assert.IsTrue(itemC.LeftCalled);
            Assert.IsTrue(itemC.RightCalled);

            var itemD = new Item<Slot>(new Slot(1), new Slot(1));
            Assert.AreEqual(new Slot(1), compiled(itemD));
            Assert.IsTrue(itemD.LeftCalled);
            Assert.IsTrue(itemD.RightCalled);
        }

        [Test]
        public void UserDefinedClass()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var method = typeof(OpClass).GetMethod("op_BitwiseAnd");

            var binaryExpression = Expression.AndAlso(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.AndAlso, binaryExpression.NodeType, "AndAlso#05");
            Assert.AreEqual(typeof(OpClass), binaryExpression.Type, "AndAlso#06");
            Assert.AreEqual(method, binaryExpression.Method, "AndAlso#07");
        }

        [Test]
        public void UserDefinedLiftedAndAlsoShortCircuit()
        {
            const string Name = "i";
            const int Value = 1;

            var parameter = Expression.Parameter(typeof(Item<Slot?>), Name);
            var compiled = Expression.Lambda<Func<Item<Slot?>, Slot?>>
            (
                Expression.AndAlso
                (
                    Expression.Property(parameter, nameof(Item<bool>.Left)),
                    Expression.Property(parameter, nameof(Item<bool>.Right))
                ),
                parameter
            ).Compile();

            var item = new Item<Slot?>(null, new Slot(Value));
            Assert.AreEqual(null, compiled(item));
            Assert.IsTrue(item.LeftCalled);
            Assert.IsFalse(item.RightCalled);
        }
    }
}