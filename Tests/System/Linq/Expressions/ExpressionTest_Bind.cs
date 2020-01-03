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
//      Federico Di Gregorio <fog@initd.org>

using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Tests.Helpers;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestBind
    {
        [Test]
        public void Arg1Null()
        {
            const int value = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.Bind(null, Expression.Constant(value)));
        }

        [Test]
        public void Arg2Null()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentNullException>(() => Expression.Bind(typeof(MemberClass).GetField(nameof(MemberClass.TestField2)), null));
        }

        [Test]
        public void BindValueTypes()
        {
            const string nameLeft = "i";
            const string nameRight = "s";

            const int left = 42;
            const short right = -1;

            var parameterLeft = Expression.Parameter(typeof(int), nameLeft);
            var parameterRight = Expression.Parameter(typeof(short), nameRight);

            var memberInitExpression = Expression.MemberInit
            (
                Expression.New(typeof(Slot)),
                // ReSharper disable once AssignNullToNotNullAttribute
                Expression.Bind(typeof(Slot).GetProperty(nameof(Slot.Integer)), parameterLeft),
                // ReSharper disable once AssignNullToNotNullAttribute
                Expression.Bind(typeof(Slot).GetProperty(nameof(Slot.Short)), parameterRight)
            );
            var compiled = Expression.Lambda<Func<int, short, Slot>>(memberInitExpression, parameterLeft, parameterRight).Compile();

            Assert.AreEqual
            (
                new Slot
                {
                    Integer = left,
                    Short = right
                },
                compiled(left, right)
            );
        }

        [Test]
        public void Event()
        {
            AssertEx.Throws<ArgumentException>(() => Expression.Bind(MemberClass.GetEventInfo(), Expression.Constant(1)));
        }

        [Test]
        public void FieldRo()
        {
            const int value = 1;

            var expression = Expression.Bind(typeof(MemberClass).GetField(nameof(MemberClass.TestField1)), Expression.Constant(value));
            Assert.AreEqual(MemberBindingType.Assignment, expression.BindingType, "Bind#01");
            Assert.AreEqual($"{nameof(MemberClass.TestField1)} = {value}", expression.ToString(), "Bind#02");
        }

        [Test]
        public void FieldRw()
        {
            const int value = 1;

            var expression = Expression.Bind(typeof(MemberClass).GetField(nameof(MemberClass.TestField2)), Expression.Constant(value));
            Assert.AreEqual(MemberBindingType.Assignment, expression.BindingType, "Bind#03");
            Assert.AreEqual($"{nameof(MemberClass.TestField2)} = {value}", expression.ToString(), "Bind#04");
        }

        [Test]
        public void FieldStatic()
        {
            const int value = 1;

            var expression = Expression.Bind(typeof(MemberClass).GetField(nameof(MemberClass.StaticField)), Expression.Constant(value));
            Assert.AreEqual(MemberBindingType.Assignment, expression.BindingType, "Bind#05");
            Assert.AreEqual($"{nameof(MemberClass.StaticField)} = {value}", expression.ToString(), "Bind#06");
        }

        [Test]
        public void Method()
        {
            const int value = 1;

            // This tests the MethodInfo version of Bind(): should raise an exception
            // because the argument is not a field or property accessor.

            AssertEx.Throws<ArgumentException>(() => Expression.Bind(new Func<int, int>(new MemberClass().TestMethod).GetMethodInfo(), Expression.Constant(value)));
        }

        [Test]
        public void PropertyAccessor()
        {
            const int value = 1;
            var method = typeof(MemberClass).GetMethod($"get_{nameof(MemberClass.TestProperty2)}");

            // ReSharper disable once AssignNullToNotNullAttribute
            var expression = Expression.Bind(method, Expression.Constant(value));
            Assert.AreEqual(MemberBindingType.Assignment, expression.BindingType, "Bind#11");
            Assert.AreEqual($"{nameof(MemberClass.TestProperty2)} = {value}", expression.ToString(), "Bind#12");
            Assert.AreEqual(typeof(MemberClass).GetProperty(nameof(MemberClass.TestProperty2)), expression.Member, "Bind#13");
        }

        [Test]
        public void PropertyAccessorStatic()
        {
            const int value = 1;
            var method = typeof(MemberClass).GetMethod($"get_{nameof(MemberClass.StaticProperty)}");

            // ReSharper disable once AssignNullToNotNullAttribute
            var expression = Expression.Bind(method, Expression.Constant(value));
            Assert.AreEqual(MemberBindingType.Assignment, expression.BindingType, "Bind#14");
            Assert.AreEqual($"{nameof(MemberClass.StaticProperty)} = {value}", expression.ToString(), "Bind#15");
            Assert.AreEqual(typeof(MemberClass).GetProperty(nameof(MemberClass.StaticProperty)), expression.Member, "Bind#16");
        }

        [Test]
        public void PropertyRo()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            AssertEx.Throws<ArgumentException>(() => Expression.Bind(typeof(MemberClass).GetProperty(nameof(MemberClass.TestProperty1)), Expression.Constant(1)));
        }

        [Test]
        public void PropertyRw()
        {
            const int value = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            var expression = Expression.Bind(typeof(MemberClass).GetProperty(nameof(MemberClass.TestProperty2)), Expression.Constant(value));
            Assert.AreEqual(MemberBindingType.Assignment, expression.BindingType, "Bind#07");
            Assert.AreEqual($"{nameof(MemberClass.TestProperty2)} = {value}", expression.ToString(), "Bind#08");
        }

        [Test]
        public void PropertyStatic()
        {
            const int value = 1;

            // ReSharper disable once AssignNullToNotNullAttribute
            var expression = Expression.Bind(typeof(MemberClass).GetProperty(nameof(MemberClass.StaticProperty)), Expression.Constant(value));
            Assert.AreEqual(MemberBindingType.Assignment, expression.BindingType, "Bind#09");
            Assert.AreEqual($"{nameof(MemberClass.StaticProperty)} = {value}", expression.ToString(), "Bind#10");
        }

        private struct Slot
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Integer { get; set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public short Short { get; set; }
        }
    }
}