#if NET20 || NET30 || NET35 || NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Theraot.Reflection;

namespace System.Dynamic.Utils
{
    internal static class TypeUtils
    {
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
    }
}

#endif