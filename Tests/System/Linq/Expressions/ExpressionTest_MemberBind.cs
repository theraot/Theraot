#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_MemberBind.cs
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

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestMemberBind
    {
        public class Foo
        {
            public string Bar;
            public string Baz;

            public Gazonk Gaz;

            public Foo()
            {
                Gazoo = new Gazonk();
                Gaz = new Gazonk();
            }

            public Gazonk Gazoo { get; set; }

            public string Gruik { get; set; }
        }

        public class Gazonk
        {
            public int Klang;
            public string Tzap;

            public string Couic { get; set; }

            public string Bang()
            {
                return "";
            }
        }

        [Test]
        public void CompiledMemberBinding()
        {
            var compiled = Expression.Lambda<Func<Foo>>
            (
                Expression.MemberInit
                (
                    Expression.New(typeof(Foo)),
                    Expression.MemberBind
                    (
                        typeof(Foo).GetProperty("Gazoo"),
                        Expression.Bind
                        (
                            typeof(Gazonk).GetField("Tzap"),
                            "tzap".ToConstant()
                        ),
                        Expression.Bind
                        (
                            typeof(Gazonk).GetField("Klang"),
                            42.ToConstant()
                        )
                    )
                )
            ).Compile();

            var foo = compiled();

            Assert.IsNotNull(foo);
            Assert.AreEqual("tzap", foo.Gazoo.Tzap);
            Assert.AreEqual(42, foo.Gazoo.Klang);
        }

        [Test]
        public void MemberBindToField()
        {
            var mb = Expression.MemberBind
            (
                typeof(Foo).GetField("Gaz"),
                Expression.Bind(typeof(Gazonk).GetField("Tzap"), "tzap".ToConstant())
            );

            Assert.AreEqual(MemberBindingType.MemberBinding, mb.BindingType);
            Assert.AreEqual("Gaz = {Tzap = \"tzap\"}", mb.ToString());
        }

        [Test]
        public void MemberBindToProperty()
        {
            var mb = Expression.MemberBind
            (
                typeof(Foo).GetProperty("Gazoo"),
                Expression.Bind(typeof(Gazonk).GetField("Tzap"), "tzap".ToConstant())
            );

            Assert.AreEqual(MemberBindingType.MemberBinding, mb.BindingType);
            Assert.AreEqual("Gazoo = {Tzap = \"tzap\"}", mb.ToString());
        }

        [Test]
        public void MemberBindToPropertyAccessor()
        {
            var mb = Expression.MemberBind
            (
                typeof(Foo).GetProperty("Gazoo").GetSetMethod(true),
                Expression.Bind(typeof(Gazonk).GetField("Tzap"), "tzap".ToConstant())
            );

            Assert.AreEqual(MemberBindingType.MemberBinding, mb.BindingType);
            Assert.AreEqual("Gazoo = {Tzap = \"tzap\"}", mb.ToString());
        }

        [Test]
        public void MemberNotFieldOrProp()
        {
            Assert.Throws<ArgumentException>(() => Expression.MemberBind(typeof(Gazonk).GetMethod("Bang") as MemberInfo));
        }

        [Test]
        public void MemberTypeMismatch()
        {
            Assert.Throws<ArgumentException>(() => Expression.MemberBind(typeof(Gazonk).GetField("Klang"), Expression.Bind(typeof(Foo).GetField("Bar"), "bar".ToConstant())));
        }

        [Test]
        public void MethodNotPropertyAccessor()
        {
            Assert.Throws<ArgumentException>(() => Expression.MemberBind(typeof(Gazonk).GetMethod("Bang")));
        }

        [Test]
        public void NullBindings()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.MemberBind(typeof(Foo).GetField("Bar"), null));
        }

        [Test]
        public void NullMember()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.MemberBind(null as MemberInfo));
        }

        [Test]
        public void NullMethod()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.MemberBind(null));
        }
    }
}