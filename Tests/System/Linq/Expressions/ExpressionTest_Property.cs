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
using System.Reflection;

namespace MonoTests.System.Linq.Expressions
{
    [TestFixture]
    public class ExpressionTestProperty
    {
        [Test]
        public void Arg1Null()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Property(null, "NoProperty"));
        }

        [Test]
        public void Arg2Null1()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Property(Expression.Constant(new MemberClass()), (string)null));
        }

        [Test]
        public void Arg2Null2()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Property(Expression.Constant(new MemberClass()), (PropertyInfo)null));
        }

        [Test]
        public void Arg2Null3()
        {
            Assert.Throws<ArgumentNullException>(() => Expression.Property(Expression.Constant(new MemberClass()), (MethodInfo)null));
        }

        [Test]
        public void NoProperty()
        {
            Assert.Throws<ArgumentException>(() => Expression.Property(Expression.Constant(new MemberClass()), "NoProperty"));
        }

        [Test]
        public void InstanceProperty1()
        {
            var expr = Expression.Property(Expression.Constant(new MemberClass()), "TestProperty1");
            Assert.AreEqual(ExpressionType.MemberAccess, expr.NodeType, "Property#01");
            Assert.AreEqual(typeof(int), expr.Type, "Property#02");
            Assert.AreEqual("value(MonoTests.System.Linq.Expressions.MemberClass).TestProperty1", expr.ToString(), "Property#03");
        }

        [Test]
        public void InstanceProperty2()
        {
            var expr = Expression.Property(Expression.Constant(new MemberClass()), MemberClass.GetRoPropertyInfo());
            Assert.AreEqual(ExpressionType.MemberAccess, expr.NodeType, "Property#04");
            Assert.AreEqual(typeof(int), expr.Type, "Property#05");
            Assert.AreEqual("value(MonoTests.System.Linq.Expressions.MemberClass).TestProperty1", expr.ToString(), "Property#06");
        }

        [Test]
        public void InstanceProperty3()
        {
            var mi = typeof(MemberClass).GetMethod("get_TestProperty1");

            var expr = Expression.Property(Expression.Constant(new MemberClass()), mi);
            Assert.AreEqual(ExpressionType.MemberAccess, expr.NodeType, "Property#07");
            Assert.AreEqual(typeof(int), expr.Type, "Property#08");
            Assert.AreEqual("value(MonoTests.System.Linq.Expressions.MemberClass).TestProperty1", expr.ToString(), "Property#09");
            Assert.AreEqual(MemberClass.GetRoPropertyInfo(), expr.Member, "Property#10");
        }

        [Test]
        public void StaticProperty1()
        {
            Assert.Throws<ArgumentException>(() => { // This will fail because access to a static field should be created using a PropertyInfo and
                // not an instance plus the field name.
                Expression.Property(Expression.Constant(new MemberClass()), "StaticProperty");
            });
        }

        [Test]
        public void StaticProperty2()
        {
            var expr = Expression.Property(null, MemberClass.GetStaticPropertyInfo());
            Assert.AreEqual(ExpressionType.MemberAccess, expr.NodeType, "Property#11");
            Assert.AreEqual(typeof(int), expr.Type, "Property#12");
            Assert.AreEqual("MemberClass.StaticProperty", expr.ToString(), "Property#13");
        }

        [Test]
        public void StaticProperty3()
        {
            var mi = typeof(MemberClass).GetMethod("get_StaticProperty");

            var expr = Expression.Property(null, mi);
            Assert.AreEqual(ExpressionType.MemberAccess, expr.NodeType, "Property#14");
            Assert.AreEqual(typeof(int), expr.Type, "Property#15");
            Assert.AreEqual("MemberClass.StaticProperty", expr.ToString(), "Property#16");
            Assert.AreEqual(MemberClass.GetStaticPropertyInfo(), expr.Member, "Property#17");
        }

        public class Foo
        {
            public string Prop { get; set; }

            public static string StatProp => "StaticFoo";
        }

        [Test]
        public void TestCompileGetInstanceProperty()
        {
            var p = Expression.Parameter(typeof(Foo), "foo");
            var fooer = Expression.Lambda<Func<Foo, string>>(
                Expression.Property(p, typeof(Foo).GetProperty("Prop")), p).Compile();

            Assert.AreEqual("foo", fooer(new Foo { Prop = "foo" }));
        }

        [Test]
        public void TestCompileGetStaticProperty()
        {
            var sf = Expression.Lambda<Func<string>>(
                Expression.Property(null, typeof(Foo).GetProperty(
                "StatProp", BindingFlags.Public | BindingFlags.Static))).Compile();

            Assert.AreEqual("StaticFoo", sf());
        }

        public struct Bar
        {
            public string Prop { get; set; }

            public Bar(string slot)
            {
                Prop = slot;
            }
        }

        [Test]
        public void TestCompileGetInstancePropertyOnStruct()
        {
            var p = Expression.Parameter(typeof(Bar), "bar");
            var barer = Expression.Lambda<Func<Bar, string>>(
                Expression.Property(p, typeof(Bar).GetProperty("Prop")), p).Compile();

            Assert.AreEqual("bar", barer(new Bar("bar")));
        }

        public static int StaticProperty => 42;

        [Test]
        [Category("NotDotNet")] // http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=339351
        public void StaticPropertyWithInstanceArgument()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Expression.Property(
                    Expression.Parameter(GetType(), "t"),
                    GetType().GetProperty("StaticProperty"));
            });
        }
    }
}