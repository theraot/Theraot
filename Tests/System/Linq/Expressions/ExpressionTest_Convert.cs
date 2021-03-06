﻿#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_Convert.cs
//
// Author:
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

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestConvert
    {
        private enum AnEnum
        {
            AValue
        }

        private interface IFoo
        {
            // Empty
        }

        private interface IZap
        {
            // Empty
        }

        [Test]
        public void BoxInt32()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(int), ""), typeof(object));
            Assert.AreEqual(typeof(object), c.Type);
            Assert.IsFalse(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNull(c.Method);
        }

        [Test]
        public void ChainedNullableConvert()
        {
            var p = Expression.Parameter(typeof(sbyte?), "a");

            var compiled = Expression.Lambda<Func<sbyte?, long?>>
            (
                Expression.Convert
                (
                    Expression.Convert
                    (
                        p,
                        typeof(int?)
                    ),
                    typeof(long?)
                ), p
            ).Compile();

            Assert.AreEqual((long?)3, compiled((sbyte?)3));
            Assert.AreEqual(null, compiled(null));
        }

        [Test]
        public void CompileConvertClassWithExplicitOp()
        {
            var p = Expression.Parameter(typeof(Klang), "klang");
            var compiled = Expression.Lambda<Func<Klang, int>>
            (
                Expression.Convert(p, typeof(int)), p
            ).Compile();

            Assert.AreEqual(42, compiled(new Klang(42)));
        }

        [Test]
        public void CompileConvertStructWithImplicitOp()
        {
            var p = Expression.Parameter(typeof(Kling), "kling");
            var compiled = Expression.Lambda<Func<Kling, int>>
            (
                Expression.Convert(p, typeof(int)), p
            ).Compile();

            Assert.AreEqual(42, compiled(new Kling(42)));
        }

        [Test]
        public void CompiledBoxing()
        {
            var compiled = Expression.Lambda<Func<object>>
            (
                Expression.Convert(42.ToConstant(), typeof(object))
            ).Compile();

            Assert.AreEqual(42, compiled());
        }

        [Test]
        public void CompiledCast()
        {
            var p = Expression.Parameter(typeof(IFoo), "foo");

            var compiled = Expression.Lambda<Func<IFoo, Bar>>
            (
                Expression.Convert(p, typeof(Bar)), p
            ).Compile();

            IFoo foo = new Bar();

            var b = compiled(foo);

            Assert.AreEqual(b, foo);
        }

        [Test]
        public void CompiledConvertNullableToNullable()
        {
            var p = Expression.Parameter(typeof(int?), "i");
            var compiled = Expression.Lambda<Func<int?, short?>>
            (
                Expression.Convert(p, typeof(short?)), p
            ).Compile();

            Assert.AreEqual(null, compiled(null));
            Assert.AreEqual((short?)12, compiled(12));
        }

        [Test]
        public void CompiledConvertToSameType()
        {
            var k = new Klang(42);

            var p = Expression.Parameter(typeof(Klang), "klang");
            var compiled = Expression.Lambda<Func<Klang, Klang>>
            (
                Expression.Convert
                (
                    p, typeof(Klang)
                ),
                p
            ).Compile();

            Assert.AreEqual(k, compiled(k));
        }

        [Test]
        public void CompiledNullableBoxing()
        {
            var p = Expression.Parameter(typeof(int?), "i");
            var compiled = Expression.Lambda<Func<int?, object>>
            (
                Expression.Convert(p, typeof(object)), p
            ).Compile();

            Assert.AreEqual(null, compiled(null));
            Assert.AreEqual((int?)42, compiled(42));
        }

        [Test]
        public void CompiledNullableUnboxing()
        {
            var p = Expression.Parameter(typeof(object), "o");
            var compiled = Expression.Lambda<Func<object, int?>>
            (
                Expression.Convert(p, typeof(int?)), p
            ).Compile();

            Assert.AreEqual(null, compiled(null));
            Assert.AreEqual((int?)42, compiled((int?)42));
        }

        [Test]
        public void CompiledUnBoxing()
        {
            var p = Expression.Parameter(typeof(object), "o");

            var compiled = Expression.Lambda<Func<object, int>>
            (
                Expression.Convert(p, typeof(int)), p
            ).Compile();

            Assert.AreEqual(42, compiled(42));
        }

        [Test]
        public void CompileNotNullableToNullable()
        {
            var p = Expression.Parameter(typeof(int), "i");
            var compiled = Expression.Lambda<Func<int, int?>>
            (
                Expression.Convert(p, typeof(int?)), p
            ).Compile();

            Assert.AreEqual((int?)0, compiled(0));
            Assert.AreEqual((int?)42, compiled(42));
        }

        [Test]
        public void CompileNullableToNotNullable()
        {
            var p = Expression.Parameter(typeof(int?), "i");
            var compiled = Expression.Lambda<Func<int?, int>>
            (
                Expression.Convert(p, typeof(int)), p
            ).Compile();

            Assert.AreEqual(0, compiled(0));
            Assert.AreEqual(42, compiled(42));

            Action a = () => compiled(null);

            a.AssertThrows(typeof(InvalidOperationException));
        }

        [Test]
        public void ConvertBackwardAssignability()
        {
            var c = Expression.Convert
            (
                Expression.Constant(null, typeof(Bar)), typeof(Foo)
            );
#if TARGETS_NETCORE
            // Expressions in .NET Core also output the types
            Assert.AreEqual("Convert(null, Foo)", c.ToString());
#else
            Assert.AreEqual("Convert(null)", c.ToString());
#endif
        }

        [Test]
        public void ConvertBazToFoo()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Convert(Expression.Parameter(typeof(Baz), ""), typeof(Foo)));
        }

        [Test]
        public void ConvertCheckedFallbackToConvertForNonPrimitives()
        {
            var p = Expression.ConvertChecked
            (
                Expression.Constant(null, typeof(object)), typeof(IFoo)
            );

            Assert.AreEqual(ExpressionType.Convert, p.NodeType);
        }

        [Test]
        public void ConvertCheckedInt32ToInt64()
        {
            var c = Expression.ConvertChecked
            (
                Expression.Constant(2, typeof(int)), typeof(long)
            );

            Assert.AreEqual(ExpressionType.ConvertChecked, c.NodeType);
#if TARGETS_NETCORE
            // Expressions in .NET Core also output the types
            Assert.AreEqual("ConvertChecked(2, Int64)", c.ToString());
#else
            Assert.AreEqual("ConvertChecked(2)", c.ToString());
#endif
        }

        [Test]
        public void ConvertCheckedNullableIntToInt()
        {
            var p = Expression.Parameter(typeof(int?), "i");

            var node = Expression.ConvertChecked(p, typeof(int));
            Assert.AreEqual(ExpressionType.ConvertChecked, node.NodeType);
            Assert.IsTrue(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(int), node.Type);
            Assert.IsNull(node.Method);
        }

        [Test]
        public void ConvertClassWithExplicitOp()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(Klang), ""), typeof(int));
            Assert.AreEqual(typeof(int), c.Type);
            Assert.IsFalse(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNotNull(c.Method);
        }

        [Test]
        public void ConvertClassWithExplicitOpToNullableInt()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(Klang), ""), typeof(int?));
            Assert.AreEqual(typeof(int?), c.Type);
            Assert.IsTrue(c.IsLifted);
            Assert.IsTrue(c.IsLiftedToNull);
            Assert.IsNotNull(c.Method);
        }

        [Test]
        public void ConvertEnumToInt32()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(AnEnum), ""), typeof(int));
            Assert.AreEqual(typeof(int), c.Type);
            Assert.IsFalse(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNull(c.Method);
        }

        [Test] // #678897
        public void ConvertEnumValueToEnum()
        {
            var node = Expression.Convert
            (
                Expression.Constant(AnEnum.AValue, typeof(AnEnum)),
                typeof(Enum)
            );

            Assert.IsNotNull(node);
            Assert.AreEqual(typeof(Enum), node.Type);
        }

        [Test]
        public void ConvertIFooToFoo()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(IFoo), ""), typeof(Foo));
            Assert.AreEqual(typeof(Foo), c.Type);
            Assert.IsFalse(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNull(c.Method);
        }

        [Test]
        public void ConvertImplicitToShortToNullableInt()
        {
            var a = Expression.Parameter(typeof(ImplicitToShort?), "a");

            var method = typeof(ImplicitToShort).GetMethod("op_Implicit");

            var node = Expression.Convert(a, typeof(short), method);
            Assert.IsTrue(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(short), node.Type);
            Assert.AreEqual(method, node.Method);

            var compiled = Expression.Lambda<Func<ImplicitToShort?, int?>>
            (
                Expression.Convert
                (
                    node,
                    typeof(int?)
                ), a
            ).Compile();

            Assert.AreEqual((int?)42, compiled(new ImplicitToShort(42)));

            Action convNull = () => Assert.AreEqual(null, compiled(null));

            convNull.AssertThrows(typeof(InvalidOperationException));
        }

        [Test]
        public void ConvertInt32ToBool()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Convert(Expression.Parameter(typeof(int), ""), typeof(bool)));
        }

        [Test]
        public void ConvertInt32ToInt64()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(int), ""), typeof(long));
            Assert.AreEqual(typeof(long), c.Type);
            Assert.IsFalse(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNull(c.Method);
        }

        [Test]
        public void ConvertInt32ToNullableInt32()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(int), ""), typeof(int?));
            Assert.AreEqual(typeof(int?), c.Type);
            Assert.IsTrue(c.IsLifted);
            Assert.IsTrue(c.IsLiftedToNull);
            Assert.IsNull(c.Method);
        }

        [Test]
        public void ConvertInt64ToInt32()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(long), ""), typeof(int));
            Assert.AreEqual(typeof(int), c.Type);
            Assert.IsFalse(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNull(c.Method);
        }

        [Test]
        public void ConvertInterfaces()
        {
            var p = Expression.Parameter(typeof(IFoo), null);

            var conv = Expression.Convert(p, typeof(IZap));
            Assert.AreEqual(typeof(IZap), conv.Type);
            p = Expression.Parameter(typeof(IZap), null);
            conv = Expression.Convert(p, typeof(IFoo));

            Assert.AreEqual(typeof(IFoo), conv.Type);
        }

        [Test]
        public void ConvertIntToString()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Convert(1.ToConstant(), typeof(string)));
        }

        [Test]
        public void ConvertLongToDecimal()
        {
            var p = Expression.Parameter(typeof(long), "l");

            var node = Expression.Convert(p, typeof(decimal));
            Assert.IsFalse(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(decimal), node.Type);
            Assert.IsNotNull(node.Method);

            var compiled = Expression.Lambda<Func<long, decimal>>(node, p).Compile();

            Assert.AreEqual(42, compiled(42));
        }

        [Test]
        public void ConvertNullableImplicitToIntToNullableLong()
        {
            var i = Expression.Parameter(typeof(ImplicitToInt?), "i");

            var method = typeof(ImplicitToInt).GetMethod("op_Implicit");

            var node = Expression.Convert(i, typeof(int), method);
            node = Expression.Convert(node, typeof(long?));
            var compiled = Expression.Lambda<Func<ImplicitToInt?, long?>>(node, i).Compile();

            Assert.AreEqual((long?)42, compiled(new ImplicitToInt(42)));
            Action convNull = () => Assert.AreEqual(null, compiled(null));
            convNull.AssertThrows(typeof(InvalidOperationException));
        }

        [Test]
        public void ConvertNullableInt32ToInt32()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(int?), ""), typeof(int));
            Assert.AreEqual(typeof(int), c.Type);
            Assert.IsTrue(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNull(c.Method);
        }

        [Test]
        public void ConvertNullableIntToStringWithConvertMethod()
        {
            Assert.Throws<InvalidOperationException>
            (
                () => Expression.Convert
                (
                    Expression.Constant((int?)0),
                    typeof(string),
                    typeof(Convert).GetMethod("ToString", new[] { typeof(object) })
                )
            );
        }

        [Test]
        public void ConvertNullableStructWithImplicitOpToNullableInt()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(Kling?), ""), typeof(int?));
            Assert.AreEqual(typeof(int?), c.Type);
            Assert.IsTrue(c.IsLifted);
            Assert.IsTrue(c.IsLiftedToNull);
            Assert.IsNotNull(c.Method);
        }

        [Test]
        public void ConvertNullableULongToNullableDecimal()
        {
            var p = Expression.Parameter(typeof(ulong?), "l");

            var node = Expression.Convert(p, typeof(decimal?));
            Assert.IsTrue(node.IsLifted);
            Assert.IsTrue(node.IsLiftedToNull);
            Assert.AreEqual(typeof(decimal?), node.Type);
            Assert.IsNotNull(node.Method);

            var compiled = Expression.Lambda<Func<ulong?, decimal?>>(node, p).Compile();

            Assert.AreEqual(42, compiled(42));
            Assert.AreEqual(null, compiled(null));
        }

        [Test]
        public void ConvertStructToFoo()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Convert(Expression.Parameter(typeof(AStruct), ""), typeof(Foo)));
        }

        [Test]
        public void ConvertStructWithImplicitOp()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(Kling), ""), typeof(int));
            Assert.AreEqual(typeof(int), c.Type);
            Assert.IsFalse(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNotNull(c.Method);
        }

        [Test]
        public void ConvertStructWithImplicitOpToNullableInt()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(Kling), ""), typeof(int?));
            Assert.AreEqual(typeof(int?), c.Type);
            Assert.IsTrue(c.IsLifted);
            Assert.IsTrue(c.IsLiftedToNull);
            Assert.IsNotNull(c.Method);
        }

        [Test]
        public void NullableImplicitToShort()
        {
            var i = Expression.Parameter(typeof(ImplicitToShort?), "i");

            var method = typeof(ImplicitToShort).GetMethod("op_Implicit");

            var node = Expression.Convert(i, typeof(short?), method);

            Assert.IsTrue(node.IsLifted);
            Assert.IsTrue(node.IsLiftedToNull);
            Assert.AreEqual(typeof(short?), node.Type);
            Assert.AreEqual(method, node.Method);

            var compiled = Expression.Lambda<Func<ImplicitToShort?, short?>>(node, i).Compile();

            Assert.AreEqual((short?)42, compiled(new ImplicitToShort(42)));
        }

        [Test]
        public void NullExpression()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Convert(null, typeof(int)));
        }

        [Test]
        public void NullType()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Convert(1.ToConstant(), null));
        }

        [Test]
        public void UnBoxInt32()
        {
            var c = Expression.Convert(Expression.Parameter(typeof(object), ""), typeof(int));
            Assert.AreEqual(typeof(int), c.Type);
            Assert.IsFalse(c.IsLifted);
            Assert.IsFalse(c.IsLiftedToNull);
            Assert.IsNull(c.Method);
        }

        private struct AStruct
        {
            // Empty
        }

        private struct ImplicitToInt
        {
            private readonly int _value;

            public ImplicitToInt(int v)
            {
                _value = v;
            }

            public static implicit operator int(ImplicitToInt i)
            {
                return i._value;
            }
        }

        private struct ImplicitToShort
        {
            private readonly short _value;

            public ImplicitToShort(short v)
            {
                _value = v;
            }

            public static implicit operator short(ImplicitToShort i)
            {
                return i._value;
            }
        }

        private struct Kling
        {
            private readonly int _i;

            public Kling(int i)
            {
                _i = i;
            }

            public static implicit operator int(Kling k)
            {
                return k._i;
            }
        }

        private sealed class Bar : Foo
        {
            // Empty
        }

        private sealed class Baz
        {
            // Empty
        }

        private class Foo : IFoo
        {
            // Empty
        }

        private sealed class Klang
        {
            private readonly int _i;

            public Klang(int i)
            {
                _i = i;
            }

            public static explicit operator int(Klang k)
            {
                return k._i;
            }
        }
    }
}