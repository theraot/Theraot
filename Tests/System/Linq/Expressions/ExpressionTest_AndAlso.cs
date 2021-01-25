#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

#pragma warning disable IDE1006 // Naming Styles
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
//      Federico Di Gregorio <fog@initd.org>
//      Jb Evain <jbevain@novell.com>

using System;
using System.Linq;
using System.Linq.Expressions;
using Theraot;
using NUnit.Framework;

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestAndAlso
    {
        [Test]
        public void AndAlsoBoolItem()
        {
            const string name = "i";

            var parameter = Expression.Parameter(typeof(Item<bool>), name);
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
            var type = typeof(bool);

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
            const string name = "i";

            var parameter = Expression.Parameter(typeof(Item<bool?>), name);
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
            const string nameLeft = "a";
            const string nameRight = "b";

            var type = typeof(bool);

            var parameterLeft = Expression.Parameter(type, nameLeft);
            var parameterRight = Expression.Parameter(type, nameRight);
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
            const string nameLeft = "a";
            const string nameRight = "b";

            var type = typeof(bool?);

            var parameterLeft = Expression.Parameter(type, nameLeft);
            var parameterRight = Expression.Parameter(type, nameRight);
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
            const string name = "b";

            var parameter = Expression.Parameter(typeof(B), name);
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
            const string nameLeft = "l";
            const string nameRight = "r";

            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    var parameterLeft = Expression.Parameter(typeof(Incomplete), nameLeft);
                    var parameterRight = Expression.Parameter(typeof(Incomplete), nameRight);

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
            const string nameLeft = "l";
            const string nameRight = "r";

            var input = new[]
            {
                (Left: 64, Right: 64),
                (Left: 64, Right: 32),
                (Left: 32, Right: 64),
                (Left: 32, Right: 32)
            };

            var instances = input.Select(value => (value.Left, value.Right, Result: value.Left & value.Right));

            var parameterLeft = Expression.Parameter(typeof(Slot), nameLeft);
            var parameterRight = Expression.Parameter(typeof(Slot), nameRight);

            var method = typeof(Slot).GetMethod("op_BitwiseAnd");

            var binaryExpression = Expression.AndAlso(parameterLeft, parameterRight, method);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(method, binaryExpression.Method);

            var compiled = Expression.Lambda<Func<Slot, Slot, Slot>>(binaryExpression, parameterLeft, parameterRight).Compile();

            foreach (var (left, right, result) in instances)
            {
                Assert.AreEqual(new Slot(result), compiled(new Slot(left), new Slot(right)));
            }
        }

        [Test]
        public void UserDefinedAndAlsoLiftedToNull()
        {
            const string nameLeft = "l";
            const string nameRight = "r";

            var input = new (int? Left, int? Right)[]
            {
                (Left: 64, Right: 64),
                (Left: 64, Right: 32),
                (Left: 32, Right: 64),
                (Left: 32, Right: 32),
                (Left: null, Right: 64),
                (Left: 64, Right: null),
                (Left: null, Right: null)
            };

            var instances = input.Select(value => (value.Left, value.Right, Result: value.Left & value.Right));

            var parameterLeft = Expression.Parameter(typeof(Slot?), nameLeft);
            var parameterRight = Expression.Parameter(typeof(Slot?), nameRight);

            var method = typeof(Slot).GetMethod("op_BitwiseAnd");

            var node = Expression.AndAlso(parameterLeft, parameterRight, method);
            Assert.IsTrue(node.IsLifted);
            Assert.IsTrue(node.IsLiftedToNull);
            Assert.AreEqual(method, node.Method);

            var compiled = Expression.Lambda<Func<Slot?, Slot?, Slot?>>(node, parameterLeft, parameterRight).Compile();

            foreach (var (left, right, result) in instances)
            {
                Assert.AreEqual
                (
                    result == null ? default(Slot?) : new Slot(result.Value),
                    compiled
                    (
                        left == null ? default(Slot?) : new Slot(left.Value),
                        right == null ? default(Slot?) : new Slot(right.Value)
                    )
                );
            }
        }

        [Test]
        public void UserDefinedAndAlsoShortCircuit()
        {
            const string name = "i";

            var parameter = Expression.Parameter(typeof(Item<Slot>), name);
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
        [Category("NotDotNet")]
        public void UserDefinedLiftedAndAlsoShortCircuit()
        {
            const string name = "i";
            const int value = 1;

            var parameter = Expression.Parameter(typeof(Item<Slot?>), name);
            var compiled = Expression.Lambda<Func<Item<Slot?>, Slot?>>
            (
                Expression.AndAlso
                (
                    Expression.Property(parameter, nameof(Item<bool>.Left)),
                    Expression.Property(parameter, nameof(Item<bool>.Right))
                ),
                parameter
            ).Compile();

            var item = new Item<Slot?>(null, new Slot(value));
            Assert.AreEqual(null, compiled(item));
            Assert.IsTrue(item.LeftCalled);
            Assert.IsFalse(item.RightCalled);
        }

        private struct Incomplete
        {
            private readonly int _value;

            private Incomplete(int val)
            {
                _value = val;
            }

            public static Incomplete operator &(Incomplete a, Incomplete b)
            {
                return new Incomplete(a._value & b._value);
            }
        }

        private struct Slot
        {
            private readonly int _value;

            public Slot(int val)
            {
                _value = val;
            }

            public static Slot operator &(Slot a, Slot b)
            {
                return new Slot(a._value & b._value);
            }

            public static bool operator false(Slot a)
            {
                return a._value == 0;
            }

            public static bool operator true(Slot a)
            {
                return a._value != 0;
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }

        private class A // Should not be static, inheritance is needed for testing
        {
            public static bool operator false(A x)
            {
                No.Op(x);
                return false;
            }

            public static bool operator true(A x)
            {
                No.Op(x);
                return true;
            }
        }

        private sealed class B : A
        {
            // ReSharper disable once UnusedMember.Local
            public static bool op_False(B x)
            {
                No.Op(x);
                return false;
            }

            // ReSharper disable once UnusedMember.Local
            public static bool op_True<T>(B x)
            {
                No.Op(x);
                No.Op(typeof(T));
                return true;
            }

            public static B operator &(B x, B y)
            {
                No.Op(x);
                No.Op(y);
                return new B();
            }
        }
    }
}