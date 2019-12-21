#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Theraot.Reflection;

namespace System.Dynamic.Utils
{
    internal static class TypeUtils
    {
        private static readonly Type[] _arrayAssignableInterfaces = typeof(int[]).GetInterfaces()
            .Where(i => i.IsGenericType)
            .Select(i => i.GetGenericTypeDefinition())
            .ToArray();

        internal static bool AreEquivalent(Type? t1, Type t2)
        {
            return t1 != null && t1 == t2;
        }

        internal static Type? FindGenericType(Type definition, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsConstructedGenericType() && type.GetGenericTypeDefinition() == definition)
                {
                    return type;
                }

                if (definition.IsInterface)
                {
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        var found = FindGenericType(definition, interfaceType);
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

        internal static MethodInfo? GetBooleanOperator(Type type, string name)
        {
            do
            {
                var result = type.GetStaticMethodInternal(name, new[] { type });
                if (result?.IsSpecialName == true && !result.ContainsGenericParameters)
                {
                    return result;
                }

                type = type.BaseType;
            } while (type != null);

            return null;
        }

        internal static MethodInfo GetStaticMethodInternal(this Type type, string name, Type[] types)
        {
            // Don't use BindingFlags.Static
            return Array.Find(type.GetMethods(), method => string.Equals(method.Name, name, StringComparison.Ordinal) && method.IsStatic && method.MatchesArgumentTypes(types));
        }

        internal static bool HasBuiltInEqualityOperator(Type left, Type right)
        {
            if (left.IsInterface && !right.IsValueType)
            {
                return true;
            }

            if (right.IsInterface && !left.IsValueType)
            {
                return true;
            }

            if (!left.IsValueType && !right.IsValueType && (left.IsReferenceAssignableFromInternal(right) || right.IsReferenceAssignableFromInternal(left)))
            {
                return true;
            }

            if (left != right)
            {
                return false;
            }

            var notNullable = left.GetNonNullable();
            return notNullable == typeof(bool) || notNullable.IsNumeric() || notNullable.IsEnum;
        }

        internal static bool HasReferenceConversionTo(this Type source, Type target)
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
            if (source.IsInterface || target.IsInterface)
            {
                return true;
            }

            // Variant delegate conversion
            if (IsLegalExplicitVariantDelegateConversion(source, target))
            {
                return true;
            }

            // Object conversion handled by assignable above.
            return (source.IsArray || target.IsArray) && StrictHasReferenceConversionTo(source, target, true);
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
            if (source.IsInterface || target.IsInterface)
            {
                return true;
            }

            // Variant delegate conversion
            if (IsLegalExplicitVariantDelegateConversion(source, target))
            {
                return true;
            }

            // Object conversion handled by assignable above.
            return (source.IsArray || target.IsArray) && StrictHasReferenceConversionToInternal(source, target, true);
        }

        internal static bool HasReferenceEquality(Type left, Type right)
        {
            if (left.IsValueType || right.IsValueType)
            {
                return false;
            }

            // If we have an interface and a reference type then we can do
            // reference equality.
            // If we have two reference types and one is assignable to the
            // other then we can do reference equality.
            return left.IsInterface
                   || right.IsInterface
                   || left.IsReferenceAssignableFromInternal(right)
                   || right.IsReferenceAssignableFromInternal(left);
        }

        internal static bool IsArrayTypeAssignableTo(Type type, Type target)
        {
            if (!type.IsArray || !target.IsArray)
            {
                return false;
            }

            return type.GetArrayRank() == target.GetArrayRank() && type.GetElementType().IsAssignableToInternal(target.GetElementType());
        }

        internal static bool IsArrayTypeAssignableToInterface(Type type, Type target)
        {
            if (!type.IsArray)
            {
                return false;
            }

            return
                (
                    target.IsGenericInstanceOf(typeof(IList<>))
                    || target.IsGenericInstanceOf(typeof(ICollection<>))
                    || target.IsGenericInstanceOf(typeof(IEnumerable<>))
                )
                && type.GetElementType() == target.GetGenericArguments()[0];
        }

        internal static bool IsAssignableTo(this Type type, Type target)
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

