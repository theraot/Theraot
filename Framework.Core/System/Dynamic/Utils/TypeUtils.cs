#if NET20 || NET30 || NET35 || NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Theraot.Core;

namespace System.Dynamic.Utils
{
    internal static class TypeUtils
    {
        private static readonly Type[] _arrayAssignableInterfaces = typeof(int[]).GetInterfaces()
            .Where(i => i.IsGenericType)
            .Select(i => i.GetGenericTypeDefinition())
            .ToArray();

        public static bool AreEquivalent(Type t1, Type t2) => t1 != null && t1 == t2;

        public static bool AreReferenceAssignable(Type dest, Type src)
        {
            // This actually implements "Is this identity assignable and/or reference assignable?"
            if (AreEquivalent(dest, src))
            {
                return true;
            }

            return !dest.IsValueType && !src.IsValueType && dest.IsAssignableFrom(src);
        }

        public static Type FindGenericType(Type definition, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsConstructedGenericType() && AreEquivalent(type.GetGenericTypeDefinition(), definition))
                {
                    return type;
                }

                if (definition.IsInterface)
                {
                    foreach (Type currentType in type.GetTypeInfo().GetInterfaces())
                    {
                        Type found = FindGenericType(definition, currentType);
                        if (found != null)
                        {
                            return found;
                        }
                    }
                }

                type = type.BaseType;
            }

