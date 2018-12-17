using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private static readonly Type[] _arrayAssignableInterfaces = typeof(int[]).GetInterfaces()
            .Where(i => i.GetTypeInfo().IsGenericType)
            .Select(i => i.GetGenericTypeDefinition())
            .ToArray();

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
            var info = type.GetTypeInfo();
            return !info.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        public static bool DelegateEquals(this Delegate @delegate, MethodInfo method, object target)
        {
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
            var info = type.GetTypeInfo();
            return (TAttribute[])info.GetCustomAttributes(typeof(TAttribute), inherit);
        }

        public static Type GetNonNullable(this Type type)
        {
            if (type.IsNullable())
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

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

        public static Type GetNonRefType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetNonRefTypeInternal();
        }

        public static Type GetNotNullable(this Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type);
            return underlying ?? type;
        }

        public static Type GetNullable(this Type type)
        {
            var info = type.GetTypeInfo();
            if (info.IsValueType && !IsNullable(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        public static Type GetReturnType(this MethodBase methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }
            return methodInfo.IsConstructor ? methodInfo.DeclaringType : ((MethodInfo)methodInfo).ReturnType;
        }

        public static MethodInfo GetStaticMethod(this Type type, string name, Type[] types)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
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
#if NET45
                    return info.GetValue(obj);
#else
            return info.GetValue(obj, null);
#endif
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
                                         || source.GetTypeInfo().IsEnum && source.GetTypeInfo().UnderlyingSystemType == typeof(bool));
        }

        public static bool HasReferenceConversionTo(this Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            // void -> void conversion is handled elsewhere
            // (it's an identity conversion)
            // All other void conversions are disallowed.
            if (source == typeof(void) || target == typeof(void))
            {
                return false;
            }
            var nonNullableSource = source.GetNonNullable();
            var nonNullableTarget = target.GetNonNullable();
            // Down conversion
            if (nonNullableSource.IsAssignableFrom(nonNullableTarget))
            {
                return true;
            }
            // Up conversion
            if (nonNullableTarget.IsAssignableFrom(nonNullableSource))
            {
                return true;
            }
            // Interface conversion
            var sourceInfo = source.GetTypeInfo();
            var targetInfo = target.GetTypeInfo();
            if (sourceInfo.IsInterface || targetInfo.IsInterface)
            {
                return true;
            }
            // Variant delegate conversion
            if (TypeHelper.IsLegalExplicitVariantDelegateConversion(source, target))
            {
                return true;
            }
            // Object conversion handled by assignable above.
            return (source.IsArray || target.IsArray) && StrictHasReferenceConversionTo(source, target, true);
        }

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

        public static bool IsAssignableTo(this Type type, ParameterInfo parameterInfo)
        {
            return IsAssignableTo(type.GetNotNullable(), parameterInfo.GetNonRefType());
        }

        public static bool IsAssignableTo(this Type type, Type target)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return type.IsAssignableToInternal(target);
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

        public static bool IsConstructedGenericType(this Type type)
        {
            var info = type.GetTypeInfo();
            return info.IsGenericType && !info.IsGenericTypeDefinition;
        }

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

        public static bool IsGenericImplementationOf(this Type type, Type interfaceGenericTypeDefinition)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
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
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
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
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
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
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
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
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
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
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
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
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
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

        public static bool IsImplicitlyConvertibleTo(this Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return source.IsImplicitlyConvertibleToInternal(target);
        }

        public static bool IsInteger(this Type type)
        {
            type = GetNonNullable(type);
            return type.IsPrimitiveInteger();
        }

        public static bool IsInteger64(this Type type)
        {
            type = GetNonNullable(type);
            if (!type.IsSameOrSubclassOfInternal(typeof(Enum)))
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

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

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
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return type.IsReferenceAssignableFromInternal(source);
        }

        public static bool IsSafeArray(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
#if NETCOREAPP2_0 || NETCOREAPP2_1
                    return type.IsSZArray;
#else
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

        public static Type MakeNullable(this Type self)
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
                if (!IsReferenceAssignableFromInternal(parameters[index].ParameterType, argTypes[index]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void SetValue(this PropertyInfo info, object obj, object value)
        {
            //Added in .NET 4.5
#if NET45
            info.SetValue(obj, value);
#else
            info.SetValue(obj, value, null);
#endif
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

        internal static Type GetNonRefTypeInternal(this Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        internal static MethodInfo GetStaticMethodInternal(this Type type, string name)
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

        internal static MethodInfo GetStaticMethodInternal(this Type type, string name, Type[] types)
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

        internal static MethodInfo[] GetStaticMethodsInternal(this Type type)
        {
            var methods = type.GetMethods();
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

        internal static bool HasReferenceConversionToInternal(this Type source, Type target)
        {
            // void -> void conversion is handled elsewhere
            // (it's an identity conversion)
            // All other void conversions are disallowed.
            if (source == typeof(void) || target == typeof(void))
            {
                return false;
            }
            var nonNullableSource = source.GetNonNullable();
            var nonNullableTarget = target.GetNonNullable();
            // Down conversion
            if (nonNullableSource.IsAssignableFrom(nonNullableTarget))
            {
                return true;
            }
            // Up conversion
            if (nonNullableTarget.IsAssignableFrom(nonNullableSource))
            {
                return true;
            }
            // Interface conversion
            var sourceInfo = source.GetTypeInfo();
            var targetInfo = target.GetTypeInfo();
            if (sourceInfo.IsInterface || targetInfo.IsInterface)
            {
                return true;
            }
            // Variant delegate conversion
            if (TypeHelper.IsLegalExplicitVariantDelegateConversion(source, target))
            {
                return true;
            }
            // Object conversion handled by assignable above.
            return (source.IsArray || target.IsArray) && StrictHasReferenceConversionToInternal(source, target, true);
        }

        internal static bool IsAssignableToInternal(this Type type, Type target)
        {
            return target.IsAssignableFrom(type)
                || TypeHelper.IsArrayTypeAssignableTo(type, target)
                || TypeHelper.IsArrayTypeAssignableToInterface(type, target);
        }

        internal static bool IsByRefParameterInternal(this ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.IsByRef)
            {
                return true;
            }
            return (parameterInfo.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out;
        }

        internal static bool IsFloatingPoint(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;

                default:
                    return false;
            }
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

        internal static bool IsImplicitlyConvertibleToInternal(this Type source, Type target)
        {
            return source == target
            || TypeHelper.IsImplicitNumericConversion(source, target)
            || TypeHelper.IsImplicitReferenceConversion(source, target)
            || TypeHelper.IsImplicitBoxingConversion(source, target)
            || TypeHelper.IsImplicitNullableConversion(source, target);
        }

        internal static bool IsReferenceAssignableFromInternal(this Type type, Type source)
        {
            // This actually implements "Is this identity assignable and/or reference assignable?"
            if (type == source)
            {
                return true;
            }
            var info = type.GetTypeInfo();
            var sourceInfo = source.GetTypeInfo();
            return !info.IsValueType
                   && !sourceInfo.IsValueType
                   && type.IsAssignableFrom(source);
        }

        internal static bool IsSameOrSubclassOfInternal(this Type type, Type baseType)
        {
            if (type == baseType)
            {
                return true;
            }
            return type.IsSubclassOfInternal(baseType);
        }

        internal static bool IsUnsigned(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                default:
                    return false;
            }
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

        private static bool HasArrayToInterfaceConversion(Type source, Type target)
        {
            if (!source.IsSafeArray() || !target.GetTypeInfo().IsInterface || !target.GetTypeInfo().IsGenericType)
            {
                return false;
            }
            var targetParams = target.GetGenericArguments();
            if (targetParams.Length != 1)
            {
                return false;
            }
            var targetGen = target.GetGenericTypeDefinition();
            foreach (var currentInterface in _arrayAssignableInterfaces)
            {
                if (targetGen == currentInterface)
                {
                    return StrictHasReferenceConversionToInternal(source.GetElementType(), targetParams[0], false);
                }
            }
            return false;
        }

        private static bool HasInterfaceToArrayConversion(Type source, Type target)
        {
            if (!target.IsSafeArray() || !source.GetTypeInfo().IsInterface || !source.GetTypeInfo().IsGenericType)
            {
                return false;
            }
            var sourceParams = source.GetGenericArguments();
            if (sourceParams.Length != 1)
            {
                return false;
            }
            var sourceGen = source.GetGenericTypeDefinition();
            foreach (var currentInterface in _arrayAssignableInterfaces)
            {
                if (sourceGen == currentInterface)
                {
                    return StrictHasReferenceConversionToInternal(sourceParams[0], target.GetElementType(), false);
                }
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

        private static bool StrictHasReferenceConversionTo(this Type source, Type target, bool skipNonArray)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            return source.StrictHasReferenceConversionToInternal(target, skipNonArray);
        }

        private static bool StrictHasReferenceConversionToInternal(this Type source, Type target, bool skipNonArray)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            // HasReferenceConversionTo was both too strict and too lax. It was too strict in prohibiting
            // some valid conversions involving arrays, and too lax in allowing casts between interfaces
            // and sealed classes that don't implement them. Unfortunately fixing the lax cases would be
            // a breaking change, especially since such expressions will even work if only given null
            // arguments.
            // This method catches the cases that were incorrectly disallowed, but when it needs to
            // examine possible conversions of element or type parameters it applies stricter rules.
            while (true)
            {
                if (!skipNonArray) // Skip if we just came from HasReferenceConversionTo and have just tested these
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (source.GetTypeInfo().IsValueType)
                    {
                        return false;
                    }
                    // ReSharper disable once PossibleNullReferenceException
                    if (target.GetTypeInfo().IsValueType)
                    {
                        return false;
                    }
                    // Includes to case of either being typeof(object)
                    if
                    (
                        // ReSharper disable once PossibleNullReferenceException
                        source.IsAssignableFrom(target)
                        // ReSharper disable once PossibleNullReferenceException
                        || target.IsAssignableFrom(source)
                    )
                    {
                        return true;
                    }
                    if (source.GetTypeInfo().IsInterface)
                    {
                        if (target.GetTypeInfo().IsInterface || target.GetTypeInfo().IsClass && !target.GetTypeInfo().IsSealed)
                        {
                            return true;
                        }
                    }
                    else if (target.GetTypeInfo().IsInterface)
                    {
                        if (source.GetTypeInfo().IsClass && !source.GetTypeInfo().IsSealed)
                        {
                            return true;
                        }
                    }
                }
                if (source.IsArray)
                {
                    if (target.IsArray)
                    {
                        if (source.GetArrayRank() != target.GetArrayRank() || source.IsSafeArray() != target.IsSafeArray())
                        {
                            return false;
                        }
                        source = source.GetElementType();
                        target = target.GetElementType();
                        skipNonArray = false;
                    }
                    else
                    {
                        return HasArrayToInterfaceConversion(source, target);
                    }
                }
                else if (target.IsArray)
                {
                    if (HasInterfaceToArrayConversion(source, target))
                    {
                        return true;
                    }
                    return TypeHelper.IsImplicitReferenceConversion(typeof(Array), source);
                }
                else
                {
                    return TypeHelper.IsLegalExplicitVariantDelegateConversion(source, target);
                }
            }
        }
    }
}