        internal static bool IsAssignableToInternal(this Type type, Type target)
        {
            return target.IsAssignableFrom(type)
                   || IsArrayTypeAssignableTo(type, target)
                   || IsArrayTypeAssignableToInterface(type, target);
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

        internal static bool IsGenericImplementationOf(this Type type, Type interfaceGenericTypeDefinition, [NotNullWhen(true)] out Type? interfaceType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            foreach (var currentInterface in type.GetInterfaces())
            {
                if (!currentInterface.IsGenericInstanceOf(interfaceGenericTypeDefinition))
                {
                    continue;
                }

                interfaceType = currentInterface;
                return true;
            }

            interfaceType = default;
            return false;
        }

        internal static bool IsImplicitlyConvertibleToInternal(this Type source, Type target)
        {
            return source == target
                   || TypeHelper.IsImplicitNumericConversion(source, target)
                   || IsImplicitReferenceConversion(source, target)
                   || TypeHelper.IsImplicitBoxingConversion(source, target)
                   || IsImplicitNullableConversion(source, target);
        }

        internal static bool IsImplicitNullableConversion(Type source, Type target)
        {
            return target.IsNullable() && source.GetNonNullable().IsImplicitlyConvertibleToInternal(target.GetNonNullable());
        }

        internal static bool IsImplicitReferenceConversion(Type source, Type target)
        {
            return target.IsAssignableFrom(source);
        }

        internal static bool IsLegalExplicitVariantDelegateConversion(Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // There *might* be a legal conversion from a generic delegate type S to generic delegate type  T,
            // provided all of the follow are true:
            //   o Both types are constructed generic types of the same generic delegate type, D<X1,... Xk>.
            //     That is, S = D<S1...>, T = D<T1...>.
            //   o If type parameter Xi is declared to be invariant then Si must be identical to Ti.
            //   o If type parameter Xi is declared to be covariant ("out") then Si must be convertible
            //     to Ti via an identify conversion,  implicit reference conversion, or explicit reference conversion.
            //   o If type parameter Xi is declared to be contravariant ("in") then either Si must be identical to Ti,
            //     or Si and Ti must both be reference types.
            if (!PrivateIsDelegate(source) || !PrivateIsDelegate(target) || !source.IsGenericType || !target.IsGenericType)
            {
                return false;
            }

            var genericDelegate = source.GetGenericTypeDefinition();
            if (target.GetGenericTypeDefinition() != genericDelegate)
            {
                return false;
            }

            var genericParameters = genericDelegate.GetGenericArguments();
            var sourceArguments = source.GetGenericArguments();
            var destArguments = target.GetGenericArguments();
            for (var index = 0; index < genericParameters.Length; index++)
            {
                var sourceArgument = sourceArguments[index];
                var destArgument = destArguments[index];
                if (sourceArgument == destArgument)
                {
                    continue;
                }

                var genericParameter = genericParameters[index];
                if (PrivateIsInvariant(genericParameter))
                {
                    return false;
                }

                if (PrivateIsCovariant(genericParameter))
                {
                    if (!sourceArgument.HasReferenceConversionToInternal(destArgument))
                    {
                        return false;
                    }
                }
                else if (PrivateIsContravariant(genericParameter) && (sourceArgument.IsValueType || destArgument.IsValueType))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsReferenceAssignableFromInternal(this Type type, Type source)
        {
            // This actually implements "Is this identity assignable and/or reference assignable?"
            if (type == source)
            {
                return true;
            }

            return !type.IsValueType
                   && !source.IsValueType
                   && type.IsAssignableFrom(source);
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

        internal static bool IsValidInstanceType(MemberInfo member, Type instanceType)
        {
            var targetType = member.DeclaringType;
            if (targetType == null)
            {
                // Can this happen?
                return false;
            }

            if (targetType.IsReferenceAssignableFromInternal(instanceType))
            {
                return true;
            }

            if (!instanceType.IsValueType)
            {
                return false;
            }

            if (targetType.IsReferenceAssignableFromInternal(typeof(object)))
            {
                return true;
            }

            if (targetType.IsReferenceAssignableFromInternal(typeof(ValueType)))
            {
                return true;
            }

            if (instanceType.IsEnum && targetType.IsReferenceAssignableFromInternal(typeof(Enum)))
            {
                return true;
            }

            // A call to an interface implemented by a struct is legal whether the struct has
            // been boxed or not.
            return targetType.IsInterface && instanceType.GetInterfaces().Any(targetType.IsReferenceAssignableFromInternal);
        }

        internal static bool MatchesArgumentTypes(this MethodInfo method, Type[] argTypes)
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

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < parameters.Length; index++)
            {
                if (!IsReferenceAssignableFromInternal(parameters[index].ParameterType, argTypes[index]))
                {
                    return false;
                }
            }

            return true;
        }

        internal static void ValidateType(Type type, string paramName)
        {
            ValidateType(type, paramName, false, false);
        }

        internal static void ValidateType(Type type, string? paramName, bool allowByRef, bool allowPointer)
        {
            if (!ValidateType(type, paramName, -1))
            {
                return;
            }

            if (!allowByRef && type.IsByRef)
            {
                throw new ArgumentException("type must not be ByRef", paramName);
            }

            if (!allowPointer && type.IsPointer)
            {
                throw new ArgumentException("Type must not be a pointer type", paramName);
            }
        }

        internal static bool ValidateType(Type type, string? paramName, int index)
        {
            if (type == typeof(void))
            {
                return false; // Caller can skip further checks.
            }

            if (type.ContainsGenericParameters)
            {
                var formattedParamName = index >= 0 ? $"{paramName}[{index}]" : paramName;
                throw type.IsGenericTypeDefinition
                    ? new ArgumentException($"Type {type} is a generic type definition", formattedParamName)
                    : new ArgumentException($"Type {type} contains generic parameters", formattedParamName);
            }

            return true;
        }

        private static bool HasArrayToInterfaceConversion(Type source, Type target)
        {
            if (!source.IsSafeArray() || !target.IsInterface || !target.IsGenericType)
            {
                return false;
            }

            var targetParams = target.GetGenericArguments();
            if (targetParams.Length != 1)
            {
                return false;
            }

            var targetGen = target.GetGenericTypeDefinition();
            return _arrayAssignableInterfaces.Any(currentInterface => targetGen == currentInterface) && StrictHasReferenceConversionToInternal(source.GetElementType(), targetParams[0], false);
        }

        private static bool HasInterfaceToArrayConversion(Type source, Type target)
        {
            if (!target.IsSafeArray() || !source.IsInterface || !source.IsGenericType)
            {
                return false;
            }

            var sourceParams = source.GetGenericArguments();
            if (sourceParams.Length != 1)
            {
                return false;
            }

            var sourceGen = source.GetGenericTypeDefinition();
            return _arrayAssignableInterfaces.Any(currentInterface => sourceGen == currentInterface) && StrictHasReferenceConversionToInternal(sourceParams[0], target.GetElementType(), false);
        }

        private static bool PrivateIsContravariant(Type type)
        {
            return 0 != (type.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
        }

        private static bool PrivateIsCovariant(Type type)
        {
            return 0 != (type.GenericParameterAttributes & GenericParameterAttributes.Covariant);
        }

        private static bool PrivateIsDelegate(Type type)
        {
            return type.IsSubclassOf(typeof(MulticastDelegate));
        }

        private static bool PrivateIsInvariant(Type type)
        {
            return 0 == (type.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
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
                    if (source.IsValueType)
                    {
                        return false;
                    }

                    // ReSharper disable once PossibleNullReferenceException
                    if (target.IsValueType)
                    {
                        return false;
                    }

                    // Includes to case of either being typeof(object)
                    if
                    (
                        source.IsAssignableFrom(target)
                        || target.IsAssignableFrom(source)
                    )
                    {
                        return true;
                    }

                    if (source.IsInterface)
                    {
                        if (target.IsInterface || (target.IsClass && !target.IsSealed))
                        {
                            return true;
                        }
                    }
                    else if (target.IsInterface && source.IsClass && !source.IsSealed)
                    {
                        return true;
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
                else
                {
                    return target.IsArray ? HasInterfaceToArrayConversion(source, target) || IsImplicitReferenceConversion(typeof(Array), source) : IsLegalExplicitVariantDelegateConversion(source, target);
                }
            }
        }
    }
}

#endif