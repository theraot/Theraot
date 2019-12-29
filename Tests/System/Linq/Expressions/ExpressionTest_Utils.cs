#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

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

using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Theraot;

namespace MonoTests.System.Linq.Expressions
{
    public struct OpStruct
    {
        public static OpStruct operator -(OpStruct a, OpStruct b)
        {
            No.Op(b);
            return a;
        }

        public static OpStruct operator &(OpStruct a, OpStruct b)
        {
            No.Op(b);
            return a;
        }

        public static OpStruct operator *(OpStruct a, OpStruct b)
        {
            No.Op(b);
            return a;
        }

        public static OpStruct operator +(OpStruct a, OpStruct b)
        {
            No.Op(b);
            return a;
        }
    }

    public class MemberClass
    {
        public static int StaticField;

        public readonly int TestField2 = 1;

        // Used by Reflection
        public int TestField1;

        public delegate int Delegate(int i);

        // Used by Reflection

        public event Delegate OnTest;

        public static int StaticProperty
        {
            get => StaticField;
            set => StaticField = value;
        }

        public int TestProperty1 => TestField1;

        public int TestProperty2
        {
            get => TestField1;

            set => TestField1 = value;
        }

        public static EventInfo GetEventInfo()
        {
            return typeof(MemberClass).GetEvent(nameof(OnTest));
        }

        public static PropertyInfo GetRwPropertyInfo()
        {
            return typeof(MemberClass).GetProperty(nameof(TestProperty2));
        }

        public static FieldInfo GetStaticFieldInfo()
        {
            return typeof(MemberClass).GetField(nameof(StaticField));
        }

        public static PropertyInfo GetStaticPropertyInfo()
        {
            return typeof(MemberClass).GetProperty(nameof(StaticProperty));
        }

        public static T StaticGenericMethod<T>(T arg)
        {
            return arg;
        }

        public static int StaticMethod(int i)
        {
            return 1 + i;
        }

        public void DoNothing()
        {
            // Empty
        }

        public T TestGenericMethod<T>(T arg)
        {
            return arg;
        }

        public int TestMethod(int i)
        {
            return TestField1 + i;
        }
    }

    public class NoOpClass
    {
        // No user-defined operators here (we use this class to test for exceptions.)
    }

    public class OpClass
    {
        public static OpClass operator -(OpClass a, OpClass b)
        {
            No.Op(b);
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

        public static bool operator !=(OpClass a, OpClass b)
        {
            return !ReferenceEquals(a, b);
        }

        public static OpClass operator %(OpClass a, OpClass b)
        {
            No.Op(b);
            return a;
        }

        public static OpClass operator &(OpClass a, OpClass b)
        {
            No.Op(b);
            return a;
        }

        public static OpClass operator *(OpClass a, OpClass b)
        {
            No.Op(b);
            return a;
        }

        public static OpClass operator /(OpClass a, OpClass b)
        {
            No.Op(b);
            return a;
        }

        public static OpClass operator ^(OpClass a, OpClass b)
        {
            No.Op(b);
            return a;
        }

        public static OpClass operator |(OpClass a, OpClass b)
        {
            No.Op(b);
            return a;
        }

        public static OpClass operator ~(OpClass a)
        {
            return a;
        }

        public static OpClass operator +(OpClass a, OpClass b)
        {
            No.Op(b);
            return a;
        }

        public static OpClass operator +(OpClass a)
        {
            return a;
        }

        public static bool operator <(OpClass a, OpClass b)
        {
            No.Op(a);
            No.Op(b);
            return false;
        }

        public static OpClass operator <<(OpClass a, int b)
        {
            No.Op(b);
            return a;
        }

        public static bool operator <=(OpClass a, OpClass b)
        {
            No.Op(a);
            No.Op(b);
            return false;
        }

        public static bool operator ==(OpClass a, OpClass b)
        {
            return ReferenceEquals(a, b);
        }

        public static bool operator >(OpClass a, OpClass b)
        {
            No.Op(a);
            No.Op(b);
            return false;
        }

        public static bool operator >=(OpClass a, OpClass b)
        {
            No.Op(a);
            No.Op(b);
            return false;
        }

        public static OpClass operator >>(OpClass a, int b)
        {
            No.Op(b);
            return a;
        }

        public static bool operator false(OpClass a)
        {
            No.Op(a);
            return false;
        }

        public static bool operator true(OpClass a)
        {
            No.Op(a);
            return false;
        }

        public static OpClass WrongUnaryParameterCount(OpClass a, OpClass b)
        {
            No.Op(b);
            return a;
        }

        public static void WrongUnaryReturnVoid(OpClass a)
        {
            No.Op(a);
        }

        public override bool Equals(object obj)
        {
            // Required when you have == or !=
            // Keep cast
            return this == (OpClass)obj;
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public OpClass WrongUnaryNotStatic(OpClass a)
        {
            return a;
        }
    }

    internal static class ExpressionExtensions
    {
        public static void AssertThrows(this Action action, Type type)
        {
            if (action == null)
            {
                Assert.Fail();
                return; // OK
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

        public static ConstantExpression ToConstant<T>(this T t)
        {
            return Expression.Constant(t);
        }
    }

    internal class Item<T>
    {
        private readonly T _left;

        private readonly T _right;

        public Item(T left, T right)
        {
            _left = left;
            _right = right;
        }

        public T Left
        {
            get
            {
                LeftCalled = true;
                return _left;
            }
        }

        public bool LeftCalled { get; private set; }

        public T Right
        {
            get
            {
                RightCalled = true;
                return _right;
            }
        }

        public bool RightCalled { get; private set; }
    }
}