//
// ExpressionTest_Utils.cs
//
// Author:
//   Federico Di Gregorio <fog@initd.org>
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
using System.Reflection;

namespace MonoTests.System.Linq.Expressions
{
    public class OpClass
    {
        public static OpClass operator +(OpClass a, OpClass b)
        {
            return a;
        }

        public static OpClass operator -(OpClass a, OpClass b)
        {
            return a;
        }

        public static OpClass operator *(OpClass a, OpClass b)
        {
            return a;
        }

        public static OpClass operator /(OpClass a, OpClass b)
        {
            return a;
        }

        public static OpClass operator %(OpClass a, OpClass b)
        {
            return a;
        }

        public static OpClass operator &(OpClass a, OpClass b)
        {
            return a;
        }

        public static OpClass operator |(OpClass a, OpClass b)
        {
            return a;
        }

        public static OpClass operator ^(OpClass a, OpClass b)
        {
            return a;
        }

        public static OpClass operator >>(OpClass a, int b)
        {
            return a;
        }

        public static OpClass operator <<(OpClass a, int b)
        {
            return a;
        }

        public static bool operator true(OpClass a)
        {
            return false;
        }

        public static bool operator false(OpClass a)
        {
            return false;
        }

        public static bool operator >(OpClass a, OpClass b)
        {
            return false;
        }

        public static bool operator <(OpClass a, OpClass b)
        {
            return false;
        }

        public static bool operator >=(OpClass a, OpClass b)
        {
            return false;
        }

        public static bool operator <=(OpClass a, OpClass b)
        {
            return false;
        }

        public static OpClass operator +(OpClass a)
        {
            return a;
        }

        public static OpClass operator -(OpClass a)
        {
            return a;
        }

        public static OpClass operator !(OpClass a)
        {
            return a;
        }

        public static OpClass operator ~(OpClass a)
        {
            return a;
        }

        public static void WrongUnaryReturnVoid(OpClass a)
        {
            GC.KeepAlive(a);
        }

        public static OpClass WrongUnaryParameterCount(OpClass a, OpClass b)
        {
            GC.KeepAlive(b);
            return a;
        }

        public OpClass WrongUnaryNotStatic(OpClass a)
        {
            return a;
        }

        public static bool operator ==(OpClass a, OpClass b)
        {
            return a == (object)b;
        }

        public static bool operator !=(OpClass a, OpClass b)
        {
            return a != (object)b;
        }

        //
        // Required when you have == or !=
        //
        public override bool Equals(object obj)
        {
            // Keep cast
            return (object)this == obj;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }

    public class NoOpClass
    {
        // No user-defined operators here (we use this class to test for exceptions.)
    }

    public class MemberClass
    {
        public int TestField1;
        public readonly int TestField2 = 1;

        public int TestProperty1
        {
            get { return TestField1; }
        }

        public int TestProperty2
        {
            get { return TestField1; }

            set { TestField1 = value; }
        }

        public int TestMethod(int i)
        {
            return TestField1 + i;
        }

        public T TestGenericMethod<T>(T arg)
        {
            return arg;
        }

        public delegate int Delegate(int i);

        public event Delegate OnTest;

        public void DoNothing()
        {
            // Just to avoid a compiler warning
            GC.KeepAlive(OnTest);
        }

        public static int StaticField;

        public static int StaticProperty
        {
            get { return StaticField; }
            set { StaticField = value; }
        }

        public static int StaticMethod(int i)
        {
            return 1 + i;
        }

        public static T StaticGenericMethod<T>(T arg)
        {
            return arg;
        }

        public static MethodInfo GetMethodInfo()
        {
            return typeof(MemberClass).GetMethod("TestMethod");
        }

        public static FieldInfo GetRoFieldInfo()
        {
            return typeof(MemberClass).GetField("TestField1");
        }

        public static FieldInfo GetRwFieldInfo()
        {
            return typeof(MemberClass).GetField("TestField2");
        }

        public static PropertyInfo GetRoPropertyInfo()
        {
            return typeof(MemberClass).GetProperty("TestProperty1");
        }

        public static PropertyInfo GetRwPropertyInfo()
        {
            return typeof(MemberClass).GetProperty("TestProperty2");
        }

        public static EventInfo GetEventInfo()
        {
            return typeof(MemberClass).GetEvent("OnTest");
        }

        public static FieldInfo GetStaticFieldInfo()
        {
            return typeof(MemberClass).GetField("StaticField");
        }

        public static PropertyInfo GetStaticPropertyInfo()
        {
            return typeof(MemberClass).GetProperty("StaticProperty");
        }
    }

    public struct OpStruct
    {
        public static OpStruct operator +(OpStruct a, OpStruct b)
        {
            return a;
        }

        public static OpStruct operator -(OpStruct a, OpStruct b)
        {
            return a;
        }

        public static OpStruct operator *(OpStruct a, OpStruct b)
        {
            return a;
        }

        public static OpStruct operator &(OpStruct a, OpStruct b)
        {
            return a;
        }
    }

    internal static class ExpressionExtensions
    {
        public static ConstantExpression ToConstant<T>(this T t)
        {
            return Expression.Constant(t);
        }

        public static void AssertThrows(this Action action, Type type)
        {
            if (action == null)
            {
                Assert.Fail();
                return;
            }
            try
            {
                action();
                Assert.Fail();
            }
            catch (Exception e)
            {
                if (e.GetType() != type)
                {
                    Assert.Fail();
                }
            }
        }
    }

    internal class Item<T>
    {
        private bool _leftCalled;
        private readonly T _left;

        public T Left
        {
            get
            {
                _leftCalled = true;
                return _left;
            }
        }

        public bool LeftCalled
        {
            get { return _leftCalled; }
        }

        private bool _rightCalled;
        private readonly T _right;

        public T Right
        {
            get
            {
                _rightCalled = true;
                return _right;
            }
        }

        public bool RightCalled
        {
            get { return _rightCalled; }
        }

        public Item(T left, T right)
        {
            _left = left;
            _right = right;
        }
    }
}