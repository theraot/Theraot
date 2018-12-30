using System;
using System.Collections.Generic;
using System.Reflection;
using Theraot.Collections.ThreadSafe;

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
using System.Runtime.CompilerServices;
#endif

namespace Theraot.Reflection
{
    public static partial class TypeExtensions
    {
        private static readonly CacheDict<Type, bool> _binaryPortableCache = new CacheDict<Type, bool>(256);
        private static readonly CacheDict<Type, bool> _blittableCache = new CacheDict<Type, bool>(256);
        private static readonly CacheDict<Type, bool> _valueTypeRecursiveCache = new CacheDict<Type, bool>(256);

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool CanBeNull(this Type type)
        {
            var info = type.GetTypeInfo();
            return !info.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool DelegateEquals(this Delegate @delegate, MethodInfo method, object target)
        {
            return @delegate.GetMethodInfo().Equals(method) && @delegate.Target == target;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static TAttribute[] GetAttributes<TAttribute>(this Assembly item)
            where TAttribute : Attribute
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
#if NET20 || NET30 || NET35 || NET40 || NET45 || NET46 ||NET47
            return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), true);
#else
            return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute));
#endif
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static TAttribute[] GetAttributes<TAttribute>(this MemberInfo item, bool inherit)
            where TAttribute : Attribute
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), inherit);
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static TAttribute[] GetAttributes<TAttribute>(this Module item)
            where TAttribute : Attribute
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
#if NET20 || NET30 || NET35 || NET40 || NET45 || NET46 ||NET47
            return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), true);
#else
            return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute));
