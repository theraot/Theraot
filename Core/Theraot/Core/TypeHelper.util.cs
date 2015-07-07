// Needed for NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Theraot.Core
{
    public static partial class TypeHelper
    {
        public static bool AreEquivalent(Type t1, Type t2)
        {
            return t1.IsEquivalentTo(t2);
        }

        public static bool AreReferenceAssignable(Type target, Type source)
        {
            // This actually implements "Is this identity assignable and/or reference assignable?"
            if (AreEquivalent(target, source))
            {
                return true;
            }
            if (!target.IsValueType && !source.IsValueType && target.IsAssignableFrom(source))
            {
                return true;
            }
            return false;
        }

        public static MethodInfo FindConversionOperator(MethodInfo[] methods, Type typeFrom, Type typeTo, bool implicitOnly)
        {
            foreach (MethodInfo mi in methods)
            {
                if (mi.Name != "op_Implicit" && (implicitOnly || mi.Name != "op_Explicit"))
                {
                    continue;
                }
                if (!AreEquivalent(mi.ReturnType, typeTo))
                {
                    continue;
                }
                ParameterInfo[] pis = mi.GetParameters();
                if (!AreEquivalent(pis[0].ParameterType, typeFrom))
                {
                    continue;
                }
                return mi;
            }
            return null;
        }

        public static Type FindGenericType(Type definition, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (IsConstructedGenericType(type) && AreEquivalent(type.GetGenericTypeDefinition(), definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type itype in type.GetInterfaces())
                    {
                        Type found = FindGenericType(definition, itype);
                        if (found != null)
                            return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        public static MethodInfo GetAnyStaticMethod(this Type type, string name)
        {
            foreach (var method in type.GetMethods())
            {
                if (method.IsStatic && method.Name == name)
                {
                    return method;
                }
            }
            return null;
        }

        public static MethodInfo GetAnyStaticMethodValidated(
            this Type type,
            string name,
            Type[] types)
        {
            var method = type.GetAnyStaticMethod(name);

            return method.MatchesArgumentTypes(types) ? method : null;
        }

        /// <summary>
        /// Searches for an operator method on the type. The method must have
        /// the specified signature, no generic arguments, and have the
        /// SpecialName bit set. Also searches inherited operator methods.
        ///
        /// NOTE: This was designed to satisfy the needs of op_True and
        /// op_False, because we have to do runtime lookup for those. It may
        /// not work right for unary operators in general.
        ///// </summary>
        public static MethodInfo GetBooleanOperator(Type type, string name)
        {
            do
            {
                MethodInfo result = type.GetAnyStaticMethodValidated(name, new Type[] { type });
                if (result != null && result.IsSpecialName && !result.ContainsGenericParameters)
                {
                    return result;
                }
                type = type.BaseType;
            } while (type != null);
            return null;
        }

        public static MethodInfo[] GetMethodsIgnoreCase(this Type type, BindingFlags flags, string name)
        {
            var list = new List<MethodInfo>();
            foreach (var method in type.GetMethods())
            {
                // TODO: Binding flags filter
                if (method.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(method);
                }
            }
            return list.ToArray();
        }

        public static Type GetNonNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static Type GetNonRefType(this Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        public static Type GetNullableType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type.IsValueType && !IsNullableType(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
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

        public static MethodInfo GetUserDefinedCoercionMethod(Type convertFrom, Type convertToType, bool implicitOnly)
        {
            // check for implicit coercions first
            Type nnExprType = GetNonNullableType(convertFrom);
            Type nnConvType = GetNonNullableType(convertToType);
            // try exact match on types
            MethodInfo[] eMethods = nnExprType.GetStaticMethods();
            MethodInfo method = FindConversionOperator(eMethods, convertFrom, convertToType, implicitOnly);
            if (method != null)
            {
                return method;
            }
            MethodInfo[] cMethods = nnConvType.GetStaticMethods();
            method = FindConversionOperator(cMethods, convertFrom, convertToType, implicitOnly);
            if (method != null)
            {
                return method;
            }
            // try lifted conversion
            if (!AreEquivalent(nnExprType, convertFrom) ||
                !AreEquivalent(nnConvType, convertToType))
            {
                method = FindConversionOperator(eMethods, nnExprType, nnConvType, implicitOnly);
                if (method == null)
                {
                    method = FindConversionOperator(cMethods, nnExprType, nnConvType, implicitOnly);
                }
                if (method != null)
                {
                    return method;
                }
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
            Type nnType = GetNonNullableType(left);
            if (nnType == typeof(bool) || IsNumeric(nnType) || nnType.IsEnum)
            {
                return true;
            }
            return false;
        }

        public static bool HasIdentityPrimitiveOrNullableConversion(Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            // Identity conversion
            if (AreEquivalent(source, target))
            {
                return true;
            }

            // Nullable conversions
            if (IsNullableType(source) && AreEquivalent(target, GetNonNullableType(source)))
            {
                return true;
            }
            if (IsNullableType(target) && AreEquivalent(source, GetNonNullableType(target)))
            {
                return true;
            }
            // Primitive runtime conversions
            // All conversions amongst enum, bool, char, integer and float types
            // (and their corresponding nullable types) are legal except for
            // nonbool==>bool and nonbool==>bool?
            // Since we have already covered bool==>bool, bool==>bool?, etc, above,
            // we can just disallow having a bool or bool? target type here.
            if (IsConvertible(source) && IsConvertible(target) && GetNonNullableType(target) != typeof(bool))
            {
                return true;
            }
            return false;
        }

        public static bool HasReferenceConversion(Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            // void -> void conversion is handled elsewhere
            // (it's an identity conversion)
            // All other void conversions are disallowed.
            if (source == typeof(void) || target == typeof(void))
            {
                return false;
            }

            Type nnSourceType = GetNonNullableType(source);
            Type nnDestType = GetNonNullableType(target);

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
            if (source.IsInterface || target.IsInterface)
            {
                return true;
            }
            // Variant delegate conversion
            if (IsLegalExplicitVariantDelegateConversion(source, target))
                return true;

            // Object conversion
            if (source == typeof(object) || target == typeof(object))
            {
                return true;
            }
            return false;
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

            return left.IsInterface || right.IsInterface ||
                AreReferenceAssignable(left, right) ||
                AreReferenceAssignable(right, left);
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

        public static bool IsBool(this Type type)
        {
            return GetNonNullableType(type) == typeof(bool);
        }

        public static bool IsConstructedGenericType(this Type type)
        {
            return type.IsGenericType && !type.IsGenericTypeDefinition;
        }

        public static bool IsContravariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return PrivateIsContravariant(type);
        }

        public static bool IsConvertible(Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
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

        public static bool IsCovariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return PrivateIsCovariant(type);
        }

        public static bool IsDelegate(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return PrivateIsDelegate(type);
        }

        public static bool IsEquivalentTo(this Type t1, Type t2)
        {
            return t1 == t2;
        }

        public static bool IsImplicitBoxingConversion(Type source, Type target)
        {
            if (source.IsValueType && (target == typeof(object) || target == typeof(System.ValueType)))
                return true;
            if (source.IsEnum && target == typeof(System.Enum))
                return true;
            return false;
        }

        public static bool IsImplicitlyConvertible(Type source, Type target)
        {
            return AreEquivalent(source, target) ||                // identity conversion
                IsImplicitNumericConversion(source, target) ||
                IsImplicitReferenceConversion(source, target) ||
                IsImplicitBoxingConversion(source, target) ||
                IsImplicitNullableConversion(source, target);
        }

        public static bool IsImplicitNullableConversion(Type source, Type target)
        {
            if (IsNullableType(target))
                return IsImplicitlyConvertible(GetNonNullableType(source), GetNonNullableType(target));
            return false;
        }

        public static bool IsImplicitNumericConversion(Type source, Type target)
        {
            if (source == typeof(sbyte))
            {
                if (target == typeof(short) || target == typeof(int) || target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
                {
                    return true;
                }
            }
            else if (source == typeof(byte))
            {
                if (target == typeof(short) || target == typeof(ushort) || target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
                {
                    return true;
                }
            }
            else if (source == typeof(short))
            {
                if (target == typeof(int) || target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
                {
                    return true;
                }
            }
            else if (source == typeof(ushort))
            {
                if (target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
                {
                    return true;
                }
            }
            else if (source == typeof(int))
            {
                if (target == typeof(long) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
                {
                    return true;
                }
            }
            else if (source == typeof(uint))
            {
                if (target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
                {
                    return true;
                }
            }
            else if (source == typeof(long) || target == typeof(ulong))
            {
                if (target == typeof(float) || target == typeof(double) || target == typeof(decimal))
                {
                    return true;
                }
            }
            else if (source == typeof(char))
            {
                if (target == typeof(ushort) || target == typeof(int) || target == typeof(uint) || target == typeof(long) || target == typeof(ulong) || target == typeof(float) || target == typeof(double) || target == typeof(decimal))
                {
                    return true;
                }
            }
            else if (source == typeof(float))
            {
                return (target == typeof(double));
            }
            return false;
        }

        public static bool IsImplicitReferenceConversion(Type source, Type target)
        {
            return target.IsAssignableFrom(source);
        }

        public static bool IsInteger(this Type type)
        {
            type = GetNonNullableType(type);
            return type.IsPrimitiveInteger();
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

        public static bool IsInvariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return PrivateIsInvariant(type);
        }

        public static bool IsLegalExplicitVariantDelegateConversion(Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
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
                return false;

            Type genericDelegate = source.GetGenericTypeDefinition();

            if (target.GetGenericTypeDefinition() != genericDelegate)
                return false;

            Type[] genericParameters = genericDelegate.GetGenericArguments();
            Type[] sourceArguments = source.GetGenericArguments();
            Type[] destArguments = target.GetGenericArguments();

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

                if (PrivateIsInvariant(genericParameter))
                {
                    return false;
                }

                if (PrivateIsCovariant(genericParameter))
                {
                    if (!HasReferenceConversion(sourceArgument, destArgument))
                    {
                        return false;
                    }
                }
                else if (PrivateIsContravariant(genericParameter))
                {
                    if (sourceArgument.IsValueType || destArgument.IsValueType)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsNullableType(this Type type)
        {
            return IsConstructedGenericType(type) && type.GetGenericTypeDefinition() == typeof(Nullable<>);
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

        public static bool IsUnsignedInteger(this Type type)
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

        // Checks if the type is a valid target for an instance call
        public static bool IsValidInstanceType(MemberInfo member, Type instanceType)
        {
            Type targetType = member.DeclaringType;
            if (AreReferenceAssignable(targetType, instanceType))
            {
                return true;
            }
            if (instanceType.IsValueType)
            {
                if (AreReferenceAssignable(targetType, typeof(System.Object)))
                {
                    return true;
                }
                if (AreReferenceAssignable(targetType, typeof(System.ValueType)))
                {
                    return true;
                }
                if (instanceType.IsEnum && AreReferenceAssignable(targetType, typeof(System.Enum)))
                {
                    return true;
                }
                // A call to an interface implemented by a struct is legal whether the struct has
                // been boxed or not.
                if (targetType.IsInterface)
                {
                    foreach (Type interfaceType in instanceType.GetInterfaces())
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

        public static bool MatchesArgumentTypes(this MethodInfo mi, Type[] argTypes)
        {
            if (mi == null || argTypes == null)
            {
                return false;
            }
            var ps = mi.GetParameters();

            if (ps.Length != argTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < ps.Length; i++)
            {
                if (!AreReferenceAssignable(ps[i].ParameterType, argTypes[i]))
                {
                    return false;
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

        internal static void ValidateType(Type type)
        {
            if (type != typeof(void))
            {
                // A check to avoid a bunch of reflection (currently not supported) during cctor
                if (type.IsGenericTypeDefinition)
                {
                    throw new ArgumentException("type is Generic");
                }
                if (type.ContainsGenericParameters)
                {
                    throw new ArgumentException("type contains generic parameters.");
                }
            }
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
            return type.IsSubclassOf(typeof(System.MulticastDelegate));
        }

        private static bool PrivateIsInvariant(Type type)
        {
            return 0 == (type.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        }
    }
}