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
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestBind
    {
        private struct Slot
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Integer { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public short Short { get; set; }
        }

        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Bind(null, Expression.Constant(1)));
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Bind(typeof(MemberClass).GetField(nameof(MemberClass.TestField2)), null));
        }

        [Test]
        public void BindValueTypes()
        {
            var i = Expression.Parameter(typeof(int), "i");
            var s = Expression.Parameter(typeof(short), "s");

            var memberInitExpression = Expression.MemberInit
            (
                Expression.New(typeof(Slot)),
                Expression.Bind(typeof(Slot).GetProperty("Integer"), i),
                Expression.Bind(typeof(Slot).GetProperty("Short"), s)
            );
            var compiled = Expression.Lambda<Func<int, short, Slot>>(memberInitExpression, i, s).Compile();

            Assert.AreEqual(new Slot {Integer = 42, Short = -1}, compiled(42, -1));
        }

        [Test]
        public void Event()
        {
            Assert.Throws<ArgumentException>(() => Expression.Bind(MemberClass.GetEventInfo(), Expression.Constant(1)));
        }

        [Test]
        public void FieldRo()
        {
            var expr = Expression.Bind(typeof(MemberClass).GetField(nameof(MemberClass.TestField1)), Expression.Constant(1));
            Assert.AreEqual(MemberBindingType.Assignment, expr.BindingType, "Bind#01");
            Assert.AreEqual($"{nameof(MemberClass.TestField1)} = 1", expr.ToString(), "Bind#02");
        }

        [Test]
        public void FieldRw()
        {
            var expr = Expression.Bind(typeof(MemberClass).GetField(nameof(MemberClass.TestField2)), Expression.Constant(1));
            Assert.AreEqual(MemberBindingType.Assignment, expr.BindingType, "Bind#03");
            Assert.AreEqual($"{nameof(MemberClass.TestField2)} = 1", expr.ToString(), "Bind#04");
        }

        [Test]
        public void FieldStatic()
        {
            var expr = Expression.Bind(MemberClass.GetStaticFieldInfo(), Expression.Constant(1));
            Assert.AreEqual(MemberBindingType.Assignment, expr.BindingType, "Bind#05");
            Assert.AreEqual("StaticField = 1", expr.ToString(), "Bind#06");
        }

        [Test]
        public void Method1()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // This tests the MethodInfo version of Bind(): should raise an exception
                    // because the method is not an accessor.
                    Expression.Bind(typeof(MemberClass).GetMethod(nameof(MemberClass.TestMethod)), Expression.Constant(1));
                }
            );
        }

        [Test]
        public void Method2()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    // This tests the MemberInfo version of Bind(): should raise an exception
                    // because the argument is not a field or property accessor.
                    Expression.Bind((MemberInfo)typeof(MemberClass).GetMethod(nameof(MemberClass.TestMethod)), Expression.Constant(1));
                }
            );
        }

        [Test]
        public void PropertyAccessor()
        {
            var mi = typeof(MemberClass).GetMethod("get_TestProperty2");

            var expr = Expression.Bind(mi, Expression.Constant(1));
            Assert.AreEqual(MemberBindingType.Assignment, expr.BindingType, "Bind#11");
            Assert.AreEqual("TestProperty2 = 1", expr.ToString(), "Bind#12");
            Assert.AreEqual(MemberClass.GetRwPropertyInfo(), expr.Member, "Bind#13");
        }

        [Test]
        public void PropertyAccessorStatic()
        {
            var mi = typeof(MemberClass).GetMethod("get_StaticProperty");

            var expr = Expression.Bind(mi, Expression.Constant(1));
            Assert.AreEqual(MemberBindingType.Assignment, expr.BindingType, "Bind#14");
            Assert.AreEqual("StaticProperty = 1", expr.ToString(), "Bind#15");
            Assert.AreEqual(MemberClass.GetStaticPropertyInfo(), expr.Member, "Bind#16");
        }

        [Test]
        public void PropertyRo()
        {
            Assert.Throws<ArgumentException>(() => Expression.Bind(typeof(MemberClass).GetProperty(nameof(MemberClass.TestProperty1)), Expression.Constant(1)));
        }

        [Test]
        public void PropertyRw()
        {
            var expr = Expression.Bind(MemberClass.GetRwPropertyInfo(), Expression.Constant(1));
            Assert.AreEqual(MemberBindingType.Assignment, expr.BindingType, "Bind#07");
            Assert.AreEqual("TestProperty2 = 1", expr.ToString(), "Bind#08");
        }

        [Test]
        public void PropertyStatic()
        {
            var expr = Expression.Bind(MemberClass.GetStaticPropertyInfo(), Expression.Constant(1));
            Assert.AreEqual(MemberBindingType.Assignment, expr.BindingType, "Bind#09");
            Assert.AreEqual("StaticProperty = 1", expr.ToString(), "Bind#10");
        }
    }
}