#endif
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static TAttribute[] GetAttributes<TAttribute>(this ParameterInfo item, bool inherit)
            where TAttribute : Attribute
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), inherit);
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static TAttribute[] GetAttributes<TAttribute>(this Type type, bool inherit)
            where TAttribute : Attribute
        {
            var info = type.GetTypeInfo();
            return (TAttribute[])info.GetCustomAttributes(typeof(TAttribute), inherit);
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Type GetNonNullable(this Type type)
        {
            if (type.IsNullable())
            {
                return Nullable.GetUnderlyingType(type);
            }
            return type;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Type GetNonRefType(this ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }
            var parameterType = parameterInfo.ParameterType;
            if (parameterType.IsByRef)
            {
                parameterType = parameterType.GetElementType();
            }
            return parameterType;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Type GetNonRefType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetNonRefTypeInternal();
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Type GetNotNullable(this Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type);
            return underlying ?? type;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Type GetNullable(this Type type)
        {
            var info = type.GetTypeInfo();
            if (info.IsValueType && !IsNullable(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Type GetReturnType(this MethodBase methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            return methodInfo.IsConstructor ? methodInfo.DeclaringType : ((MethodInfo)methodInfo).ReturnType;
        }

        public static MethodInfo GetStaticMethod(this Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return type.GetStaticMethodInternal(name);
        }

        public static MethodInfo[] GetStaticMethods(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetStaticMethodsInternal();
        }

#if NET20 || NET30 || NET35

        public static TypeCode GetTypeCode(this Type type)
        {
            if (type == null)
            {
                return TypeCode.Empty;
            }
            while (true)
            {
                var info = type.GetTypeInfo();
                if (info.IsEnum)
                {
                    type = Enum.GetUnderlyingType(type);
                }
                else
                {
                    break;
                }
            }
            if (type == typeof(bool))
            {
                return TypeCode.Boolean;
            }
            if (type == typeof(char))
            {
                return TypeCode.Char;
            }
            if (type == typeof(sbyte))
            {
                return TypeCode.SByte;
            }
            if (type == typeof(byte))
            {
                return TypeCode.Byte;
            }
            if (type == typeof(short))
            {
                return TypeCode.Int16;
            }
            if (type == typeof(ushort))
            {
                return TypeCode.UInt16;
            }
            if (type == typeof(int))
            {
                return TypeCode.Int32;
            }
            if (type == typeof(uint))
            {
                return TypeCode.UInt32;
            }
            if (type == typeof(long))
            {
                return TypeCode.Int64;
            }
            if (type == typeof(ulong))
            {
                return TypeCode.UInt64;
            }
            if (type == typeof(float))
            {
                return TypeCode.Single;
            }
            if (type == typeof(double))
            {
                return TypeCode.Double;
            }
            if (type == typeof(decimal))
            {
                return TypeCode.Decimal;
            }
            if (type == typeof(DateTime))
            {
                return TypeCode.DateTime;
            }
            if (type == typeof(string))
            {
                return TypeCode.String;
            }
            return TypeCode.Object;
        }

#endif

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static object GetValue(this PropertyInfo info, object obj)
        {
            //Added in .NET 4.5
#if NET45
            return info.GetValue(obj);
#else
            return info.GetValue(obj, null);
#endif
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool HasAttribute<TAttribute>(this Assembly item)
            where TAttribute : Attribute
        {
            var attributes = item.GetAttributes<TAttribute>();
            if (attributes != null)
            {
                return attributes.Length > 0;
            }
            return false;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool HasAttribute<TAttribute>(this MemberInfo item)
            where TAttribute : Attribute
        {
            var attributes = item.GetAttributes<TAttribute>(true);
            if (attributes != null)
            {
                return attributes.Length > 0;
            }
            return false;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool HasAttribute<TAttribute>(this Module item)
            where TAttribute : Attribute
        {
            var attributes = item.GetAttributes<TAttribute>();
            if (attributes != null)
            {
                return attributes.Length > 0;
            }
            return false;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool HasAttribute<TAttribute>(this ParameterInfo item)
            where TAttribute : Attribute
        {
            var attributes = item.GetAttributes<TAttribute>(true);
            if (attributes != null)
            {
                return attributes.Length > 0;
            }
            return false;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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

        public static bool HasIdentityPrimitiveOrNullableConversionTo(this Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return source.HasIdentityPrimitiveOrNullableConversionToInternal(target);
        }

        public static bool HasIdentityPrimitiveOrNullableConversionToInternal(this Type source, Type target)
        {
            // Identity conversion
            if (source == target)
            {
                return true;
            }
            // Nullable conversions
            if (source.IsNullable() && target == source.GetNonNullable())
            {
                return true;
            }
            if (IsNullable(target) && source == target.GetNonNullable())
            {
                return true;
            }
            // Primitive runtime conversions
            // All conversions amongst enum, bool, char, integer and float types
            // (and their corresponding nullable types) are legal except for
            // nonbool==>bool and nonbool==>bool? which are only legal from
            // bool-backed enums.
            return IsConvertible(source) && IsConvertible(target)
                                         && (target.GetNonNullable() != typeof(bool)
                                         || source.GetTypeInfo().IsEnum && source.GetUnderlyingSystemType() == typeof(bool));
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsArithmetic(this Type type)
        {
            type = GetNonNullable(type);
            if
            (
                type == typeof(short)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(double)
                || type == typeof(float)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong)
            )
            {
                return true;
            }
            return false;
        }

        public static bool IsBinaryPortable(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return IsBinaryPortableExtracted(type);
        }

        public static bool IsBlittable(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return IsBlittableExtracted(type);
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsBool(this Type type)
        {
            return GetNonNullable(type) == typeof(bool);
        }

        public static bool IsByRefParameter(this ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }
            return parameterInfo.IsByRefParameterInternal();
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsConstructedGenericType(this Type type)
        {
            var info = type.GetTypeInfo();
            return info.IsGenericType && !info.IsGenericTypeDefinition;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsConvertible(this Type type)
        {
            type = type.GetNonNullable();
            var info = type.GetTypeInfo();
            if (info.IsEnum)
            {
                return true;
            }
            if
            (
                type == typeof(bool)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(char)
            )
            {
                return true;
            }
            return false;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsGenericInstanceOf(this Type type, Type genericTypeDefinition)
        {
            var info = type.GetTypeInfo();
            if (!info.IsGenericType)
            {
                return false;
            }
            return type.GetGenericTypeDefinition() == genericTypeDefinition;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsInteger(this Type type)
        {
            type = GetNonNullable(type);
            return type.IsPrimitiveInteger();
        }

        public static bool IsInteger64(this Type type)
        {
            type = GetNonNullable(type);
            return !type.IsSameOrSubclassOfInternal(typeof(Enum)) && (type == typeof(long) || type == typeof(ulong));
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsIntegerOrBool(this Type type)
        {
            type = GetNonNullable(type);
            if
            (
                type == typeof(bool)
                || type == typeof(sbyte)
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

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsNumeric(this Type type)
        {
            type = GetNonNullable(type);
            if
                (
                    type == typeof(char)
                    || type == typeof(sbyte)
                    || type == typeof(byte)
                    || type == typeof(short)
                    || type == typeof(int)
                    || type == typeof(long)
                    || type == typeof(double)
                    || type == typeof(float)
                    || type == typeof(ushort)
                    || type == typeof(uint)
                    || type == typeof(ulong)
                )
            {
                return true;
            }
            return false;
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsNumericOrBool(this Type type)
        {
            return IsNumeric(type) || IsBool(type);
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsSafeArray(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
#if NETCOREAPP2_0 || NETCOREAPP2_1
                    return type.IsSZArray;
#else
            return type.IsArray && type.GetElementType()?.MakeArrayType() == type;
#endif
        }

        public static bool IsSameOrSubclassOf(this Type type, Type baseType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }
            return type.IsSameOrSubclassOfInternal(baseType);
        }

        public static bool IsSubclassOf(this Type type, Type baseType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }
            return type.IsSubclassOfInternal(baseType);
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static bool IsSubclassOfInternal(this Type type, Type baseType)
        {
#if NETCOREAPP1_0 || NETCOREAPP1_1
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
#else
            return type.IsSubclassOf(baseType);
#endif
        }

        public static bool IsValueTypeRecursive(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return IsValueTypeRecursiveExtracted(type);
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Type MakeNullable(this Type self)
        {
            return typeof(Nullable<>).MakeGenericType(self);
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static void SetValue(this PropertyInfo info, object obj, object value)
        {
            //Added in .NET 4.5
#if NET45
            info.SetValue(obj, value);
#else
            info.SetValue(obj, value, null);
#endif
        }

        internal static Type GetNonRefTypeInternal(this Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        internal static MethodInfo GetStaticMethodInternal(this Type type, string name)
        {
            // Don't use BindingFlags.Static
            foreach (var method in type.GetTypeInfo().GetMethods())
            {
                if (method.Name == name && method.IsStatic)
                {
                    return method;
                }
            }
            return null;
        }

        internal static MethodInfo[] GetStaticMethodsInternal(this Type type)
        {
            var methods = type.GetTypeInfo().GetMethods();
            var list = new List<MethodInfo>(methods.Length);
            foreach (var method in methods)
            {
                if (method.IsStatic)
                {
                    list.Add(method);
                }
            }
            return list.ToArray();
        }

        internal static bool IsByRefParameterInternal(this ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.IsByRef)
            {
                return true;
            }
            return (parameterInfo.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out;
        }

        internal static bool IsFloatingPoint(this Type type)
        {
            type = GetNonNullable(type);
            if
            (
                type == typeof(float)
                || type == typeof(double)
            )
            {
                return true;
            }
            return false;
        }

        internal static bool IsSameOrSubclassOfInternal(this Type type, Type baseType)
        {
            if (type == baseType)
            {
                return true;
            }
            return type.IsSubclassOfInternal(baseType);
        }

        internal static bool IsUnsigned(this Type type)
        {
            // Including byte and char
            type = GetNonNullable(type);
            if
            (
                type == typeof(byte)
                || type == typeof(char)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong)
            )
            {
                return true;
            }
            return false;
        }

        internal static bool IsUnsignedInteger(this Type type)
        {
            // Not including byte or char, by design - use IsUnsigned instead
            type = GetNonNullable(type);
            if
            (
                type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong)
            )
            {
                return true;
            }
            return false;
        }

        private static bool GetBinaryPortableResult(Type type)
        {
            var info = type.GetTypeInfo();
            if (info.IsPrimitive)
            {
                return type != typeof(IntPtr)
                    && type != typeof(UIntPtr)
                    && type != typeof(char)
                    && type != typeof(bool);
            }
            if (info.IsValueType)
            {
                foreach (var field in info.GetFields())
                {
                    if (!IsBinaryPortableExtracted(field.FieldType))
                    {
                        return false;
                    }
                }
                // ReSharper disable once PossibleNullReferenceException
                return !info.IsAutoLayout && type.GetStructLayoutAttribute().Pack > 0;
            }
            return false;
        }

        private static bool GetBlittableResult(Type type)
        {
            var info = type.GetTypeInfo();
            if (info.IsPrimitive)
            {
                if
                (
                    type == typeof(char)
                    || type == typeof(bool)
                )
                {
                    return false;
                }
                return true;
            }
            if (info.IsValueType)
            {
                foreach (var field in type.GetTypeInfo().GetFields())
                {
                    if (!IsBlittableExtracted(field.FieldType))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private static bool GetValueTypeRecursiveResult(Type type)
        {
            var info = type.GetTypeInfo();
            if (info.IsPrimitive)
            {
                return true;
            }
            if (info.IsValueType)
            {
                foreach (var field in type.GetTypeInfo().GetFields())
                {
                    if (!IsValueTypeRecursiveExtracted(field.FieldType))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private static bool IsBinaryPortableExtracted(Type type)
        {
            var info = type.GetTypeInfo();
            if (!info.IsValueType)
            {
                return false;
            }
            if (_binaryPortableCache.TryGetValue(type, out var result))
            {
                return result;
            }
            result = GetBinaryPortableResult(type);
            _binaryPortableCache[type] = result;
            return result;
        }

        private static bool IsBlittableExtracted(Type type)
        {
            var info = type.GetTypeInfo();
            if (!info.IsValueType)
            {
                return false;
            }
            if (_blittableCache.TryGetValue(type, out var result))
            {
                return result;
            }
            result = GetBlittableResult(type);
            _binaryPortableCache[type] = result;
            return result;
        }

        private static bool IsValueTypeRecursiveExtracted(Type type)
        {
            var info = type.GetTypeInfo();
            if (!info.IsValueType)
            {
                return false;
            }
            if (_valueTypeRecursiveCache.TryGetValue(type, out var result))
            {
                return result;
            }
            result = GetValueTypeRecursiveResult(type);
            _binaryPortableCache[type] = result;
            return result;
        }
    }

    public static partial class TypeExtensions
    {
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo[] GetConstructors(this TypeInfo typeInfo)
        {
            var members = typeInfo.DeclaredMembers;
            var result = new List<ConstructorInfo>();
            foreach (var member in members)
            {
                if (member is ConstructorInfo constructorInfo)
                {
                    result.Add(constructorInfo);
                }
            }
            return result.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo[] GetFields(this TypeInfo typeInfo)
        {
            var members = typeInfo.DeclaredMembers;
            var result = new List<FieldInfo>();
            foreach (var member in members)
            {
                if (member is FieldInfo fieldInfo)
                {
                    result.Add(fieldInfo);
                }
            }
            return result.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfo GetConstructor(this TypeInfo typeInfo, Type[] typeArguments)
        {
            var members = typeInfo.DeclaredMembers;
            foreach (var member in members)
            {
                if (!(member is ConstructorInfo constructorInfo))
                {
                    continue;
                }
                var parameters = constructorInfo.GetParameters();
                if (parameters.Length != typeArguments.Length)
                {
                    continue;
                }
                var ok = true;
                for (var index = 0; index < typeArguments.Length; index++)
                {
                    if (parameters[index].GetType() != typeArguments[index])
                    {
                        ok = false;
                        break;
                    }
                }
                if (!ok)
                {
                    continue;
                }
                return constructorInfo;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo[] GetMethods(this TypeInfo typeInfo)
        {
            var members = typeInfo.DeclaredMembers;
            var result = new List<MethodInfo>();
            foreach (var member in members)
            {
                if (member is MethodInfo methodInfo)
                {
                    result.Add(methodInfo);
                }
            }
            return result.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo GetMethod(this TypeInfo typeInfo, string name, Type[] typeArguments)
        {
            var members = typeInfo.DeclaredMembers;
            foreach (var member in members)
            {
                if (member is MethodInfo methodInfo)
                {
                    if (member.Name != name)
                    {
                        continue;
                    }
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length != typeArguments.Length)
                    {
                        continue;
                    }
                    var ok = true;
                    for (var index = 0; index < typeArguments.Length; index++)
                    {
                        if (parameters[index].GetType() != typeArguments[index])
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (!ok)
                    {
                        continue;
                    }
                    return methodInfo;
                }
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo GetMethod(this TypeInfo typeInfo, string name)
        {
            var members = typeInfo.DeclaredMembers;
            MethodInfo found = null;
            foreach (var member in members)
            {
                if (member is MethodInfo methodInfo)
                {
                    if (member.Name != name)
                    {
                        continue;
                    }
                    if (found != null)
                    {
                        throw new AmbiguousMatchException();
                    }
                    found = methodInfo;
                }
            }
            return found;
        }
#endif

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static System.Runtime.InteropServices.StructLayoutAttribute GetStructLayoutAttribute(this Type type)
        {
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
            var attributes = type.GetAttributes<System.Runtime.InteropServices.StructLayoutAttribute>(false);
            foreach (var attribute in attributes)
            {
                return attribute;
            }
            return null;
#else
            return type.StructLayoutAttribute;
#endif
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Type GetUnderlyingSystemType(this Type type)
        {
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
            return type;
#else
            return type.UnderlyingSystemType;
#endif
        }
    }
}