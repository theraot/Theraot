#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_GreaterThan.cs
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

using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestGreaterThan
    {
        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => { Expression.GreaterThan(null, Expression.Constant(1)); });
        }

        [Test]
        public void Arg2Null()
        {
            Assert.Throws<ArgumentNullException>(() => { Expression.GreaterThan(Expression.Constant(1), null); });
        }

        [Test]
        public void NoOperatorClass()
        {
            Assert.Throws<InvalidOperationException>(() => { Expression.GreaterThan(Expression.Constant(new NoOpClass()), Expression.Constant(new NoOpClass())); });
        }

        [Test]
        public void Double()
        {
            var expr = Expression.GreaterThan(Expression.Constant(2.0), Expression.Constant(1.0));
            Assert.AreEqual(ExpressionType.GreaterThan, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.IsNull(expr.Method);
            Assert.AreEqual("(2 > 1)", expr.ToString());
        }

        [Test]
        public void Integer()
        {
            var expr = Expression.GreaterThan(Expression.Constant(2), Expression.Constant(1));
            Assert.AreEqual(ExpressionType.GreaterThan, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.IsNull(expr.Method);
            Assert.AreEqual("(2 > 1)", expr.ToString());
        }

        [Test]
        public void MismatchedTypes()
        {
            Assert.Throws<InvalidOperationException>(() => { Expression.GreaterThan(Expression.Constant(new OpClass()), Expression.Constant(true)); });
        }

        [Test]
        public void Boolean()
        {
            Assert.Throws<InvalidOperationException>(() => { Expression.GreaterThan(Expression.Constant(true), Expression.Constant(false)); });
        }

        [Test]
        public void StringS()
        {
            Assert.Throws<InvalidOperationException>(() => { Expression.GreaterThan(Expression.Constant(""), Expression.Constant("")); });
        }

        [Test]
        public void UserDefinedClass()
        {
            var mi = typeof(OpClass).GetMethod("op_GreaterThan");

            Assert.IsNotNull(mi);

            var expr = Expression.GreaterThan(Expression.Constant(new OpClass()), Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.GreaterThan, expr.NodeType);
            Assert.AreEqual(typeof(bool), expr.Type);
            Assert.AreEqual(mi, expr.Method);
            Assert.AreEqual("op_GreaterThan", expr.Method.Name);
            Assert.AreEqual("(value(MonoTests.System.Linq.Expressions.OpClass) > value(MonoTests.System.Linq.Expressions.OpClass))", expr.ToString());
        }

        [Test]
        public void TestCompiled()
        {
            var a = Expression.Parameter(typeof(int), "a");
            var b = Expression.Parameter(typeof(int), "b");

            var p = Expression.GreaterThan(a, b);

            var pexpr = Expression.Lambda<Func<int, int, bool>>(
                p, new[] { a, b });

            var compiled = pexpr.Compile();
            Assert.AreEqual(true, compiled(10, 1), "tc1");
            Assert.AreEqual(true, compiled(1, 0), "tc2");
            Assert.AreEqual(true, compiled(int.MinValue + 1, int.MinValue), "tc3");
            Assert.AreEqual(false, compiled(-1, 0), "tc4");
            Assert.AreEqual(false, compiled(0, int.MaxValue), "tc5");
        }

        [Test]
        public void NullableInt32GreaterThan()
        {
            var l = Expression.Parameter(typeof(int?), "l");
            var r = Expression.Parameter(typeof(int?), "r");

            var gt = Expression.Lambda<Func<int?, int?, bool>>(
                Expression.GreaterThan(l, r), l, r).Compile();

            Assert.IsFalse(gt(null, null));
            Assert.IsFalse(gt(null, 1));
            Assert.IsFalse(gt(null, -1));
            Assert.IsFalse(gt(1, null));
            Assert.IsFalse(gt(-1, null));
            Assert.IsFalse(gt(1, 2));
            Assert.IsTrue(gt(2, 1));
            Assert.IsFalse(gt(1, 1));
        }

        [Test]
        public void NullableInt32GreaterThanLiftedToNull()
        {
            var l = Expression.Parameter(typeof(int?), "l");
            var r = Expression.Parameter(typeof(int?), "r");

            var gt = Expression.Lambda<Func<int?, int?, bool?>>(
                Expression.GreaterThan(l, r, true, null), l, r).Compile();

            Assert.AreEqual(null, gt(null, null));
            Assert.AreEqual(null, gt(null, 1));
            Assert.AreEqual(null, gt(null, -1));
            Assert.AreEqual(null, gt(1, null));
            Assert.AreEqual(null, gt(-1, null));
            Assert.AreEqual((bool?)false, gt(1, 2));
            Assert.AreEqual((bool?)true, gt(2, 1));
            Assert.AreEqual((bool?)false, gt(1, 1));
        }

        private struct Slot
        {
            public readonly int Value;

            public Slot(int val)
            {
                Value = val;
            }

            public static bool operator >(Slot a, Slot b)
            {
                return a.Value > b.Value;
            }

            public static bool operator <(Slot a, Slot b)
            {
                return a.Value < b.Value;
            }
        }

        [Test]
        public void UserDefinedGreaterThanLifted()
        {
            var l = Expression.Parameter(typeof(Slot?), "l");
            var r = Expression.Parameter(typeof(Slot?), "r");

            var node = Expression.GreaterThan(l, r);
            Assert.IsTrue(node.IsLifted);
            Assert.IsFalse(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool), node.Type);
            Assert.IsNotNull(node.Method);

            var gte = Expression.Lambda<Func<Slot?, Slot?, bool>>(node, l, r).Compile();

            Assert.AreEqual(true, gte(new Slot(1), new Slot(0)));
            Assert.AreEqual(false, gte(new Slot(-1), new Slot(1)));
            Assert.AreEqual(false, gte(new Slot(1), new Slot(1)));
            Assert.AreEqual(false, gte(null, new Slot(1)));
            Assert.AreEqual(false, gte(new Slot(1), null));
            Assert.AreEqual(false, gte(null, null));
        }

        [Test]
        public void UserDefinedGreaterThanLiftedToNull()
        {
            var l = Expression.Parameter(typeof(Slot?), "l");
            var r = Expression.Parameter(typeof(Slot?), "r");

            var node = Expression.GreaterThan(l, r, true, null);
            Assert.IsTrue(node.IsLifted);
            Assert.IsTrue(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool?), node.Type);
            Assert.IsNotNull(node.Method);

            var gte = Expression.Lambda<Func<Slot?, Slot?, bool?>>(node, l, r).Compile();

            Assert.AreEqual(true, gte(new Slot(1), new Slot(0)));
            Assert.AreEqual(false, gte(new Slot(-1), new Slot(1)));
            Assert.AreEqual(false, gte(new Slot(1), new Slot(1)));
            Assert.AreEqual(null, gte(null, new Slot(1)));
            Assert.AreEqual(null, gte(new Slot(1), null));
            Assert.AreEqual(null, gte(null, null));
        }

        private enum Foo
        {
            Bar,
            Baz
        }

        [Test]
        public void EnumGreaterThan()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Expression.GreaterThan(
                    Foo.Bar.ToConstant(),
                    Foo.Baz.ToConstant());
            });
        }
    }
}