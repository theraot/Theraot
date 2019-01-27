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

using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestConstant
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Arg2NotNullable()
        {
            Expression.Constant(null, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Arg2Null()
        {
            Expression.Constant(1, null);
        }

        [Test]
        public void NullValue()
        {
            var expr = Expression.Constant(null);
            Assert.AreEqual(ExpressionType.Constant, expr.NodeType, "Constant#01");
            Assert.IsNull(expr.Value, "Constant#02");
            Assert.AreEqual(typeof(object), expr.Type, "Constant#03");
            Assert.AreEqual("null", expr.ToString(), "Constant#04");
        }

        [Test]
        public void NullableValue1()
        {
            var expr = Expression.Constant(null, typeof(int?));
            Assert.AreEqual(ExpressionType.Constant, expr.NodeType, "Constant#05");
            Assert.IsNull(expr.Value, "Constant#06");
            Assert.AreEqual(typeof(int?), expr.Type, "Constant#07");
            Assert.AreEqual("null", expr.ToString(), "Constant#08");
        }

        [Test]
        public void NullableValue2()
        {
            var expr = Expression.Constant(1, typeof(int?));
            Assert.AreEqual(ExpressionType.Constant, expr.NodeType, "Constant#09");
            Assert.AreEqual(1, expr.Value, "Constant#10");
            Assert.AreEqual(typeof(int?), expr.Type, "Constant#11");
            Assert.AreEqual("1", expr.ToString(), "Constant#12");
        }

        [Test]
        public void NullableValue3()
        {
            var expr = Expression.Constant((int?)1);
            Assert.AreEqual(ExpressionType.Constant, expr.NodeType, "Constant#13");
            Assert.AreEqual(1, expr.Value, "Constant#14");
            Assert.AreEqual(typeof(int), expr.Type, "Constant#15");
            Assert.AreEqual("1", expr.ToString(), "Constant#16");
        }

        [Test]
        public void IntegerValue()
        {
            var expr = Expression.Constant(0);
            Assert.AreEqual(ExpressionType.Constant, expr.NodeType, "Constant#17");
            Assert.AreEqual(0, expr.Value, "Constant#18");
            Assert.AreEqual(typeof(int), expr.Type, "Constant#19");
            Assert.AreEqual("0", expr.ToString(), "Constant#20");
        }

        [Test]
        public void StringValue()
        {
            var expr = Expression.Constant("a string");
            Assert.AreEqual(ExpressionType.Constant, expr.NodeType, "Constant#21");
            Assert.AreEqual("a string", expr.Value, "Constant#22");
            Assert.AreEqual(typeof(string), expr.Type, "Constant#23");
            Assert.AreEqual("\"a string\"", expr.ToString(), "Constant#24");
        }

        [Test]
        public void DateTimeValue()
        {
            var expr = Expression.Constant(new DateTime(1971, 10, 19));
            Assert.AreEqual(ExpressionType.Constant, expr.NodeType, "Constant#25");
            Assert.AreEqual(new DateTime(1971, 10, 19), expr.Value, "Constant#26");
            Assert.AreEqual(typeof(DateTime), expr.Type, "Constant#27");
            // This test must be done under the assumption that both "ToString" happen on the same culture
            Assert.AreEqual(new DateTime(1971, 10, 19).ToString(), expr.ToString(), "Constant#28");
        }

        [Test]
        public void UserClassValue()
        {
            var oc = new OpClass();
            var expr = Expression.Constant(oc);
            Assert.AreEqual(ExpressionType.Constant, expr.NodeType, "Constant#29");
            Assert.AreEqual(oc, expr.Value, "Constant#30");
            Assert.AreEqual(typeof(OpClass), expr.Type, "Constant#31");
            Assert.AreEqual("value(MonoTests.System.Linq.Expressions.OpClass)", expr.ToString(), "Constant#32");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidCtor_1()
        {
            // null value, type == valuetype is invalid
            Expression.Constant(null, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidCtor_2()
        {
            // type mismatch: int value, type == double
            Expression.Constant(0, typeof(double));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void VoidConstant()
        {
            Expression.Constant(null, typeof(void));
        }

        private static T Check<T>(T val)
        {
            var l = Expression.Lambda<Func<T>>(Expression.Constant(val), new ParameterExpression[0]);
            var fi = l.Compile();
            return fi();
        }

        [Test]
        public void NullableConstant_ToConstant()
        {
            int? a = 1;
            var c = Expression.Constant(a);
            Assert.AreEqual(typeof(int), c.Type, "#1");
            Assert.AreEqual(1, c.Value, "#2");
        }

        [Test]
        public void ConstantCodeGen()
        {
            Assert.AreEqual(Check(0), 0, "int");
            Assert.AreEqual(Check(128), 128, "int2");
            Assert.AreEqual(Check(-128), -128, "int3");
            Assert.AreEqual(Check(int.MinValue), int.MinValue, "int4");
            Assert.AreEqual(Check(int.MaxValue), int.MaxValue, "int5");
            Assert.AreEqual(Check<uint>(128), 128, "uint");
            Assert.AreEqual(Check<uint>(0), 0, "uint2");
            Assert.AreEqual(Check(uint.MinValue), uint.MinValue, "uint3");
            Assert.AreEqual(Check(uint.MaxValue), uint.MaxValue, "uint4");
            Assert.AreEqual(Check<byte>(10), 10, "byte");
            Assert.AreEqual(Check(byte.MinValue), byte.MinValue, "byte2");
            Assert.AreEqual(Check(byte.MaxValue), byte.MaxValue, "byte3");
            Assert.AreEqual(Check<short>(128), 128, "short");
            Assert.AreEqual(Check<short>(-128), -128, "short");
            Assert.AreEqual(Check(short.MinValue), short.MinValue, "short2");
            Assert.AreEqual(Check(short.MaxValue), short.MaxValue, "short3");
            Assert.AreEqual(Check<ushort>(128), 128, "ushort");
            Assert.AreEqual(Check(ushort.MinValue), ushort.MinValue, "short2");
            Assert.AreEqual(Check(ushort.MaxValue), ushort.MaxValue, "short3");
            Assert.AreEqual(Check(true), true, "bool1");
            Assert.AreEqual(Check(false), false, "bool2");
            Assert.AreEqual(Check(long.MaxValue), long.MaxValue, "long");
            Assert.AreEqual(Check(long.MinValue), long.MinValue, "long2");
            Assert.AreEqual(Check(ulong.MaxValue), ulong.MaxValue, "ulong");
            Assert.AreEqual(Check(ulong.MinValue), ulong.MinValue, "ulong2");
            Assert.AreEqual(Check<ushort>(200), 200, "ushort");
            Assert.AreEqual(Check(2.0f), 2.0f, "float");
            Assert.AreEqual(Check(2.312), 2.312, "double");
            Assert.AreEqual(Check("dingus"), "dingus", "string");
            Assert.AreEqual(Check(1.3m), 1.3m, "");

            // this forces the other code path for decimal.
            Assert.AreEqual(Check(3147483647m), 3147483647m, "decimal");
        }

        private delegate void Foo();

        [Test]
        public void DelegateTypeConstant()
        {
            Expression.Constant(typeof(Foo), typeof(Type));
        }

        [Test]
        public void EmitDateTimeConstant()
        {
            var date = new DateTime(1983, 2, 6);

            var lambda = Expression.Lambda<Func<DateTime>>(Expression.Constant(date)).Compile();

            Assert.AreEqual(date, lambda());
        }

        [Test]
        public void EmitDbNullConstant()
        {
            var lambda = Expression.Lambda<Func<DBNull>>(Expression.Constant(DBNull.Value)).Compile();

            Assert.AreEqual(DBNull.Value, lambda());
        }

        [Test]
        public void EmitNullString()
        {
            var n = Expression.Lambda<Func<string>>(
                Expression.Constant(null, typeof(string))).Compile();

            Assert.IsNull(n());
        }

        [Test]
        public void EmitNullNullableType()
        {
            var n = Expression.Lambda<Func<int?>>(
                Expression.Constant(null, typeof(int?))).Compile();

            Assert.IsNull(n());
        }

        [Test]
        public void EmitNullableInt()
        {
            var i = Expression.Lambda<Func<int?>>(
                Expression.Constant((int?)42, typeof(int?))).Compile();

            Assert.AreEqual((int?)42, i());
        }

        [Test]
        public void EmitNullableEnum()
        {
            var e = Expression.Lambda<Func<Chose?>>(
                Expression.Constant((Chose?)Chose.Moche, typeof(Chose?))).Compile();

            Assert.AreEqual((Chose?)Chose.Moche, e());
        }

        private enum Chose
        {
            Moche
        }

        private interface IBar
        {
            // Empty
        }

        private class Bar : IBar
        {
            // Empty
        }

        private interface IBaz<T>
        {
            // Empty
        }

        private class Baz<T> : IBaz<T>
        {
            // Empty
        }

        [Test]
        public void ConstantInterface()
        {
            var c = Expression.Constant(new Bar(), typeof(IBar));
            Assert.AreEqual(typeof(IBar), c.Type);
        }

        [Test]
        public void ConstantGenericInterface()
        {
            var c = Expression.Constant(new Baz<string>(), typeof(IBaz<string>));
            Assert.AreEqual(typeof(IBaz<string>), c.Type);
        }
    }
}