            return null;
        }

        public static bool HasBuiltInEqualityOperator(Type left, Type right)
        {
            // If we have an interface and a reference type then we can do
            // reference equality.
            if (left.IsInterface && !right.IsValueType)
            {
                return true;
            }

            if (right.IsInterface && !left.IsValueType)
            {
                return true;
            }

            // If we have two reference types and one is assignable to the
            // other then we can do reference equality.
            if (!left.IsValueType && !right.IsValueType)
            {
                if (AreReferenceAssignable(left, right) || AreReferenceAssignable(right, left))
                {
                    return true;
                }
            }

            // Otherwise, if the types are not the same then we definitely
            // do not have a built-in equality operator.
            if (!AreEquivalent(left, right))
            {
                return false;
            }

            // We have two identical value types, modulo nullability.  (If they were both the
            // same reference type then we would have returned true earlier.)
            Debug.Assert(left.IsValueType);

            // Equality between struct types is only defined for numerics, bools, enums,
            // and their nullable equivalents.
            Type nnType = left.GetNonNullableType();
            return nnType == typeof(bool) || nnType.IsNumeric() || nnType.IsEnum;
        }

        public static bool HasIdentityPrimitiveOrNullableConversionTo(this Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // Identity conversion
            if (AreEquivalent(source, dest))
            {
                return true;
            }

            // Nullable conversions
            if (IsNullableType(source) && AreEquivalent(dest, source.GetNonNullableType()))
            {
                return true;
            }

            if (IsNullableType(dest) && AreEquivalent(source, dest.GetNonNullableType()))
            {
                return true;
            }

            // Primitive runtime conversions
            // All conversions amongst enum, bool, char, integer and float types
            // (and their corresponding nullable types) are legal except for
            // nonbool==>bool and nonbool==>bool? which are only legal from
            // bool-backed enums.
            return IsConvertible(source) && IsConvertible(dest)
                   && (dest.GetNonNullableType() != typeof(bool)
                   || source.IsEnum && source.UnderlyingSystemType == typeof(bool));
        }

        public static bool HasReferenceConversionTo(this Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // void -> void conversion is handled elsewhere
            // (it's an identity conversion)
            // All other void conversions are disallowed.
            if (source == typeof(void) || dest == typeof(void))
            {
                return false;
            }

            Type nnSourceType = source.GetNonNullableType();
            Type nnDestType = dest.GetNonNullableType();

            // Down conversion
            if (nnSourceType.IsAssignableFrom(nnDestType))
            {
                return true;
            }

            // Up conversion
            if (nnDestType.IsAssignableFrom(nnSourceType))
            {
                return true;
            }

            // Interface conversion
            if (source.IsInterface || dest.IsInterface)
            {
                return true;
            }

            // Variant delegate conversion
            if (IsLegalExplicitVariantDelegateConversion(source, dest))
            {
                return true;
            }

            // Object conversion handled by assignable above.
            Debug.Assert(source != typeof(object) && dest != typeof(object));

            return (source.IsArray || dest.IsArray) && StrictHasReferenceConversionTo(source, dest, true);
        }

        public static bool HasReferenceEquality(Type left, Type right)
        {
            if (left.IsValueType || right.IsValueType)
            {
                return false;
            }

            // If we have an interface and a reference type then we can do
            // reference equality.

            // If we have two reference types and one is assignable to the
            // other then we can do reference equality.

            return left.IsInterface || right.IsInterface || AreReferenceAssignable(left, right)
                   || AreReferenceAssignable(right, left);
        }

        public static bool IsConvertible(this Type type)
        {
            type = type.GetNonNullableType();
            if (type.IsEnum)
            {
                return true;
            }

            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Char:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsImplicitlyConvertibleTo(this Type source, Type destination) =>
            AreEquivalent(source, destination) // identity conversion
            || IsImplicitNumericConversion(source, destination)
            || IsImplicitReferenceConversion(source, destination)
            || IsImplicitBoxingConversion(source, destination)
            || IsImplicitNullableConversion(source, destination);

        public static bool IsLegalExplicitVariantDelegateConversion(Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // There *might* be a legal conversion from a generic delegate type S to generic delegate type  T,
            // provided all of the follow are true:
            //   o Both types are constructed generic types of the same generic delegate type, D<X1,... Xk>.
            //     That is, S = D<S1...>, T = D<T1...>.
            //   o If type parameter Xi is declared to be invariant then Si must be identical to Ti.
            //   o If type parameter Xi is declared to be covariant ("out") then Si must be convertible
            //     to Ti via an identify conversion,  implicit reference conversion, or explicit reference conversion.
            //   o If type parameter Xi is declared to be contravariant ("in") then either Si must be identical to Ti,
            //     or Si and Ti must both be reference types.

            if (!IsDelegate(source) || !IsDelegate(dest) || !source.IsGenericType || !dest.IsGenericType)
            {
                return false;
            }

            Type genericDelegate = source.GetGenericTypeDefinition();

            if (dest.GetGenericTypeDefinition() != genericDelegate)
            {
                return false;
            }

            Type[] genericParameters = genericDelegate.GetGenericArguments();
            Type[] sourceArguments = source.GetGenericArguments();
            Type[] destArguments = dest.GetGenericArguments();

            Debug.Assert(genericParameters != null);
            Debug.Assert(sourceArguments != null);
            Debug.Assert(destArguments != null);
            Debug.Assert(genericParameters.Length == sourceArguments.Length);
            Debug.Assert(genericParameters.Length == destArguments.Length);

            for (int iParam = 0; iParam < genericParameters.Length; ++iParam)
            {
                Type sourceArgument = sourceArguments[iParam];
                Type destArgument = destArguments[iParam];

                Debug.Assert(sourceArgument != null && destArgument != null);

                // If the arguments are identical then this one is automatically good, so skip it.
                if (AreEquivalent(sourceArgument, destArgument))
                {
                    continue;
                }

                Type genericParameter = genericParameters[iParam];

                Debug.Assert(genericParameter != null);

                if (IsInvariant(genericParameter))
                {
                    return false;
                }

                if (IsCovariant(genericParameter))
                {
                    if (!sourceArgument.HasReferenceConversionTo(destArgument))
                    {
                        return false;
                    }
                }
                else if (IsContravariant(genericParameter) && (sourceArgument.IsValueType || destArgument.IsValueType))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNullableOrReferenceType(this Type type) => !type.IsValueType || IsNullableType(type);

        public static bool IsNullableType(this Type type) => type.IsConstructedGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static bool IsSameOrSubclass(Type type, Type subType) =>
                    AreEquivalent(type, subType) || subType.IsSubclassOf(type);

        // Checks if the type is a valid target for an instance call
        public static bool IsValidInstanceType(MemberInfo member, Type instanceType)
        {
            Type targetType = member.DeclaringType;
            if (AreReferenceAssignable(targetType, instanceType))
            {
                return true;
            }

            if (targetType == null)
            {
                return false;
            }

            if (instanceType.IsValueType)
            {
                if (AreReferenceAssignable(targetType, typeof(object)))
                {
                    return true;
                }

                if (AreReferenceAssignable(targetType, typeof(ValueType)))
                {
                    return true;
                }

                if (instanceType.IsEnum && AreReferenceAssignable(targetType, typeof(Enum)))
                {
                    return true;
                }

                // A call to an interface implemented by a struct is legal whether the struct has
                // been boxed or not.
                if (targetType.IsInterface)
                {
                    foreach (Type interfaceType in instanceType.GetTypeInfo().GetInterfaces())
                    {
                        if (AreReferenceAssignable(targetType, interfaceType))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static void ValidateType(Type type, string paramName) => ValidateType(type, paramName, false, false);

        public static void ValidateType(Type type, string paramName, bool allowByRef, bool allowPointer)
        {
            if (ValidateType(type, paramName, -1))
            {
                if (!allowByRef && type.IsByRef)
                {
                    throw Error.TypeMustNotBeByRef(paramName);
                }

                if (!allowPointer && type.IsPointer)
                {
                    throw Error.TypeMustNotBePointer(paramName);
                }
            }
        }

        public static bool ValidateType(Type type, string paramName, int index)
        {
            if (type == typeof(void))
            {
                return false; // Caller can skip further checks.
            }

            if (type.ContainsGenericParameters)
            {
                throw type.IsGenericTypeDefinition
                    ? Error.TypeIsGeneric(type, paramName, index)
                    : Error.TypeContainsGenericParameters(type, paramName, index);
            }

            return true;
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

        private static bool HasArrayToInterfaceConversion(Type source, Type dest)
        {
            Debug.Assert(source.IsArray);
            if (!source.IsSafeArray() || !dest.IsInterface || !dest.IsGenericType)
            {
                return false;
            }

            Type[] destParams = dest.GetGenericArguments();
            if (destParams.Length != 1)
            {
                return false;
            }

            Type destGen = dest.GetGenericTypeDefinition();

            foreach (Type currentInterface in _arrayAssignableInterfaces)
            {
                if (AreEquivalent(destGen, currentInterface))
                {
                    return StrictHasReferenceConversionTo(source.GetElementType(), destParams[0], false);
                }
            }

            return false;
        }

        private static bool HasInterfaceToArrayConversion(Type source, Type dest)
        {
            /*Debug.Assert(dest.IsSZArray);*/
            if (!dest.IsSafeArray() || !source.IsInterface || !source.IsGenericType)
            {
                return false;
            }

            Type[] sourceParams = source.GetGenericArguments();
            if (sourceParams.Length != 1)
            {
                return false;
            }

            Type sourceGen = source.GetGenericTypeDefinition();

            foreach (Type currentInterface in _arrayAssignableInterfaces)
            {
                if (AreEquivalent(sourceGen, currentInterface))
                {
                    return StrictHasReferenceConversionTo(sourceParams[0], dest.GetElementType(), false);
                }
            }

            return false;
        }

        private static bool IsContravariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
        }

        private static bool IsCovariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Covariant);
        }

        private static bool IsDelegate(Type t)
        {
            Debug.Assert(t != null);
            return t.IsSubclassOf(typeof(MulticastDelegate));
        }

        private static bool IsImplicitBoxingConversion(Type source, Type destination) =>
            source.IsValueType && (destination == typeof(object) || destination == typeof(ValueType)) || source.IsEnum && destination == typeof(Enum);

        private static bool IsImplicitNullableConversion(Type source, Type destination) =>
            IsNullableType(destination) && IsImplicitlyConvertibleTo(source.GetNonNullableType(), destination.GetNonNullableType());

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static bool IsImplicitNumericConversion(Type source, Type destination)
        {
            TypeCode tcSource = source.GetTypeCode();
            TypeCode tcDest = destination.GetTypeCode();

            switch (tcSource)
            {
                case TypeCode.SByte:
                    switch (tcDest)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;

                case TypeCode.Byte:
                    switch (tcDest)
                    {
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;

                case TypeCode.Int16:
                    switch (tcDest)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;

                case TypeCode.UInt16:
                    switch (tcDest)
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;

                case TypeCode.Int32:
                    switch (tcDest)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;

                case TypeCode.UInt32:
                    switch (tcDest)
                    {
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    switch (tcDest)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;

                case TypeCode.Char:
                    switch (tcDest)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;

                case TypeCode.Single:
                    return tcDest == TypeCode.Double;
            }

            return false;
        }

        private static bool IsImplicitReferenceConversion(Type source, Type destination) =>
            destination.IsAssignableFrom(source);

        private static bool IsInvariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 == (t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        }

        private static bool StrictHasReferenceConversionTo(this Type source, Type dest, bool skipNonArray)
        {
            // HasReferenceConversionTo was both too strict and too lax. It was too strict in prohibiting
            // some valid conversions involving arrays, and too lax in allowing casts between interfaces
            // and sealed classes that don't implement them. Unfortunately fixing the lax cases would be
            // a breaking change, especially since such expressions will even work if only given null
            // arguments.
            // This method catches the cases that were incorrectly disallowed, but when it needs to
            // examine possible conversions of element or type parameters it applies stricter rules.

            for (; ; )
            {
                if (!skipNonArray) // Skip if we just came from HasReferenceConversionTo and have just tested these
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (source.IsValueType)
                    {
                        return false;
                    }
                    // ReSharper disable once PossibleNullReferenceException
                    if (dest.IsValueType)
                    {
                        return false;
                    }

                    // Includes to case of either being typeof(object)
                    if (source.IsAssignableFrom(dest) || dest.IsAssignableFrom(source))
                    {
                        return true;
                    }

                    if (source.IsInterface)
                    {
                        if (dest.IsInterface || dest.IsClass && !dest.IsSealed)
                        {
                            return true;
                        }
                    }
                    else if (dest.IsInterface)
                    {
                        if (source.IsClass && !source.IsSealed)
                        {
                            return true;
                        }
                    }
                }

                if (source.IsArray)
                {
                    if (dest.IsArray)
                    {
                        if (source.GetArrayRank() != dest.GetArrayRank() || source.IsSafeArray() != dest.IsSafeArray())
                        {
                            return false;
                        }

                        source = source.GetElementType();
                        dest = dest.GetElementType();
                        skipNonArray = false;
                    }
                    else
                    {
                        return HasArrayToInterfaceConversion(source, dest);
                    }
                }
                else if (dest.IsArray)
                {
                    if (HasInterfaceToArrayConversion(source, dest))
                    {
                        return true;
                    }

                    return IsImplicitReferenceConversion(typeof(Array), source);
                }
                else
                {
                    return IsLegalExplicitVariantDelegateConversion(source, dest);
                }
            }
        }
    }
}

#endif