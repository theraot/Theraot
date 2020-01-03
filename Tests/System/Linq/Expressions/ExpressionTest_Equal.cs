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
//    Miguel de Icaza (miguel@novell.com)
//    Jb Evain (jbevain@novell.com)
//

using System;
using System.Linq.Expressions;
using NUnit.Framework;

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestEqual
    {
        public enum Foo
        {
            Bar,
            Baz
        }

        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Equal(null, Expression.Constant(1)));
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Equal(Expression.Constant(1), null));
        }

        [Test]
        public void ArgTypesDifferent()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Equal(Expression.Constant(1), Expression.Constant(2.0)));
        }

        [Test]
        public void EnumEqual()
        {
            var l = Expression.Parameter(typeof(Foo), "l");
            var r = Expression.Parameter(typeof(Foo), "r");

            var node = Expression.Equal(l, r);
            Assert.IsFalse(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNull(node.Method);

            var compiled = Expression.Lambda<Func<Foo, Foo, bool>>(node, l, r).Compile();

            Assert.AreEqual(true, compiled(Foo.Bar, Foo.Bar));
            Assert.AreEqual(false, compiled(Foo.Bar, Foo.Baz));
        }

        [Test]
        public void LiftedEnumEqual()
        {
            var l = Expression.Parameter(typeof(Foo?), "l");
            var r = Expression.Parameter(typeof(Foo?), "r");

            var node = Expression.Equal(l, r);
            Assert.IsTrue(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNull(node.Method);

            var compiled = Expression.Lambda<Func<Foo?, Foo?, bool>>(node, l, r).Compile();

            Assert.AreEqual(true, compiled(Foo.Bar, Foo.Bar));
            Assert.AreEqual(false, compiled(Foo.Bar, Foo.Baz));
            Assert.AreEqual(false, compiled(Foo.Bar, null));
            Assert.AreEqual(true, compiled(null, null));
        }

        [Test]
        public void NoOperatorClass()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Equal(Expression.Constant(new D()), Expression.Constant(new D())));
        }

        [Test]
        public void Nullable_LiftToNull_SetToFalse()
        {
            int? a = 1;
            int? b = 2;

            var expr = Expression.Equal
            (
                Expression.Constant(a, typeof(int?)),
                Expression.Constant(b, typeof(int?)),
                false, null
            );
            Assert.AreEqual(ExpressionType.Equal, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.AreEqual(true, expr.IsLifted);
            Assert.AreEqual(false, expr.IsLiftedToNull);
            Assert.IsNull(expr.Method);
        }

        [Test]
        public void Nullable_LiftToNull_SetToTrue()
        {
            int? a = 1;
            int? b = 2;

            var expr = Expression.Equal
            (
                Expression.Constant(a, typeof(int?)),
                Expression.Constant(b, typeof(int?)),
                true, null
            );
            Assert.AreEqual(ExpressionType.Equal, expr.NodeType);
            Assert.AreEqual(typeof(bool?), expr.Type);
            Assert.AreEqual(true, expr.IsLifted);
            Assert.AreEqual(true, expr.IsLiftedToNull);
            Assert.IsNull(expr.Method);
        }

        [Test]
        public void Nullable_Mixed()
        {
            Assert.Throws<InvalidOperationException>
            (
                () =>
                {
                    int? a = 1;
                    const int b = 2;

                    Expression.Equal
                    (
                        Expression.Constant(a, typeof(int?)),
                        Expression.Constant(b, typeof(int))
                    );
                }
            );
        }

        [Test]
        public void NullableBoolEqualToBool()
        {
            var l = Expression.Parameter(typeof(bool?), "l");
            var r = Expression.Parameter(typeof(bool?), "r");

            var node = Expression.Equal(l, r);
            Assert.IsTrue(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNull(node.Method);

            var compiled = Expression.Lambda<Func<bool?, bool?, bool>>(node, l, r).Compile();

            Assert.AreEqual(false, compiled(true, null));
            Assert.AreEqual(true, compiled(null, null));
            Assert.AreEqual(true, compiled(false, false));
        }

        [Test]
        public void NullableInt32Equal()
        {
            var l = Expression.Parameter(typeof(int?), "l");
            var r = Expression.Parameter(typeof(int?), "r");

            var compiled = Expression.Lambda<Func<int?, int?, bool>>
            (
                Expression.Equal(l, r), l, r
            ).Compile();

            Assert.IsTrue(compiled(null, null));
            Assert.IsFalse(compiled(null, 1));
            Assert.IsFalse(compiled(1, null));
            Assert.IsFalse(compiled(1, 2));
            Assert.IsTrue(compiled(1, 1));
            Assert.IsFalse(compiled(null, 0));
            Assert.IsFalse(compiled(0, null));
        }

        [Test]
        public void NullableInt32EqualLiftedToNull()
        {
            var l = Expression.Parameter(typeof(int?), "l");
            var r = Expression.Parameter(typeof(int?), "r");

            var compiled = Expression.Lambda<Func<int?, int?, bool?>>
            (
                Expression.Equal(l, r, true, null), l, r
            ).Compile();

            Assert.AreEqual(null, compiled(null, null));
            Assert.AreEqual(null, compiled(null, 1));
            Assert.AreEqual(null, compiled(1, null));
            Assert.AreEqual((bool?)false, compiled(1, 2));
            Assert.AreEqual((bool?)true, compiled(1, 1));
            Assert.AreEqual(null, compiled(null, 0));
            Assert.AreEqual(null, compiled(0, null));
        }

        [Test]
        public void NullableNullEqual()
        {
            var param = Expression.Parameter(typeof(DateTime?), "x");

            var node = Expression.Equal(param, Expression.Constant(null));

            Assert.IsTrue(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNull(node.Method);

            var compiled = Expression.Lambda<Func<DateTime?, bool>>(node, param).Compile();

            Assert.AreEqual(true, compiled(null));
            Assert.AreEqual(false, compiled(DateTime.Now));
        }

        [Test]
        public void Numeric()
        {
            var expr = Expression.Equal(Expression.Constant(1), Expression.Constant(2));
            Assert.AreEqual(ExpressionType.Equal, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.IsNull(expr.Method);
        }

        [Test]
        public void PrimitiveNonNumeric()
        {
            var expr = Expression.Equal(Expression.Constant('a'), Expression.Constant('b'));
            Assert.AreEqual(ExpressionType.Equal, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.IsNull(expr.Method);

            var compiled = Expression.Lambda<Func<bool>>(expr).Compile();
            Assert.IsFalse(compiled());
        }

        [Test]
        public void ReferenceCompare()
        {
            Expression.Equal(Expression.Constant(new NoOpClass()), Expression.Constant(new NoOpClass()));
        }

        [Test]
        public void UserDefinedClass()
        {
            // We can use the simplest version of GetMethod because we already know only one
            // exists in the very simple class we're using for the tests.
            var method = typeof(OpClass).GetMethod("op_Equality");

            var expr = Expression.Equal(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.Equal, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.AreEqual(method, expr.Method);
        }

        [Test]
        public void UserDefinedEqual()
        {
            var l = Expression.Parameter(typeof(Slot), "l");
            var r = Expression.Parameter(typeof(Slot), "r");

            var node = Expression.Equal(l, r);

            Assert.IsFalse(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNotNull(node.Method);

            var compiled = Expression.Lambda<Func<Slot, Slot, bool>>(node, l, r).Compile();

            Assert.AreEqual(true, compiled(new Slot(21), new Slot(21)));
            Assert.AreEqual(false, compiled(new Slot(1), new Slot(-1)));
        }

        [Test]
        public void UserDefinedEqualLifted()
        {
            var l = Expression.Parameter(typeof(Slot?), "l");
            var r = Expression.Parameter(typeof(Slot?), "r");

            var node = Expression.Equal(l, r);

            Assert.IsTrue(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNotNull(node.Method);

            var compiled = Expression.Lambda<Func<Slot?, Slot?, bool>>(node, l, r).Compile();

            Assert.AreEqual(true, compiled(null, null));
            Assert.AreEqual(false, compiled(new Slot(2), null));
            Assert.AreEqual(false, compiled(null, new Slot(2)));
            Assert.AreEqual(true, compiled(new Slot(21), new Slot(21)));
        }

        [Test]
        public void UserDefinedEqualLiftedToNull()
        {
            var l = Expression.Parameter(typeof(Slot?), "l");
            var r = Expression.Parameter(typeof(Slot?), "r");

            var node = Expression.Equal(l, r, true, null);

            Assert.IsTrue(node.IsLifted);
            Assert.IsTrue(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool?), node.Type);
            Assert.IsNotNull(node.Method);

            var compiled = Expression.Lambda<Func<Slot?, Slot?, bool?>>(node, l, r).Compile();

            Assert.AreEqual(null, compiled(null, null));
            Assert.AreEqual(null, compiled(new Slot(2), null));
            Assert.AreEqual(null, compiled(null, new Slot(2)));
            Assert.AreEqual((bool?)true, compiled(new Slot(21), new Slot(21)));
            Assert.AreEqual((bool?)false, compiled(new Slot(21), new Slot(-21)));
        }

        [Test]
        public void UserDefinedToNullableEqual()
        {
            var l = Expression.Parameter(typeof(SlotToNullable), "l");
            var r = Expression.Parameter(typeof(SlotToNullable), "r");

            var node = Expression.Equal(l, r, false, null);

            Assert.IsFalse(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool?), node.Type);
            Assert.IsNotNull(node.Method);

            var compiled = Expression.Lambda<Func<SlotToNullable, SlotToNullable, bool?>>(node, l, r).Compile();

            Assert.AreEqual((bool?)true, compiled(new SlotToNullable(2), new SlotToNullable(2)));
            Assert.AreEqual((bool?)false, compiled(new SlotToNullable(2), new SlotToNullable(-2)));
        }

        [Test]
        public void UserDefinedToNullableEqualFromNullable()
        {
            Assert.Throws<InvalidOperationException>
            (
                () => Expression.Equal
                (
                    Expression.Parameter(typeof(SlotToNullable?), "l"),
                    Expression.Parameter(typeof(SlotToNullable?), "r")
                )
            );
        }

        public struct D
        {
            // Empty
        }

        private struct Slot
        {
            private readonly int _value;

            public Slot(int value)
            {
                _value = value;
            }

            public static bool operator !=(Slot a, Slot b)
            {
                return a._value != b._value;
            }

            public static bool operator ==(Slot a, Slot b)
            {
                return a._value == b._value;
            }

            public override bool Equals(object obj)
            {
                var other = obj as Slot?;
                return other?._value == _value;
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }

        private struct SlotToNullable
        {
            private readonly int _value;

            public SlotToNullable(int value)
            {
                _value = value;
            }

            public static bool? operator !=(SlotToNullable a, SlotToNullable b)
            {
                return a._value != b._value;
            }

            public static bool? operator ==(SlotToNullable a, SlotToNullable b)
            {
                return a._value == b._value;
            }

            public override bool Equals(object obj)
            {
                var other = obj as SlotToNullable?;
                return other?._value == _value;
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }
    }
}