﻿#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

#pragma warning disable RCS1079 // Throwing of new NotImplementedException

//
// ExpressionTest_ListInit.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using Theraot;

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestListInit
    {
        [Test]
        [Category("NotDotNet")]
        public void CompileArrayListOfStringsInit()
        {
            var add = typeof(ArrayList).GetMethod("Add");

            var compiled = Expression.Lambda<Func<ArrayList>>
            (
                Expression.ListInit
                (
                    Expression.New(typeof(ArrayList)),
                    Expression.ElementInit(add, "foo".ToConstant()),
                    Expression.ElementInit(add, "bar".ToConstant())
                )
            ).Compile();

            var list = compiled();

            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("foo", list[0]);
            Assert.AreEqual("bar", list[1]);
        }

        [Test]
        public void CompileListOfStringsInit()
        {
            var add = typeof(List<string>).GetMethod("Add");

            var compiled = Expression.Lambda<Func<List<string>>>
            (
                Expression.ListInit
                (
                    Expression.New(typeof(List<string>)),
                    Expression.ElementInit(add, "foo".ToConstant()),
                    Expression.ElementInit(add, "bar".ToConstant())
                )
            ).Compile();

            var list = compiled();

            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("foo", list[0]);
            Assert.AreEqual("bar", list[1]);
        }

        [Test]
        public void ExpressionTypeDoesNotHaveAddMethod()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.ListInit(Expression.New(typeof(Bar)), "foo".ToConstant()));
        }

        [Test]
        public void ExpressionTypeDoesNotImplementIEnumerable()
        {
            Assert.Throws<InvalidOperationException>(() => Expression.ListInit(Expression.New(typeof(Foo)), "foo".ToConstant()));
        }

        [Test]
        public void InitListOfStringWithConstants()
        {
            var li = Expression.ListInit
            (
                Expression.New(typeof(List<string>)),
                "foo".ToConstant(), "bar".ToConstant()
            );

            Assert.AreEqual(typeof(List<string>), li.Type);
            Assert.AreEqual(ExpressionType.ListInit, li.NodeType);
            Assert.AreEqual("new List`1() {Void Add(System.String)(\"foo\"), Void Add(System.String)(\"bar\")}", li.ToString());
        }

        [Test]
        public void InitListOfStringWithElementInitializers()
        {
            var li = Expression.ListInit
            (
                Expression.New(typeof(List<string>)),
                Expression.ElementInit
                (
                    typeof(List<string>).GetMethod("Add"),
                    "foo".ToConstant()
                ),
                Expression.ElementInit
                (
                    typeof(List<string>).GetMethod("Add"),
                    "bar".ToConstant()
                )
            );

            Assert.AreEqual(typeof(List<string>), li.Type);
            Assert.AreEqual(ExpressionType.ListInit, li.NodeType);
            Assert.AreEqual("new List`1() {Void Add(System.String)(\"foo\"), Void Add(System.String)(\"bar\")}", li.ToString());
        }

        [Test]
        public void NullElementInitializer()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ListInit(CreateNewList(), new ElementInit[] { null }));
        }

        [Test]
        public void NullExpression()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ListInit(null, new List<ElementInit>()));
        }

        [Test]
        public void NullExpressionInitializer()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.ListInit(CreateNewList(), new Expression[] { null }));
        }

        private static NewExpression CreateNewList()
        {
            return Expression.New(typeof(List<string>));
        }

        public class Bar : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public class Baz : Bar
        {
            public void Add(object a, object b)
            {
                No.Op(a);
                No.Op(b);
            }
        }

        public class Foo
        {
            // Empty
        }
    }
}