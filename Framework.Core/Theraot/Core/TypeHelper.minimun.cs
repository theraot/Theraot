// Needed for NET40

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Core
{
    public static partial class TypeHelper
    {
        public static object[] EmptyObjects
        {
            get { return ArrayReservoir<object>.EmptyArray; }
        }

        public static TTarget As<TTarget>(object source)
            where TTarget : class
        {
            return As
            (
                source,
                new Func<TTarget>
                (
                    () =>
                    {
                        throw new InvalidOperationException("Cannot convert to " + typeof(TTarget).Name);
                    }
                )
            );
        }

        public static TTarget As<TTarget>(object source, TTarget def)
            where TTarget : class
        {
            return As(source, () => def);
        }

        public static TTarget As<TTarget>(object source, Func<TTarget> alternative)
            where TTarget : class
        {
            if (alternative == null)
            {
                throw new ArgumentNullException(nameof(alternative));
            }
            var sourceAsTarget = source as TTarget;
            if (sourceAsTarget == null)
            {
                return alternative();
            }
            return sourceAsTarget;
        }

        public static bool CanBeNull(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            var info = type.GetTypeInfo();
            return !info.IsValueType || !ReferenceEquals(Nullable.GetUnderlyingType(type), null);
        }

        public static TTarget Cast<TTarget>(object source)
        {
            return Cast
            (
                source,
                new Func<TTarget>
                (
                    () =>
                    {
                        throw new InvalidOperationException("Cannot convert to " + typeof(TTarget).Name);
                    }
                )
            );
        }

        public static TTarget Cast<TTarget>(object source, TTarget def)
        {
            return Cast(source, () => def);
        }

        public static TTarget Cast<TTarget>(object source, Func<TTarget> alternative)
        {
            if (alternative == null)
            {
                throw new ArgumentNullException(nameof(alternative));
            }
            try
            {
                var sourceAsTarget = (TTarget)source;
                return sourceAsTarget;
            }
            catch
            {
                return alternative();
            }
        }

        public static object Create(this Type type, params object[] arguments)
        {
            return Activator.CreateInstance(type, arguments);
        }

        public static TReturn Default<TReturn>()
        {
            return FuncHelper.GetDefaultFunc<TReturn>().Invoke();
        }

        public static MethodInfo FindConversionOperator(MethodInfo[] methods, Type typeFrom, Type typeTo, bool implicitOnly)
        {
            foreach (var method in methods)
            {
                if (method.Name != "op_Implicit" && (implicitOnly || method.Name != "op_Explicit"))
                {
                    continue;
                }
                if (method.ReturnType != typeTo)
                {
                    continue;
                }
                var parameters = method.GetParameters();
                if (parameters[0].ParameterType != typeFrom)
                {
                    continue;
                }
                return method;
            }
            return null;
        }

        public static TAttribute[] GetAttributes<TAttribute>(this Type type, bool inherit)
                    where TAttribute : Attribute
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            var info = type.GetTypeInfo();
            return (TAttribute[])info.GetCustomAttributes(typeof(TAttribute), inherit);
        }

        public static Func<TReturn> GetDefault<TReturn>()
        {
            return FuncHelper.GetDefaultFunc<TReturn>();
        }

        public static Type GetNonRefType(this Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        public static Type GetNotNullableType(this Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying == null)
            {
                return type;
            }
            return underlying;
        }

        public static Type GetNullableType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            var info = type.GetTypeInfo();
            if (info.IsValueType && !IsNullable(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        public static bool HasAttribute<TAttribute>(this Type item)
                    where TAttribute : Attribute
        {
            var attributes = item.GetAttributes<TAttribute>(true);
            if (attributes != null)
            {
                return attributes.Length > 0;
            }
            return false;
        }

        public static bool IsAtomic<T>()
        {
#if NETCOREAPP1_0 || NETCOREAPP1_1
            var info = typeof(T).GetTypeInfo();
            return info.IsClass || (info.IsPrimitive && Marshal.SizeOf<T>() <= IntPtr.Size);
#else
            var type = typeof(T);
            var info = type.GetTypeInfo();
            return info.IsClass || (info.IsPrimitive && Marshal.SizeOf(type) <= IntPtr.Size);
#endif
        }

        public static bool IsConstructedGenericType(this Type type)
        {
            var info = type.GetTypeInfo();
            return info.IsGenericType && !info.IsGenericTypeDefinition;
        }

        public static bool IsContravariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return PrivateIsContravariant(type);
        }

        public static bool IsCovariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return PrivateIsCovariant(type);
        }

        public static bool IsDelegate(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return PrivateIsDelegate(type);
        }

        public static bool IsGenericInstanceOf(this Type type, Type genericTypeDefinition)
        {
            var info = type.GetTypeInfo();
            if (!info.IsGenericType)
            {
                return false;
            }
            return type.GetGenericTypeDefinition() == genericTypeDefinition;
        }

        public static bool IsImplicitBoxingConversion(Type source, Type target)
        {
            var info = source.GetTypeInfo();
            if (info.IsValueType && (target == typeof(object) || target == typeof(ValueType)))
            {
                return true;
            }
            if (info.IsEnum && target == typeof(Enum))
            {
                return true;
            }
            return false;
        }

        public static bool IsImplicitNumericConversion(Type source, Type target)
        {
            if (source == typeof(sbyte))
            {
                if (
                    target == typeof(short)
                    || target == typeof(int)
                    || target == typeof(long)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(byte))
            {
                if (
                    target == typeof(short)
                    || target == typeof(ushort)
                    || target == typeof(int)
                    || target == typeof(uint)
                    || target == typeof(long)
                    || target == typeof(ulong)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(short))
            {
                if (
                    target == typeof(int)
                    || target == typeof(long)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(ushort))
            {
                if (
                    target == typeof(int)
                    || target == typeof(uint)
                    || target == typeof(long)
                    || target == typeof(ulong)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(int))
            {
                if (
                    target == typeof(long)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(uint))
            {
                if (
                    target == typeof(ulong)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(long) || target == typeof(ulong))
            {
                if (
                    target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(char))
            {
                if (
                    target == typeof(ushort)
                    || target == typeof(int)
                    || target == typeof(uint)
                    || target == typeof(long)
                    || target == typeof(ulong)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(float))
            {
                return target == typeof(double);
            }
            return false;
        }

        public static bool IsInvariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return PrivateIsInvariant(type);
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsPrimitiveInteger(this Type type)
        {
            if
                (
                    type == typeof(sbyte)
                    || type == typeof(byte)
                    || type == typeof(short)
                    || type == typeof(int)
                    || type == typeof(long)
                    || type == typeof(ushort)
                    || type == typeof(uint)
                    || type == typeof(ulong)
                )
            {
                return true;
            }
            return false;
        }

        public static bool IsSameOrSubclassOf(this Type type, Type baseType)
        {
            if (type == baseType)
            {
                return true;
            }
            while (type != null)
            {
                var info = type.GetTypeInfo();
                type = info.BaseType;
                if (type == baseType)
                {
                    return true;
                }
            }
            return false;
        }

        public static Type MakeNullableType(this Type self)
        {
            return typeof(Nullable<>).MakeGenericType(self);
        }

        private static bool PrivateIsContravariant(Type type)
        {
            var info = type.GetTypeInfo();
            return 0 != (info.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
        }

        private static bool PrivateIsCovariant(Type type)
        {
            var info = type.GetTypeInfo();
            return 0 != (info.GenericParameterAttributes & GenericParameterAttributes.Covariant);
        }

        private static bool PrivateIsDelegate(Type type)
        {
            var info = type.GetTypeInfo();
            return info.IsSubclassOf(typeof(MulticastDelegate));
        }

        private static bool PrivateIsInvariant(Type type)
        {
            var info = type.GetTypeInfo();
            return 0 == (info.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        }
    }
}