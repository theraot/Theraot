#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_ElementInit.cs
//
// Author:
//   olivier Dufour (olivier.duff@gmail.com)
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
using System.Reflection;
using Theraot;
using NUnit.Framework;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestElementInit
    {
        public class Foo
        {
            public void Add(string s)
            {
                No.Op(s);
            }

            public void Baz()
            {
            }
        }

        public static class Bar
        {
            public static void Add()
            {
            }
        }

        [Test]
        public void AddMethodIsNotAnInstanceMethod()
        {
            Assert.Throws<ArgumentException>(() => Expression.ElementInit(typeof(Bar).GetMethod("Add")));
        }

        [Test]
        public void ArgNull()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ElementInit(typeof(Foo).GetMethod("Add"), null));
        }

        [Test]
        public void ElementInitToString()
        {
            var elementInit = Expression.ElementInit(typeof(Foo).GetMethod("Add"), Expression.Constant(""));

            Assert.AreEqual("Void Add(System.String)(\"\")", elementInit.ToString());
        }

        [Test]
        public void MethodArgumentCountDoesnMatchParameterLength()
        {
            Assert.Throws<ArgumentException>(() => Expression.ElementInit(typeof(Foo).GetMethod("Add")));
        }

        [Test]
        public void MethodArgumentDoesntMatchParameterType()
        {
            Assert.Throws<ArgumentException>(() => Expression.ElementInit(typeof(Foo).GetMethod("Add"), Expression.Constant(1)));
        }

        [Test]
        public void MethodHasNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ElementInit(typeof(Foo).GetMethod("Add"), new Expression[] {null}));
        }

        [Test]
        public void MethodNameDoesntMatchAdd()
        {
            Assert.Throws<ArgumentException>(() => Expression.ElementInit(typeof(Foo).GetMethod("Baz")));
        }

        [Test]
        public void MethodNull()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ElementInit(null));
        }
    }
}