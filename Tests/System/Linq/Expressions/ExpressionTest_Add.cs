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
using Tests.Helpers;
using Theraot;

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
            const decimal ValueLeft = 1m;
            const decimal ValueRight = 1m;
            const decimal Result = ValueLeft + ValueRight;
            var method = typeof(decimal).GetMethod("op_Addition", new[] {typeof(decimal), typeof(decimal)});

            var parameterLeft = Expression.Parameter(typeof(decimal), NameLeft);
            var parameterRight = Expression.Parameter(typeof(decimal), NameRight);

            var binaryExpression = Expression.Add(parameterLeft, parameterRight);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(decimal), binaryExpression.Type);
            Assert.AreEqual(method, binaryExpression.Method);

            var compiled = Expression.Lambda<Func<decimal, decimal, decimal>>(binaryExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(Result, compiled(ValueLeft, ValueRight));
        }

        [Test]
        public void AddLifted()
        {
            var type = typeof(int?);

            var additionExpression = Expression.Add
            (
                Expression.Constant(null, type),
                Expression.Constant(null, type)
            );

            Assert.AreEqual(type, additionExpression.Type);
            Assert.IsTrue(additionExpression.IsLifted);
            Assert.IsTrue(additionExpression.IsLiftedToNull);
        }

        [Test]
        public void AddLiftedDecimals()
        {
            const string NameLeft = "l";
            const string NameRight = "r";
            const decimal ValueLeft = 1m;
            const decimal ValueRight = 1m;
            const decimal Result = ValueLeft + ValueRight;
            var type = typeof(decimal?);
            var method = typeof(decimal).GetMethod("op_Addition", new[] {typeof(decimal), typeof(decimal)});

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);

            var binaryExpression = Expression.Add(parameterLeft, parameterRight);
            Assert.IsTrue(binaryExpression.IsLifted);
            Assert.IsTrue(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(type, binaryExpression.Type);
            Assert.AreEqual(method, binaryExpression.Method);

            var compiled = Expression.Lambda<Func<decimal?, decimal?, decimal?>>(binaryExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(Result, compiled(ValueLeft, ValueRight));
            Assert.AreEqual(null, compiled(ValueLeft, null));
            Assert.AreEqual(null, compiled(null, ValueRight));
            Assert.AreEqual(null, compiled(null, null));
        }

        [Test]
        public void AddNotLifted()
        {
            const int ValueLeft = 1;
            const int ValueRight = 1;
            var type = typeof(int);

            var binaryExpression = Expression.Add
            (
                Expression.Constant(ValueLeft, type),
                Expression.Constant(ValueRight, type)
            );

            Assert.AreEqual(type, binaryExpression.Type);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
        }

        [Test]
        public void AddStrings()
        {
            const string NameLeft = "l";
            const string NameRight = "r";
            const string ValueLeft = "foo";
            const string ValueRight = "bar";
            const string Result = ValueLeft + ValueRight;
            var type = typeof(string);
            var method = type.GetMethod(nameof(string.Concat), new[] {typeof(object), typeof(object)});

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);

            var binaryExpression = Expression.Add(parameterLeft, parameterRight, method);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(type, binaryExpression.Type);
            Assert.AreEqual(method, binaryExpression.Method);

            var compiled = Expression.Lambda<Func<string, string, string>>(binaryExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(string.Empty, compiled(null, null));
            Assert.AreEqual(Result, compiled(ValueLeft, ValueRight));
        }

        [Test]
        public void AddTestNullable()
        {
            const string NameLeft = "a";
            const string NameRight = "b";
            const int ValueLeft = 1;
            const int ValueRight = 2;
            const decimal Result = ValueLeft + ValueRight;
            var type = typeof(int?);

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);
            var lambda = Expression.Lambda<Func<int?, int?, int?>>(Expression.Add(parameterLeft, parameterRight), parameterLeft, parameterRight);

            var binaryExpression = lambda.Body as BinaryExpression;
            Assert.IsNotNull(binaryExpression);
            Assert.AreEqual(type, binaryExpression.Type);
            Assert.IsTrue(binaryExpression.IsLifted);
            Assert.IsTrue(binaryExpression.IsLiftedToNull);

            var compiled = lambda.Compile();

            Assert.AreEqual(null, compiled(ValueLeft, null), "a1");
            Assert.AreEqual(null, compiled(null, null), "a2");
            Assert.AreEqual(null, compiled(null, ValueRight), "a3");
            Assert.AreEqual(Result, compiled(ValueLeft, ValueRight), "a4");
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

            var type = typeof(int);

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);
            var lambda = Expression.Lambda<Func<int, int, int>>(Expression.Add(parameterLeft, parameterRight), parameterLeft, parameterRight);

            var binaryExpression = lambda.Body as BinaryExpression;
            Assert.IsNotNull(binaryExpression);
            Assert.AreEqual(type, binaryExpression.Type);
            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);

            var compiled = lambda.Compile();

            Assert.AreEqual(ResultA, compiled(ValueALeft, ValueARight));
            Assert.AreEqual(ResultB, compiled(ValueBLeft, ValueBRight));
            Assert.AreEqual(ResultC, compiled(ValueCLeft, ValueCRight));
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
            var type = typeof(int?);

            var binaryExpression = Expression.Add
            (
                Expression.Constant(valueLeft, type),
                Expression.Constant(valueRight, type)
            );
            Assert.AreEqual(ExpressionType.Add, binaryExpression.NodeType, "Add#05");
            Assert.AreEqual(type, binaryExpression.Type, "Add#06");
            Assert.IsNull(binaryExpression.Method, "Add#07");
            Assert.AreEqual($"({valueLeft} + {valueRight})", binaryExpression.ToString(), "Add#08");
        }

        [Test]
        public void Numeric()
        {
            const int ValueLeft = 1;
            const int ValueRight = 2;
            var type = typeof(int);

            var binaryExpression = Expression.Add(Expression.Constant(ValueLeft), Expression.Constant(ValueRight));
            Assert.AreEqual(ExpressionType.Add, binaryExpression.NodeType, "Add#01");
            Assert.AreEqual(type, binaryExpression.Type, "Add#02");
            Assert.IsNull(binaryExpression.Method, "Add#03");
            Assert.AreEqual($"({ValueLeft} + {ValueRight})", binaryExpression.ToString(), "Add#04");
        }

        [Test]
        public void TestMethodAddition()
        {
            const int ValueLeft = 1;
            const int ValueRight = 2;
            var result = S.MyAdder(ValueLeft, ValueRight);

            var binaryExpression = Expression.Add
            (
                Expression.Constant(ValueLeft),
                Expression.Constant(ValueRight),
                typeof(S).GetMethod(nameof(S.MyAdder))
            );
            var lambda = Expression.Lambda<Func<int>>(binaryExpression);

            var compiled = lambda.Compile();
            Assert.AreEqual(result, compiled());
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

            var type = typeof(Slot);

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);

            var binaryExpression = Expression.Add(parameterLeft, parameterRight);

            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(type, binaryExpression.Type);

            var compiled = Expression.Lambda<Func<Slot, Slot, Slot>>(binaryExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(new Slot(ResultA), compiled(new Slot(ValueALeft), new Slot(ValueARight)));
            Assert.AreEqual(new Slot(ResultB), compiled(new Slot(ValueBLeft), new Slot(ValueBRight)));
        }

        [Test]
        public void UserDefinedAddLifted()
        {
            const string NameLeft = "l";
            const string NameRight = "r";

            const int ValueLeft = 21;
            const int ValueRight = 21;

            const int Result = ValueLeft + ValueRight;

            var type = typeof(Slot?);

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);

            var binaryExpression = Expression.Add(parameterLeft, parameterRight);

            Assert.IsTrue(binaryExpression.IsLifted);
            Assert.IsTrue(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(type, binaryExpression.Type);

            var compiled = Expression.Lambda<Func<Slot?, Slot?, Slot?>>(binaryExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual(null, compiled(null, null));
            Assert.AreEqual((Slot?)new Slot(Result), compiled(new Slot(ValueLeft), new Slot(ValueRight)));
        }

        [Test]
        public void UserDefinedClass()
        {
            var valueLeft = new OpClass();
            var valueRight = new OpClass();
            var result = valueLeft + valueRight;

            var type = typeof(OpClass);

            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var method = type.GetMethod("op_Addition");

            var binaryExpression = Expression.Add(Expression.Constant(valueLeft), Expression.Constant(valueRight));
            Assert.AreEqual(ExpressionType.Add, binaryExpression.NodeType, "Add#09");
            Assert.AreEqual(type, binaryExpression.Type, "Add#10");
            Assert.AreEqual(method, binaryExpression.Method, "Add#11");
            Assert.AreEqual
            (
                $"(value({type.FullName}) + value({type.FullName}))",
                binaryExpression.ToString(),
                "Add#13"
            );

            var lambda = Expression.Lambda<Func<OpClass>>(binaryExpression);

            var compiled = lambda.Compile();
            Assert.AreEqual(result, compiled());
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

            var type = typeof(SlotFromNullableToNullable?);

            var parameterLeft = Expression.Parameter(type, NameLeft);
            var parameterRight = Expression.Parameter(type, NameRight);

            var binaryExpression = Expression.Add(parameterLeft, parameterRight);

            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(type, binaryExpression.Type);
            Assert.IsNotNull(binaryExpression.Method);

            var compiled = Expression.Lambda<Func<SlotFromNullableToNullable?, SlotFromNullableToNullable?, SlotFromNullableToNullable?>>(binaryExpression, parameterLeft, parameterRight).Compile();

            AssertEx.AreEqual(null, compiled(null, null));
            AssertEx.AreEqual(null, compiled(new SlotFromNullableToNullable(ValueA), null));
            AssertEx.AreEqual(null, compiled(null, new SlotFromNullableToNullable(ValueA)));
            AssertEx.AreEqual(new SlotFromNullableToNullable(ResultA), compiled(new SlotFromNullableToNullable(ValueA), new SlotFromNullableToNullable(ValueA)));
            AssertEx.AreEqual(new SlotFromNullableToNullable(ResultB), compiled(new SlotFromNullableToNullable(ValueA), new SlotFromNullableToNullable(ValueB)));
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

            var binaryExpression = Expression.Add(parameterLeft, parameterRight);

            Assert.IsFalse(binaryExpression.IsLifted);
            Assert.IsFalse(binaryExpression.IsLiftedToNull);
            Assert.AreEqual(typeof(SlotToNullable?), binaryExpression.Type);
            Assert.IsNotNull(binaryExpression.Method);

            var compiled = Expression.Lambda<Func<SlotToNullable, SlotToNullable, SlotToNullable?>>(binaryExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual((SlotToNullable?)new SlotToNullable(ResultA), compiled(new SlotToNullable(ValueA), new SlotToNullable(ValueA)));
            Assert.AreEqual((SlotToNullable?)new SlotToNullable(ResultB), compiled(new SlotToNullable(ValueA), new SlotToNullable(ValueB)));
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