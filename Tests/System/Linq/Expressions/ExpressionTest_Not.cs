#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

//
// ExpressionTest_Not.cs
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

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Reflection;

#endif

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestNot
    {
        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Not(null));
        }

        [Test]
        public void CompiledNotNullableBool()
        {
            var p = Expression.Parameter(typeof(bool?), "i");
            var compiled = Expression.Lambda<Func<bool?, bool?>>(Expression.Not(p), p).Compile();

            Assert.AreEqual(null, compiled(null));
            Assert.AreEqual((bool?)false, compiled(true));
            Assert.AreEqual((bool?)true, compiled(false));
        }

        [Test]
        public void CompiledNotNullableInt32()
        {
            var p = Expression.Parameter(typeof(int?), "i");
            var compiled = Expression.Lambda<Func<int?, int?>>(Expression.Not(p), p).Compile();

            Assert.AreEqual(null, compiled(null));
            Assert.AreEqual((int?)-4, compiled(3));
            Assert.AreEqual((int?)2, compiled(-3));
        }

        [Test]
        public void CompileNotBool()
        {
            var p = Expression.Parameter(typeof(bool), "i");
            var compiled = Expression.Lambda<Func<bool, bool>>(Expression.Not(p), p).Compile();

            Assert.AreEqual(false, compiled(true));
            Assert.AreEqual(true, compiled(false));
        }

        [Test]
        public void CompileNotInt32()
        {
            var p = Expression.Parameter(typeof(int), "i");
            var compiled = Expression.Lambda<Func<int, int>>(Expression.Not(p), p).Compile();

            Assert.AreEqual(-2, compiled(1));
            Assert.AreEqual(-4, compiled(3));
            Assert.AreEqual(2, compiled(-3));
        }

        [Test]
        public void MethodArgNotStatic()
        {
            Assert.Throws<ArgumentException>(() => Expression.Not(Expression.Constant(new object()), typeof(OpClass).GetMethod("WrongUnaryNotStatic")));
        }

        [Test]
        public void MethodArgParameterCount()
        {
            Assert.Throws<ArgumentException>(() => Expression.Not(Expression.Constant(new object()), typeof(OpClass).GetMethod("WrongUnaryParameterCount")));
        }

        [Test]
        public void MethodArgReturnsVoid()
        {
            Assert.Throws<ArgumentException>(() => Expression.Not(Expression.Constant(new object()), typeof(OpClass).GetMethod("WrongUnaryReturnVoid")));
        }

        [Test]
        public void NotNullableBool()
        {
            var n = Expression.Not(Expression.Parameter(typeof(bool?), ""));
            Assert.AreEqual(typeof(bool?), n.Type);
            Assert.IsTrue(n.IsLifted);
            Assert.IsTrue(n.IsLiftedToNull);
            Assert.IsNull(n.Method);
        }

        [Test]
        public void NotNullableInt32()
        {
            var n = Expression.Not(Expression.Parameter(typeof(int?), ""));
            Assert.AreEqual(typeof(int?), n.Type);
            Assert.IsTrue(n.IsLifted);
            Assert.IsTrue(n.IsLiftedToNull);
            Assert.IsNull(n.Method);
        }

        [Test]
        public void Number()
        {
            var up = Expression.Not(1.ToConstant());
            Assert.AreEqual("Not(1)", up.ToString());
        }

        [Test]
        public void UserDefinedClass()
        {
            var method = typeof(OpClass).GetMethod("op_LogicalNot");

            var expr = Expression.Not(Expression.Constant(new OpClass()));
            Assert.AreEqual(ExpressionType.Not, expr.NodeType);
            Assert.AreEqual(typeof(OpClass), expr.Type);
            Assert.AreEqual(method, expr.Method);
            Assert.AreEqual("Not(value(MonoTests.System.Linq.Expressions.OpClass))", expr.ToString());
        }

        [Test]
        public void UserDefinedNotNullable()
        {
            var method = typeof(Slot).GetMethod("op_LogicalNot");
            var s = Expression.Parameter(typeof(Slot?), "s");
            var node = Expression.Not(s);
            Assert.IsTrue(node.IsLifted);
            Assert.IsTrue(node.IsLiftedToNull);
            Assert.AreEqual(typeof(bool?), node.Type);
            Assert.AreEqual(method, node.Method);

            var compiled = Expression.Lambda<Func<Slot?, bool?>>(node, s).Compile();

            Assert.AreEqual(null, compiled(null));
            Assert.AreEqual(true, compiled(new Slot(1)));
            Assert.AreEqual(false, compiled(new Slot(0)));
        }

        private struct Slot
        {
            private readonly int _value;

            public Slot(int value)
            {
                _value = value;
            }

            public static bool operator !(Slot s)
            {
                return s._value > 0;
            }
        }
    }
}