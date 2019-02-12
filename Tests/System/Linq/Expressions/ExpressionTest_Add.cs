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
using Theraot;
using NUnit.Framework;
using Tests.Helpers;

#if TARGETS_NETCORE || TARGETS_NETSTANDARD

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestAdd
    {
        public static class S
        {
            public static int MyAdder(int a, int b)
            {
                No.Op(a);
                No.Op(b);
                return 1000;
            }
        }

        private struct Slot
        {
            private readonly int _value;

            public Slot(int value)
            {
                _value = value;
            }

            public static Slot operator +(Slot a, Slot b)
            {
                return new Slot(a._value + b._value);
            }
        }

        private struct SlotToNullable
        {
            private readonly int _value;

            public SlotToNullable(int value)
            {
                _value = value;
            }

            public static SlotToNullable? operator +(SlotToNullable a, SlotToNullable b)
            {
                return new SlotToNullable(a._value + b._value);
            }
        }

        private struct SlotFromNullableToNullable
        {
            private readonly int _value;

            public SlotFromNullableToNullable(int value)
            {
                _value = value;
            }

            public static SlotFromNullableToNullable? operator +(SlotFromNullableToNullable? a, SlotFromNullableToNullable? b)
            {
                return a.HasValue && b.HasValue ? (SlotFromNullableToNullable?)new SlotFromNullableToNullable(a.Value._value + b.Value._value) : null;
            }
        }

        [Test]
        public void AddDecimals()
        {
            const string NameLeft = "l";
            const string NameRight = "r";
            const decimal Value = 1m;
            const decimal Result = Value + Value;

            var parameterLeft = Expression.Parameter(typeof(decimal), NameLeft);
            var parameterRight = Expression.Parameter(typeof(decimal), NameRight);

            var additionOperator = typeof(decimal).GetMethod("op_Addition", new[] {typeof(decimal), typeof(decimal)});

            var additionExpression = Expression.Add(parameterLeft, parameterRight);
            Assert.IsFalse(additionExpression.IsLifted);
            Assert.IsFalse(additionExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(decimal), additionExpression.Type);
            Assert.AreEqual(additionOperator, additionExpression.Method);

            var method = Expression.Lambda<Func<decimal, decimal, decimal>>(additionExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(Result, method(Value, Value));
        }

        [Test]
        public void AddLifted()
        {
            var additionExpression = Expression.Add
            (
                Expression.Constant(null, typeof(int?)),
                Expression.Constant(null, typeof(int?))
            );

            Assert.AreEqual(typeof(int?), additionExpression.Type);
            Assert.IsTrue(additionExpression.IsLifted);
            Assert.IsTrue(additionExpression.IsLiftedToNull);
        }

        [Test]
        public void AddLiftedDecimals()
        {
            const string NameLeft = "l";
            const string NameRight = "r";
            const decimal Value = 1m;
            const decimal Result = Value + Value;

            var parameterLeft = Expression.Parameter(typeof(decimal?), NameLeft);
            var parameterRight = Expression.Parameter(typeof(decimal?), NameRight);

            var additionOperator = typeof(decimal).GetMethod("op_Addition", new[] {typeof(decimal), typeof(decimal)});

            var additionExpression = Expression.Add(parameterLeft, parameterRight);
            Assert.IsTrue(additionExpression.IsLifted);
            Assert.IsTrue(additionExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(decimal?), additionExpression.Type);
            Assert.AreEqual(additionOperator, additionExpression.Method);

            var add = Expression.Lambda<Func<decimal?, decimal?, decimal?>>(additionExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(Result, add(Value, Value));
            Assert.AreEqual(null, add(Value, null));
            Assert.AreEqual(null, add(null, null));
        }

        [Test]
        public void AddNotLifted()
        {
            var additionExpression = Expression.Add
            (
                Expression.Constant(1, typeof(int)),
                Expression.Constant(1, typeof(int))
            );

            Assert.AreEqual(typeof(int), additionExpression.Type);
            Assert.IsFalse(additionExpression.IsLifted);
            Assert.IsFalse(additionExpression.IsLiftedToNull);
        }

        [Test]
        public void AddStrings()
        {
            const string NameLeft = "l";
            const string NameRight = "r";
            const string ValueLeft = "foo";
            const string ValueRight = "bar";
            const string Result = ValueLeft + ValueRight;

            var parameterLeft = Expression.Parameter(typeof(string), NameLeft);
            var parameterRight = Expression.Parameter(typeof(string), NameRight);

            var additionOperator = typeof(string).GetMethod(nameof(string.Concat), new[] {typeof(object), typeof(object)});

            var additionExpression = Expression.Add(parameterLeft, parameterRight, additionOperator);
            Assert.IsFalse(additionExpression.IsLifted);
            Assert.IsFalse(additionExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(string), additionExpression.Type);
            Assert.AreEqual(additionOperator, additionExpression.Method);

            var method = Expression.Lambda<Func<string, string, string>>(additionExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(string.Empty, method(null, null));
            Assert.AreEqual(Result, method(ValueLeft, ValueRight));
        }

        [Test]
        public void AddTestNullable()
        {
            const string NameLeft = "a";
            const string NameRight = "b";
            const int ValueLeft = 1;
            const int ValueRight = 2;
            const decimal Result = ValueLeft + ValueRight;

            var parameterLeft = Expression.Parameter(typeof(int?), NameLeft);
            var parameterRight = Expression.Parameter(typeof(int?), NameRight);
            var lambda = Expression.Lambda<Func<int?, int?, int?>>(Expression.Add(parameterLeft, parameterRight), parameterLeft, parameterRight);

            var binaryExpression = lambda.Body as BinaryExpression;
            Assert.IsNotNull(binaryExpression);
            Assert.AreEqual(typeof(int?), binaryExpression.Type);
            Assert.IsTrue(binaryExpression.IsLifted);
            Assert.IsTrue(binaryExpression.IsLiftedToNull);

            var method = lambda.Compile();

            Assert.AreEqual(null, method(ValueLeft, null), "a1");
            Assert.AreEqual(null, method(null, null), "a2");
            Assert.AreEqual(null, method(null, ValueRight), "a3");
            Assert.AreEqual(Result, method(ValueLeft, ValueRight), "a4");
        }

        [Test]
        public void Arg1Null()
        {
            const int Value = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.Add(null, Expression.Constant(Value)));
        }

        [Test]
        public void Arg2Null()
        {
            const int Value = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.Add(Expression.Constant(Value), null));
        }

        [Test]
        public void ArgTypesDifferent()
        {
            const int ValueLeft = 1;
            const double ValueRight = 2.0;

            AssertEx.Throws<InvalidOperationException>(() => Expression.Add(Expression.Constant(ValueLeft), Expression.Constant(ValueRight)));
        }

        [Test]
        public void Boolean()
        {
            const bool ValueLeft = true;
            const bool ValueRight = false;

            AssertEx.Throws<InvalidOperationException>(() => Expression.Add(Expression.Constant(ValueLeft), Expression.Constant(ValueRight)));
        }

        [Test]
        public void CompileAdd()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            const int ValueALeft = 6;
            const int ValueARight = 6;
            const int ValueBLeft = -1;
            const int ValueBRight = 1;
            const int ValueCLeft = 1;
            const int ValueCRight = -3;

            const int ResultA = ValueALeft + ValueARight;
            const int ResultB = ValueBLeft + ValueBRight;
            const int ResultC = ValueCLeft + ValueCRight;

            var left = Expression.Parameter(typeof(int), NameLeft);
            var right = Expression.Parameter(typeof(int), NameRight);
            var lambda = Expression.Lambda<Func<int, int, int>>(Expression.Add(left, right), left, right);

            var binaryExpression = lambda.Body as BinaryExpression;
            Assert.IsNotNull(binaryExpression);
            Assert.AreEqual(typeof(int), binaryExpression.Type);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);

            var method = lambda.Compile();

            Assert.AreEqual(ResultA, method(ValueALeft, ValueARight));
            Assert.AreEqual(ResultB, method(ValueBLeft, ValueBRight));
            Assert.AreEqual(ResultC, method(ValueCLeft, ValueCRight));
        }

        [Test]
        public void NoOperatorClass()
        {
            AssertEx.Throws<InvalidOperationException>(() => Expression.Add(Expression.Constant(new NoOpClass()), Expression.Constant(new NoOpClass())));
        }

        [Test]
        public void Nullable()
        {
            int? valueLeft = 1;
            int? valueRight = 2;

            var additionExpression = Expression.Add
            (
                Expression.Constant(valueLeft, typeof(int?)),
                Expression.Constant(valueRight, typeof(int?))
            );
            Assert.AreEqual(ExpressionType.Add, additionExpression.NodeType, "Add#05");
            Assert.AreEqual(typeof(int?), additionExpression.Type, "Add#06");
            Assert.IsNull(additionExpression.Method, "Add#07");
            Assert.AreEqual($"({valueLeft} + {valueRight})", additionExpression.ToString(), "Add#08");
        }

        [Test]
        public void Numeric()
        {
            const int ValueLeft = 1;
            const int ValueRight = 2;

            var additionExpression = Expression.Add(Expression.Constant(ValueLeft), Expression.Constant(ValueRight));
            Assert.AreEqual(ExpressionType.Add, additionExpression.NodeType, "Add#01");
            Assert.AreEqual(typeof(int), additionExpression.Type, "Add#02");
            Assert.IsNull(additionExpression.Method, "Add#03");
            Assert.AreEqual($"({ValueLeft} + {ValueRight})", additionExpression.ToString(), "Add#04");
        }

        [Test]
        public void TestMethodAddition()
        {
            const int ValueLeft = 1;
            const int ValueRight = 2;
            var result = S.MyAdder(ValueLeft, ValueRight);

            var expression = Expression.Add
            (
                Expression.Constant(ValueLeft),
                Expression.Constant(ValueRight),
                typeof(S).GetMethod(nameof(S.MyAdder))
            );
            var lambda = Expression.Lambda<Func<int>>(expression);

            var method = lambda.Compile();
            Assert.AreEqual(result, method());
        }

        [Test]
        public void UserDefinedAdd()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            const int ValueALeft = 21;
            const int ValueARight = 21;
            const int ValueBLeft = 1;
            const int ValueBRight = -1;

            const int ResultA = ValueALeft + ValueARight;
            const int ResultB = ValueBLeft + ValueBRight;

            var parameterLeft = Expression.Parameter(typeof(Slot), NameLeft);
            var parameterRight = Expression.Parameter(typeof(Slot), NameRight);

            var additionExpression = Expression.Add(parameterLeft, parameterRight);

            Assert.IsFalse(additionExpression.IsLifted);
            Assert.IsFalse(additionExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(Slot), additionExpression.Type);

            var method = Expression.Lambda<Func<Slot, Slot, Slot>>(additionExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(new Slot(ResultA), method(new Slot(ValueALeft), new Slot(ValueARight)));
            Assert.AreEqual(new Slot(ResultB), method(new Slot(ValueBLeft), new Slot(ValueBRight)));
        }

        [Test]
        public void UserDefinedAddLifted()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            var parameterLeft = Expression.Parameter(typeof(Slot?), NameLeft);
            var parameterRight = Expression.Parameter(typeof(Slot?), NameRight);

            var additionExpression = Expression.Add(parameterLeft, parameterRight);

            Assert.IsTrue(additionExpression.IsLifted);
            Assert.IsTrue(additionExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(Slot?), additionExpression.Type);

            var method = Expression.Lambda<Func<Slot?, Slot?, Slot?>>(additionExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(null, method(null, null));
            Assert.AreEqual((Slot?)new Slot(42), method(new Slot(21), new Slot(21)));
        }

        [Test]
        public void UserDefinedClass()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var additionOperator = typeof(OpClass).GetMethod("op_Addition");

            var left = new OpClass();
            var additionExpression = Expression.Add(Expression.Constant(left), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.Add, additionExpression.NodeType, "Add#09");
            Assert.AreEqual(typeof(OpClass), additionExpression.Type, "Add#10");
            Assert.AreEqual(additionOperator, additionExpression.Method, "Add#11");
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual("op_Addition", additionExpression.Method.Name, "Add#12");
            Assert.AreEqual
            (
                $"(value({typeof(OpClass).FullName}) + value({typeof(OpClass).FullName}))",
                additionExpression.ToString(),
                "Add#13"
            );

            var lambda = Expression.Lambda<Func<OpClass>>(additionExpression);

			var compiled = lambda.Compile();
			Assert.AreEqual (left, compiled  ());
        }

        [Test]
        public void UserDefinedFromNullableToNullableAdd()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            const int ValueA = 2;
            const int ValueB = -2;

            const int ResultA = ValueA + ValueA;
            const int ResultB = ValueA + ValueB;
            
            var parameterLeft = Expression.Parameter(typeof(SlotFromNullableToNullable?), NameLeft);
            var parameterRight = Expression.Parameter(typeof(SlotFromNullableToNullable?), NameRight);

            var additionExpression = Expression.Add(parameterLeft, parameterRight);

            Assert.IsFalse(additionExpression.IsLifted);
            Assert.IsFalse(additionExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(SlotFromNullableToNullable?), additionExpression.Type);
            Assert.IsNotNull(additionExpression.Method);

            var method = Expression.Lambda<Func<SlotFromNullableToNullable?, SlotFromNullableToNullable?, SlotFromNullableToNullable?>>(additionExpression, parameterLeft, parameterRight).Compile();

            AssertEx.AreEqual(null, method(null, null));
            AssertEx.AreEqual(null, method(new SlotFromNullableToNullable(ValueA), null));
            AssertEx.AreEqual(null, method(null, new SlotFromNullableToNullable(ValueA)));
            AssertEx.AreEqual(new SlotFromNullableToNullable(ResultA), method(new SlotFromNullableToNullable(ValueA), new SlotFromNullableToNullable(ValueA)));
            AssertEx.AreEqual(new SlotFromNullableToNullable(ResultB), method(new SlotFromNullableToNullable(ValueA), new SlotFromNullableToNullable(ValueB)));
        }

        [Test]
        public void UserDefinedToNullableAdd()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            const int ValueA = 2;
            const int ValueB = -2;

            const int ResultA = ValueA + ValueA;
            const int ResultB = ValueA + ValueB;

            var parameterLeft = Expression.Parameter(typeof(SlotToNullable), NameLeft);
            var parameterRight = Expression.Parameter(typeof(SlotToNullable), NameRight);

            var additionExpression = Expression.Add(parameterLeft, parameterRight);

            Assert.IsFalse(additionExpression.IsLifted);
            Assert.IsFalse(additionExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(SlotToNullable?), additionExpression.Type);
            Assert.IsNotNull(additionExpression.Method);

            var method = Expression.Lambda<Func<SlotToNullable, SlotToNullable, SlotToNullable?>>(additionExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual((SlotToNullable?)new SlotToNullable(ResultA), method(new SlotToNullable(ValueA), new SlotToNullable(ValueA)));
            Assert.AreEqual((SlotToNullable?)new SlotToNullable(ResultB), method(new SlotToNullable(ValueA), new SlotToNullable(ValueB)));
        }

        [Test]
        public void UserDefinedToNullableAddFromNullable()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            AssertEx.Throws<InvalidOperationException>
            (
                () => Expression.Add
                (
                    Expression.Parameter(typeof(SlotToNullable?), NameLeft),
                    Expression.Parameter(typeof(SlotToNullable?), NameRight)
                )
            );
        }
    }
}