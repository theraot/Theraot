// Needed for NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Theraot.Reflection
{
    public static partial class TypeHelper
    {
        public static bool AreReferenceAssignable(Type target, Type source)
        {
            // This actually implements "Is this identity assignable and/or reference assignable?"
            if (target == source)
            {
                return true;
            }
            var targetInfo = target.GetTypeInfo();
            var sourceInfo = source.GetTypeInfo();
            if (!targetInfo.IsValueType && !sourceInfo.IsValueType && target.IsAssignableFrom(source))
            {
                return true;
            }
            return false;
        }

        public static Type FindGenericType(Type definition, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsConstructedGenericType() && type.GetGenericTypeDefinition() == definition)
                {
                    return type;
                }
                var definitionInfo = definition.GetTypeInfo();
                var info = type.GetTypeInfo();
                if (definitionInfo.IsInterface)
                {
                    foreach (var interfaceType in info.GetInterfaces())
                    {
                        var found = FindGenericType(definition, interfaceType);
                        if (found != null)
                        {
                            return found;
                        }
                    }
                }
                type = info.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Searches for an operator method on the type. The method must have
        /// the specified signature, no generic arguments, and have the
        /// SpecialName bit set. Also searches inherited operator methods.
        /// </summary>
        /// <remarks>
        /// This was designed to satisfy the needs of op_True and
        /// op_False, because we have to do runtime lookup for those. It may
        /// not work right for unary operators in general.
        /// </remarks>
        /// <param name="type">The type to search for the operator.</param>
        /// <param name="name">The name of the operator. Either "op_True" or "op_False".</param>
        public static MethodInfo GetBooleanOperator(Type type, string name)
        {
            do
            {
                var result = type.GetStaticMethod(name, new[] { type });
                if (result != null && result.IsSpecialName && !result.ContainsGenericParameters)
                {
                    return result;
                }
                var info = type.GetTypeInfo();
                type = info.BaseType;
            } while (type != null);
            return null;
        }

        public static MethodInfo GetUserDefinedConversionMethod(Type source, Type target, bool implicitOnly)
        {
            var nonNullableSource = source.GetNonNullable();
            var nonNullableTarget = target.GetNonNullable();
            MethodInfo[] sourceStaticMethods;
            MethodInfo[] targetStaticMethods;
            if (nonNullableSource == source)
            {
                if (nonNullableTarget == target)
                {
                    return FindConversionOperator(nonNullableSource.GetStaticMethods(), source, target, implicitOnly)
                        ?? FindConversionOperator(nonNullableTarget.GetStaticMethods(), source, target, implicitOnly);
                }
                return FindConversionOperator(sourceStaticMethods = nonNullableSource.GetStaticMethods(), source, target, implicitOnly)
                    ?? FindConversionOperator(targetStaticMethods = nonNullableTarget.GetStaticMethods(), source, target, implicitOnly)
                    ?? FindConversionOperator(sourceStaticMethods, source, nonNullableTarget, implicitOnly)
                    ?? FindConversionOperator(targetStaticMethods, source, nonNullableTarget, implicitOnly);
            }
            if (nonNullableTarget == target)
            {
                return FindConversionOperator(sourceStaticMethods = nonNullableSource.GetStaticMethods(), source, target, implicitOnly)
                    ?? FindConversionOperator(targetStaticMethods = nonNullableTarget.GetStaticMethods(), source, target, implicitOnly)
                    ?? FindConversionOperator(sourceStaticMethods, nonNullableSource, target, implicitOnly)
                    ?? FindConversionOperator(targetStaticMethods, nonNullableSource, target, implicitOnly);
            }
            return FindConversionOperator(sourceStaticMethods = nonNullableSource.GetStaticMethods(), source, target, implicitOnly)
                ?? FindConversionOperator(targetStaticMethods = nonNullableTarget.GetStaticMethods(), source, target, implicitOnly)
                ?? FindConversionOperator(sourceStaticMethods, nonNullableSource, target, implicitOnly)
                ?? FindConversionOperator(targetStaticMethods, nonNullableSource, target, implicitOnly)
                ?? FindConversionOperator(sourceStaticMethods, source, nonNullableTarget, implicitOnly)
                ?? FindConversionOperator(targetStaticMethods, source, nonNullableTarget, implicitOnly)
                ?? FindConversionOperator(sourceStaticMethods, nonNullableSource, nonNullableTarget, implicitOnly)
                ?? FindConversionOperator(targetStaticMethods, nonNullableSource, nonNullableTarget, implicitOnly);
        }

        public static bool HasBuiltInEqualityOperator(Type left, Type right)
        {
            var leftInfo = left.GetTypeInfo();
            var rightInfo = right.GetTypeInfo();
            if (leftInfo.IsInterface && !rightInfo.IsValueType)
            {
                return true;
            }
            if (rightInfo.IsInterface && !leftInfo.IsValueType)
            {
                return true;
            }
            if (!leftInfo.IsValueType && !rightInfo.IsValueType)
            {
                if (left.IsReferenceAssignableFrom(right) || right.IsReferenceAssignableFrom(left))
                {
                    return true;
                }
            }
            if (left != right)
            {
                return false;
            }
            var notNullable = left.GetNonNullable();
            var info = notNullable.GetTypeInfo();
            if (notNullable == typeof(bool) || notNullable.IsNumeric() || info.IsEnum)
            {
                return true;
            }
            return false;
        }

        public static bool HasReferenceEquality(Type left, Type right)
        {
            var leftInfo = left.GetTypeInfo();
            var rightInfo = right.GetTypeInfo();
            if (leftInfo.IsValueType || rightInfo.IsValueType)
            {
                return false;
            }
            // If we have an interface and a reference type then we can do
            // reference equality.
            // If we have two reference types and one is assignable to the
            // other then we can do reference equality.
            return leftInfo.IsInterface
                || rightInfo.IsInterface
                || left.IsReferenceAssignableFrom(right)
                || right.IsReferenceAssignableFrom(left);
        }

        public static bool IsConvertible(Type type)
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

        public static bool IsImplicitlyConvertible(Type source, Type target)
        {
            return source == target
                || IsImplicitNumericConversion(source, target)
                || IsImplicitReferenceConversion(source, target)
                || IsImplicitBoxingConversion(source, target)
                || IsImplicitNullableConversion(source, target);
        }

        public static bool IsImplicitNullableConversion(Type source, Type target)
        {
            if (target.IsNullable())
            {
                return IsImplicitlyConvertible(source.GetNonNullable(), target.GetNonNullable());
            }
            return false;
        }

        public static bool IsImplicitReferenceConversion(Type source, Type target)
        {
            return target.IsAssignableFrom(source);
        }

        public static bool IsLegalExplicitVariantDelegateConversion(Type source, Type target)
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
            var sourceInfo = source.GetTypeInfo();
            var targetInfo = target.GetTypeInfo();
            if (!PrivateIsDelegate(source) || !PrivateIsDelegate(target) || !sourceInfo.IsGenericType || !targetInfo.IsGenericType)
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
                    if (!sourceArgument.HasReferenceConversionTo(destArgument))
                    {
                        return false;
                    }
                }
                else if (PrivateIsContravariant(genericParameter))
                {
                    var sourceArgumentInfo = sourceArgument.GetTypeInfo();
                    var destArgumentInfo = destArgument.GetTypeInfo();
                    if (sourceArgumentInfo.IsValueType || destArgumentInfo.IsValueType)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsValidInstanceType(MemberInfo member, Type instanceType)
        {
            var targetType = member.DeclaringType;
            if (targetType == null)
            {
                // Can this happen?
                return false;
            }
            if (targetType.IsReferenceAssignableFrom(instanceType))
            {
                return true;
            }
            var instanceInfo = instanceType.GetTypeInfo();
            if (instanceInfo.IsValueType)
            {
                if (targetType.IsReferenceAssignableFrom(typeof(object)))
                {
                    return true;
                }
                if (targetType.IsReferenceAssignableFrom(typeof(ValueType)))
                {
                    return true;
                }
                if (instanceInfo.IsEnum && targetType.IsReferenceAssignableFrom(typeof(Enum)))
                {
                    return true;
                }
                // A call to an interface implemented by a struct is legal whether the struct has
                // been boxed or not.
                var targetInfo = targetType.GetTypeInfo();
                if (targetInfo.IsInterface)
                {
                    foreach (var interfaceType in instanceType.GetInterfaces())
                    {
                        if (targetType.IsReferenceAssignableFrom(interfaceType))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static void ValidateType(Type type)
        {
            if (type != typeof(void))
            {
                // A check to avoid a bunch of reflection (currently not supported) during cctor
                var info = type.GetTypeInfo();
                if (info.IsGenericTypeDefinition)
                {
                    throw new ArgumentException("type is Generic");
                }
                if (info.ContainsGenericParameters)
                {
                    throw new ArgumentException("type contains generic parameters.");
                }
            }
        }
    }

    public static partial class TypeHelper
    {
#if NET20 || NET30 || NET35 || NET40 || NET45

        /// <summary>
        /// Creates a closed delegate for the given (dynamic)method.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo for the target method.</param>
        /// <param name="delegateType">Delegate type with a matching signature.</param>
        public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType)
        {
            if (methodInfo is System.Reflection.Emit.DynamicMethod dynamicMethod)
            {
                return dynamicMethod.CreateDelegate(delegateType);
            }
            return Delegate.CreateDelegate(delegateType, methodInfo);
        }

        /// <summary>
        /// Creates a closed delegate for the given (dynamic)method.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo for the target method.</param>
        /// <param name="delegateType">Delegate type with a matching signature.</param>
        /// <param name="target">The object to which the delegate is bound, or null to treat method as static.</param>
        public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target)
        {
            if (methodInfo is System.Reflection.Emit.DynamicMethod dynamicMethod)
            {
                return dynamicMethod.CreateDelegate(delegateType, target);
            }
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }

#endif
    }
}