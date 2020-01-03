#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_New.cs
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
using System.Reflection;
using NUnit.Framework;
using Theraot;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestNew
    {
        [Test]
        public void CompileNewClass()
        {
            // TODO: split
            var p = Expression.Parameter(typeof(string), "p");
            var n = Expression.New(typeof(Gazonk).GetConstructor(new[] { typeof(string) }), p);
            var f = Expression.Lambda<Func<string, Gazonk>>(n, p).Compile();

            var g1 = new Gazonk("foo");
            var g2 = new Gazonk("bar");

            Assert.IsNotNull(g1);
            Assert.AreEqual(g1, f("foo"));
            Assert.IsNotNull(g2);
            Assert.AreEqual(g2, f("bar"));

            n = Expression.New(typeof(Bar));
            var l = Expression.Lambda<Func<Bar>>(n).Compile();

            var bar = l();

            Assert.IsNotNull(bar);
            Assert.IsNull(bar.Value);
        }

        [Test]
        public void CompileNewClassEmptyConstructor()
        {
            var compiled = Expression.Lambda<Func<AClass>>
            (
                Expression.New(typeof(AClass))
            ).Compile();

            var k = compiled();
            Assert.IsNull(k.Left);
            Assert.IsNull(k.Right);
        }

        [Test]
        public void CompileNewClassWithParameters()
        {
            var pl = Expression.Parameter(typeof(string), "left");
            var pr = Expression.Parameter(typeof(string), "right");

            var compiled = Expression.Lambda<Func<string, string, AClass>>
            (
                Expression.New(typeof(AClass).GetConstructor(new[] { typeof(string), typeof(string) }), pl, pr), pl, pr
            ).Compile();

            var k = compiled("foo", "bar");

            Assert.AreEqual("foo", k.Left);
            Assert.AreEqual("bar", k.Right);
        }

        [Test]
        public void CompileNewStruct()
        {
            var compiled = Expression.Lambda<Func<AStruct>>
            (
                Expression.New(typeof(AStruct))
            ).Compile();

            var s = compiled();
            Assert.AreEqual(0, s.Left);
            Assert.AreEqual(0, s.Right);
        }

        [Test]
        public void CompileNewStructWithParameters()
        {
            var pl = Expression.Parameter(typeof(int), "left");
            var pr = Expression.Parameter(typeof(int), "right");

            var compiled = Expression.Lambda<Func<int, int, AStruct>>
            (
                Expression.New(typeof(AStruct).GetConstructor(new[] { typeof(int), typeof(int) }), pl, pr), pl, pr
            ).Compile();

            var s = compiled(42, 12);

            Assert.AreEqual(42, s.Left);
            Assert.AreEqual(12, s.Right);
        }

        [Test]
        public void ConstructorHasTooMuchParameters()
        {
            Assert.Throws<ArgumentException>(() => Expression.New(typeof(Foo).GetConstructor(new[] { typeof(string) })));
        }

        [Test]
        public void HasNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.New(typeof(Foo).GetConstructor(new[] { typeof(string) }), null as Expression));
        }

        [Test]
        public void HasWrongArgument()
        {
            Assert.Throws<ArgumentException>(() => Expression.New(typeof(Foo).GetConstructor(new[] { typeof(string) }), Expression.Constant(12)));
        }

        [Test]
        public void MemberArgumentMiscount()
        {
            Assert.Throws<ArgumentException>
            (
                () => Expression.New
                (
                    typeof(FakeAnonymousType).GetConstructor(new[] { typeof(string) }),
                    new[] { "FooValue".ToConstant() }, typeof(FakeAnonymousType).GetProperty(nameof(FakeAnonymousType.FooValue)), typeof(FakeAnonymousType).GetProperty("BarValue")
                )
            );
        }

        [Test]
        public void MemberArgumentMismatch()
        {
            Assert.Throws<ArgumentException>
            (
                () => Expression.New
                (
                    typeof(FakeAnonymousType).GetConstructor(new[] { typeof(string) }),
                    new[] { "FooValue".ToConstant() }, typeof(FakeAnonymousType).GetProperty(nameof(FakeAnonymousType.GazonkValue))
                )
            );
        }

        [Test]
        public void MemberHasNoGetter()
        {
            Assert.Throws<ArgumentException>
            (
                () => Expression.New
                (
                    typeof(FakeAnonymousType).GetConstructor(new[] { typeof(string) }),
                    new[] { "FooValue".ToConstant() }, typeof(FakeAnonymousType).GetProperty("Zap")
                )
            );
        }

        [Test]
        public void NewBar()
        {
            var n = Expression.New(typeof(Bar));

            Assert.IsNotNull(n.Constructor);
            Assert.IsNotNull(n.Arguments);
            Assert.IsNull(n.Members); // wrong doc

            Assert.AreEqual("new Bar()", n.ToString());

            n = Expression.New(typeof(Bar).GetConstructor(ArrayEx.Empty<Type>()));

            Assert.AreEqual("new Bar()", n.ToString());
        }

        [Test]
        public void NewFakeAnonymousType()
        {
            var n = Expression.New
            (
                typeof(FakeAnonymousType).GetConstructor(new[] { typeof(string), typeof(string), typeof(string) }),
                new[] { "FooValue".ToConstant(), "BarValue".ToConstant(), "BazValue".ToConstant() }, typeof(FakeAnonymousType).GetProperty("FooValue"), typeof(FakeAnonymousType).GetProperty("BarValue"), typeof(FakeAnonymousType).GetProperty("BazValue")
            );

            Assert.IsNotNull(n.Constructor);
            Assert.IsNotNull(n.Arguments);
            Assert.IsNotNull(n.Members);
            Assert.AreEqual("new FakeAnonymousType(FooValue = \"FooValue\", BarValue = \"BarValue\", BazValue = \"BazValue\")", n.ToString());
        }

        [Test]
        public void NewFoo()
        {
            var n = Expression.New(typeof(Foo).GetConstructor(new[] { typeof(string) }), Expression.Constant("foo"));

            Assert.AreEqual(ExpressionType.New, n.NodeType);
            Assert.AreEqual(typeof(Foo), n.Type);
            Assert.AreEqual(1, n.Arguments.Count);
            Assert.IsNull(n.Members);
            Assert.AreEqual("new Foo(\"foo\")", n.ToString());
        }

        [Test]
        [Category("NotDotNet")]
        public void NewVoid()
        {
            Assert.Throws<ArgumentException>(() => Expression.New(typeof(void)));
        }

        [Test]
        public void NoParameterlessConstructor()
        {
            Assert.Throws<ArgumentException>(() => Expression.New(typeof(Foo)));
        }

        [Test]
        public void NullConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.New(null as ConstructorInfo));
        }

        [Test]
        public void NullMember()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Expression.New
                (
                    typeof(FakeAnonymousType).GetConstructor(new[] { typeof(string) }),
                    new[] { "FooValue".ToConstant() },
                    new MemberInfo[] { null }
                )
            );
        }

        [Test]
        public void NullType()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.New(null as Type));
        }

        public struct AStruct
        {
            public int Left;
            public int Right;

            public AStruct(int left, int right)
            {
                Left = left;
                Right = right;
            }
        }

        public struct Baz
        {
            // Empty
        }

        public class AClass
        {
            public AClass()
            {
                // Empty
            }

            public AClass(string l, string r)
            {
                Left = l;
                Right = r;
            }

            public string Left { get; set; }

            public string Right { get; set; }
        }

        public class Bar
        {
            public string Value { get; set; }
        }

        public class FakeAnonymousType
        {
            public FakeAnonymousType(string foo)
            {
                FooValue = foo;
            }

            public FakeAnonymousType(string foo, string bar, string baz)
            {
                FooValue = foo;
                BarValue = bar;
                BazValue = baz;
            }

            public string BarValue { get; set; }
            public string BazValue { get; set; }
            public string FooValue { get; set; }
            public int GazonkValue { get; set; }

            public string Zap
            {
                set => No.Op(value);
            }
        }

        public class Foo
        {
            public Foo(string s)
            {
                No.Op(s);
            }
        }

        public class Gazonk
        {
            private readonly string _value;

            public Gazonk(string s)
            {
                _value = s;
            }

            public override bool Equals(object obj)
            {
                return obj is Gazonk o && _value == o._value;
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }
        }
    }
}