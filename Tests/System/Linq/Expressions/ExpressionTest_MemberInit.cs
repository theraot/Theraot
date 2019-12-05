#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_MemberInit.cs
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

#if TARGETS_NETCORE || TARGETS_NETSTANDARD
using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestMemberInit
    {
        [Test]
        public void CompiledInit()
        {
            var compiled = Expression.Lambda<Func<Thing>>
            (
                Expression.MemberInit
                (
                    Expression.New(typeof(Thing)),
                    Expression.Bind(typeof(Thing).GetField("Value"), "foo".ToConstant()),
                    Expression.Bind(typeof(Thing).GetProperty(nameof(Thing.Bar)), "bar".ToConstant())
                )
            ).Compile();

            var thing = compiled();
            Assert.IsNotNull(thing);
            Assert.AreEqual("foo", thing.Value);
            Assert.AreEqual("bar", thing.Bar);
        }

        [Test]
        public void InitFields()
        {
            var m = Expression.MemberInit
            (
                Expression.New(typeof(Foo)), Expression.Bind(typeof(Foo).GetField(nameof(Foo.Bar)), "bar".ToConstant()), Expression.Bind(typeof(Foo).GetField(nameof(Foo.Baz)), "baz".ToConstant())
            );

            Assert.AreEqual(typeof(Foo), m.Type);
            Assert.AreEqual(ExpressionType.MemberInit, m.NodeType);
            Assert.AreEqual($"new {nameof(Foo)}() {{{nameof(Foo.Bar)} = \"bar\", {nameof(Foo.Baz)} = \"baz\"}}", m.ToString());
        }

        [Test]
        public void MemberNotAssignableToNewType()
        {
            Assert.Throws<ArgumentException>
            (
                () =>
                {
                    Expression.MemberInit
                    (
                        Expression.New(typeof(Foo)), Expression.Bind(typeof(Gazonk).GetField(nameof(Gazonk.Tzap)), "tzap".ToConstant())
                    );
                }
            );
        }

        [Test]
        public void NullBindings()
        {
            Assert.Throws<ArgumentNullException>
            (
                () =>
                {
                    Expression.MemberInit
                    (
                        Expression.New(typeof(Foo)),
                        null
                    );
                }
            );
        }

        [Test]
        public void NullExpression()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.MemberInit(null));
        }

        public class Foo
        {
            public string Bar;
            public string Baz;
        }

        public class Gazonk
        {
            public string Tzap;
        }

        public class Thing
        {
            public string Value;

            public string Bar { get; set; }
        }
    }
}