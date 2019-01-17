#if LESSTHAN_NET45

// Extensions.cs
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

// Needed for NET35 (LINQ)

using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
    internal static class Extensions
    {
        public static MethodInfo GetInvokeMethod(this Type self)
        {
            return self.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
        }

        public static Type[] GetParameterTypes(this MethodBase self)
        {
            var parameters = self.GetParameters();
            var types = new Type[parameters.Length];
            for (var index = 0; index < types.Length; index++)
            {
                types[index] = parameters[index].ParameterType;
            }
            return types;
        }

        public static bool IsExpression(this Type self)
        {
            return self == typeof(Expression) || self.IsSubclassOf(typeof(Expression));
        }

        public static MethodInfo MakeGenericMethodFrom(this MethodInfo self, MethodInfo method)
        {
            return self.MakeGenericMethod(method.GetGenericArguments());
        }

        public static Type MakeGenericTypeFrom(this Type self, Type type)
        {
            return self.MakeGenericType(type.GetGenericArguments());
        }

        public static Type MakeStrongBoxType(this Type self)
        {
            return typeof(StrongBox<>).MakeGenericType(self);
        }

        public static void OnFieldOrProperty(this MemberInfo self, Action<FieldInfo> onField, Action<PropertyInfo> onProperty)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (onField == null)
            {
                throw new ArgumentNullException(nameof(onField));
            }
            if (onProperty == null)
            {
                throw new ArgumentNullException(nameof(onProperty));
            }
#endif
            switch (self.MemberType)
            {
                case MemberTypes.Field:
                    onField((FieldInfo)self);
                    return;

                case MemberTypes.Property:
                    onProperty((PropertyInfo)self);
                    return;

                default:
                    throw new ArgumentException(string.Empty);
            }
        }

        public static T OnFieldOrProperty<T>(this MemberInfo self, Func<FieldInfo, T> onField, Func<PropertyInfo, T> onProperty)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (onField == null)
            {
                throw new ArgumentNullException(nameof(onField));
            }
            if (onProperty == null)
            {
                throw new ArgumentNullException(nameof(onProperty));
            }
#endif
            switch (self.MemberType)
            {
                case MemberTypes.Field:
                    return onField((FieldInfo)self);

                case MemberTypes.Property:
                    return onProperty((PropertyInfo)self);

                default:
                    throw new ArgumentException(string.Empty);
            }
        }
    }
}

#endif