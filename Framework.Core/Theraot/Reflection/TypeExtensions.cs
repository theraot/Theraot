using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Reflection
{
    public static class TypeExtensions
    {
        private static readonly Assembly[] _assemblies;
        private static readonly CacheDict<Type, bool> _binaryPortableCache = new CacheDict<Type, bool>(256);
        private static readonly CacheDict<Type, bool> _blittableCache = new CacheDict<Type, bool>(256);
        private static readonly CacheDict<Type, bool> _valueTypeRecursiveCache = new CacheDict<Type, bool>(256);

        static TypeExtensions()
        {
            Type[] known = {
                typeof(object),
                typeof(BitConverter),
                typeof(StructuralComparisons),
                typeof(Debug),
                typeof(IStrongBox),
                typeof(BarrierPostPhaseException),
                typeof(TaskExtensions),
                typeof(Uri),
                typeof(TypeHelper),
                typeof(CancelEventArgs),
                typeof(Console),
                typeof(BufferedStream),
                typeof(File),
                typeof(FileAccess),
                typeof(ResourceReader),
                typeof(AsnEncodedData),
                typeof(AsymmetricAlgorithm),
                typeof(IIdentity)
            };
            var assemblies = new List<Assembly>();
            foreach (var type in known)
            {
                var info = type.GetTypeInfo();
                var assembly = info.Assembly;
                if (!assemblies.Contains(assembly))
                {
                    assemblies.Add(assembly);
                }
            }
            _assemblies = assemblies.ToArray();
        }

        public static bool CanBe<T>(this Type type, T value)
        {
            if (value == null)
            {
                return type.CanBeNull();
            }
            return value.GetType().IsAssignableTo(type);
        }

        public static bool CanBeNull(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            var info = type.GetTypeInfo();
            return !info.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        public static object Create(this Type type, params object[] arguments)
        {
            return Activator.CreateInstance(type, arguments);
        }

        public static bool DelegateEquals(this Delegate @delegate, MethodInfo method, object target)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }
            return @delegate.GetMethodInfo().Equals(method) && @delegate.Target == target;
        }

        public static TAttribute[] GetAttributes<TAttribute>(this ICustomAttributeProvider item, bool inherit)
                                    where TAttribute : Attribute
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return (TAttribute[])item.GetCustomAttributes(typeof(TAttribute), inherit);
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

        public static MethodInfo[] GetMethodsIgnoreCase(this Type type, BindingFlags flags, string name)
        {
            var list = new List<MethodInfo>();
            foreach (var method in type.GetMethods(flags))
            {
                if (method.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(method);
                }
            }
            return list.ToArray();
        }

        public static Type GetNonNullableType(this Type type)
        {
            if (type.IsNullable())
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static Type GetNonRefType(this Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        public static Type GetNonRefType(this ParameterInfo parameterInfo)
        {
            var parameterType = parameterInfo.ParameterType;
            if (parameterType.IsByRef)
            {
                parameterType = parameterType.GetElementType();
            }
            return parameterType;
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

        public static Type GetReturnType(this MethodBase mi) => mi.IsConstructor ? mi.DeclaringType : ((MethodInfo)mi).ReturnType;

        public static MethodInfo GetStaticMethod(this Type type, string name)
        {
            // Don't use BindingFlags.Static
            foreach (var method in type.GetMethods())
            {
                if (method.Name == name && method.IsStatic)
                {
                    return method;
                }
            }
            return null;
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, Type[] types)
        {
            // Don't use BindingFlags.Static
            foreach (var method in type.GetMethods())
            {
                if (method.Name == name && method.IsStatic && method.MatchesArgumentTypes(types))
                {
                    return method;
                }
            }
            return null;
        }

        public static MethodInfo[] GetStaticMethods(this Type type)
        {
            var list = new List<MethodInfo>();
            foreach (var method in type.GetMethods())
            {
                if (method.IsStatic)
                {
                    list.Add(method);
                }
            }
            return list.ToArray();
        }

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

        public static object GetValue(this PropertyInfo info, object obj)
        {
            //Added in .NET 4.5
            return info.GetValue(obj, null);
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

        public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider item)
            where TAttribute : Attribute
        {
            var attributes = item.GetAttributes<TAttribute>(true);
            if (attributes != null)
            {
                return attributes.Length > 0;
            }
            return false;
        }

        public static bool HasConstructor(this Type type, params Type[] typeArguments)
        {
            var constructorInfo = type.GetConstructor(typeArguments);
            return constructorInfo == null;
        }

        public static bool IsArithmetic(this Type type)
        {
            type = GetNonNullableType(type);
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

        public static bool IsAssignableTo(this Type type, Type target)
        {
            return target.IsAssignableFrom(type)
                || TypeHelper.IsArrayTypeAssignableTo(type, target)
                || TypeHelper.IsArrayTypeAssignableToInterface(type, target);
        }

        public static bool IsAssignableTo(this Type type, ParameterInfo parameterInfo)
        {
            return IsAssignableTo(type.GetNotNullableType(), parameterInfo.GetNonRefType());
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

        public static bool IsBool(this Type type)
        {
            return GetNonNullableType(type) == typeof(bool);
        }

        public static bool IsConstructedGenericType(this Type type)
        {
            var info = type.GetTypeInfo();
            return info.IsGenericType && !info.IsGenericTypeDefinition;
        }

        public static bool IsGenericImplementationOf(this Type type, Type interfaceGenericTypeDefinition)
        {
            foreach (var currentInterface in type.GetInterfaces())
            {
                if (currentInterface.IsGenericInstanceOf(interfaceGenericTypeDefinition))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsGenericImplementationOf(this Type type, params Type[] interfaceGenericTypeDefinitions)
        {
            foreach (var currentInterface in type.GetInterfaces())
            {
                var info = currentInterface.GetTypeInfo();
                if (info.IsGenericTypeDefinition)
                {
                    var match = currentInterface.GetGenericTypeDefinition();
                    if (Array.Exists(interfaceGenericTypeDefinitions, item => item == match))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsGenericImplementationOf(this Type type, out Type interfaceType, Type interfaceGenericTypeDefinition)
        {
            foreach (var currentInterface in type.GetInterfaces())
            {
                if (currentInterface.IsGenericInstanceOf(interfaceGenericTypeDefinition))
                {
                    interfaceType = currentInterface;
                    return true;
                }
            }
            interfaceType = null;
            return false;
        }

        public static bool IsGenericImplementationOf(this Type type, out Type interfaceType, params Type[] interfaceGenericTypeDefinitions)
        {
            var implementedInterfaces = type.GetInterfaces();
            foreach (var currentInterface in interfaceGenericTypeDefinitions)
            {
                var index = Array.FindIndex(implementedInterfaces, item => item.IsGenericInstanceOf(currentInterface));
                if (index != -1)
                {
                    interfaceType = implementedInterfaces[index];
                    return true;
                }
            }
            interfaceType = null;
            return false;
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

        public static bool IsImplementationOf(this Type type, Type interfaceType)
        {
            foreach (var currentInterface in type.GetInterfaces())
            {
                if (currentInterface == interfaceType)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsImplementationOf(this Type type, params Type[] interfaceTypes)
        {
            foreach (var currentInterface in type.GetInterfaces())
            {
                if (Array.Exists(interfaceTypes, item => currentInterface == item))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsImplementationOf(this Type type, out Type interfaceType, params Type[] interfaceTypes)
        {
            var implementedInterfaces = type.GetInterfaces();
            foreach (var currentInterface in interfaceTypes)
            {
                var index = Array.FindIndex(implementedInterfaces, item => item == currentInterface);
                if (index != -1)
                {
                    interfaceType = implementedInterfaces[index];
                    return true;
                }
            }
            interfaceType = null;
            return false;
        }

        public static bool IsInteger(this Type type)
        {
            type = GetNonNullableType(type);
            return type.IsPrimitiveInteger();
        }

        public static bool IsInteger64(this Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsSameOrSubclassOf(typeof(Enum)))
            {
                switch (type.GetTypeCode())
                {
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return true;
                }
            }

            return false;
        }

        public static bool IsIntegerOrBool(this Type type)
        {
            type = GetNonNullableType(type);
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

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsNumeric(this Type type)
        {
            type = GetNonNullableType(type);
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

        public static bool IsNumericOrBool(this Type type)
        {
            return IsNumeric(type) || IsBool(type);
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

        public static bool IsReferenceAssignableFrom(this Type type, Type source)
        {
            if (type == source)
            {
                return true;
            }
            var info = type.GetTypeInfo();
            var sourceInfo = source.GetTypeInfo();
            if (
                !info.IsValueType
                && !sourceInfo.IsValueType
                && type.IsAssignableFrom(source)
                )
            {
                return true;
            }
            return false;
        }

        public static bool IsSafeArray(this Type type)
        {
            try
            {
                // GetArrayRank could throw - should not, but could.
                // We are not checking the lower bound of the array type, there is no API for that.
                // However, the type of arrays that can have a different lower index other than zero...
                // ... have two constructors, one taking only the size, and one taking the lower and upper bounds.
                return type.IsArray
                       && typeof(Array).IsAssignableFrom(type)
                       && type.GetArrayRank() == 1
                       && type.GetElementType() != null
                       && type.GetConstructors().Length == 1;
            }
            catch (Exception exception)
            {
                GC.KeepAlive(exception);
                return false;
            }
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

        public static bool IsValueTypeRecursive(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return IsValueTypeRecursiveExtracted(type);
        }
        public static Type MakeNullableType(this Type self)
        {
            return typeof(Nullable<>).MakeGenericType(self);
        }

        public static bool MatchesArgumentTypes(this MethodInfo method, Type[] argTypes)
        {
            if (method == null || argTypes == null)
            {
                return false;
            }
            var parameters = method.GetParameters();
            if (parameters.Length != argTypes.Length)
            {
                return false;
            }
            for (var index = 0; index < parameters.Length; index++)
            {
                if (!IsReferenceAssignableFrom(parameters[index].ParameterType, argTypes[index]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void SetValue(this PropertyInfo info, object obj, object value)
        {
            //Added in .NET 4.5
            info.SetValue(obj, value, null);
        }

        internal static bool CanCache(Type type)
        {
            var info = type.GetTypeInfo();
            var assembly = info.Assembly;
            if (Array.IndexOf(_assemblies, assembly) == -1)
            {
                return false;
            }
            if (info.IsGenericType)
            {
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    if (!CanCache(genericArgument))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool IsFloatingPoint(this Type type)
        {
            type = GetNonNullableType(type);
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

        internal static bool IsUnsigned(this Type type)
        {
            // Including byte and char
            type = GetNonNullableType(type);
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
            type = GetNonNullableType(type);
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
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (!IsBinaryPortableExtracted(field.FieldType))
                    {
                        return false;
                    }
                }
                // ReSharper disable once PossibleNullReferenceException
                return !info.IsAutoLayout && info.StructLayoutAttribute.Pack > 0;
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
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
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
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
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
            var canCache = CanCache(type);
            if (canCache && _binaryPortableCache.TryGetValue(type, out var result))
            {
                return result;
            }
            result = GetBinaryPortableResult(type);
            if (canCache)
            {
                _binaryPortableCache[type] = result;
            }
            return result;
        }

        private static bool IsBlittableExtracted(Type type)
        {
            var info = type.GetTypeInfo();
            if (!info.IsValueType)
            {
                return false;
            }
            var canCache = CanCache(type);
            if (canCache && _blittableCache.TryGetValue(type, out var result))
            {
                return result;
            }
            result = GetBlittableResult(type);
            if (canCache)
            {
                _binaryPortableCache[type] = result;
            }
            return result;
        }

        private static bool IsValueTypeRecursiveExtracted(Type type)
        {
            var info = type.GetTypeInfo();
            if (!info.IsValueType)
            {
                return false;
            }
            var canCache = CanCache(type);
            if (canCache && _valueTypeRecursiveCache.TryGetValue(type, out var result))
            {
                return result;
            }
            result = GetValueTypeRecursiveResult(type);
            if (canCache)
            {
                _binaryPortableCache[type] = result;
            }
            return result;
        }
    }
}
