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

using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestPropertyOrField
    {
        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.PropertyOrField(null, "NoPropertyOrField"));
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.PropertyOrField(Expression.Constant(new MemberClass()), null));
        }

        [Test]
        public void NoPropertyOrField()
        {
            Assert.Throws<ArgumentException>(() => Expression.PropertyOrField(Expression.Constant(new MemberClass()), "NoPropertyOrField"));
        }

        [Test]
        public void InstanceProperty()
        {
            var expr = Expression.PropertyOrField(Expression.Constant(new MemberClass()), "TestProperty1");
            Assert.AreEqual(ExpressionType.MemberAccess, expr.NodeType, "PropertyOrField#01");
            Assert.AreEqual(typeof(int), expr.Type, "PropertyOrField#02");
            Assert.AreEqual("value(MonoTests.System.Linq.Expressions.MemberClass).TestProperty1", expr.ToString(), "PropertyOrField#04");
        }
    }
}