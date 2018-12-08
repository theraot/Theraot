// Needed for NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

namespace Theraot.Core
{
    public static partial class TypeHelper
    {
        private static readonly Type[] _known = {
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

        private static readonly Assembly[] _knownAssemblies;

        static TypeHelper()
        {
            var assemblies = new List<Assembly>();
            foreach (var type in _known)
            {
                var info = type.GetTypeInfo();
                var assembly = info.Assembly;
                if (!assemblies.Contains(assembly))
                {
                    assemblies.Add(assembly);
                }
            }
            _knownAssemblies = assemblies.ToArray();
        }

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
                if (IsConstructedGenericType(type) && type.GetGenericTypeDefinition() == definition)
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
            if (IsNullable(type))
            {
                return type.GetGenericArguments()[0];
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

        public static MethodInfo GetUserDefinedConversionMethod(Type source, Type target, bool implicitOnly)
        {
            var nonNullableSource = GetNonNullableType(source);
            var nonNullableTarget = GetNonNullableType(target);
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
                if (IsReferenceAssignableFrom(left, right) || IsReferenceAssignableFrom(right, left))
                {
                    return true;
                }
            }
            if (left != right)
            {
                return false;
            }
            var notNullable = GetNonNullableType(left);
            var info = notNullable.GetTypeInfo();
            if (notNullable == typeof(bool) || IsNumeric(notNullable) || info.IsEnum)
            {
                return true;
            }
            return false;
        }

        public static bool HasIdentityPrimitiveOrNullableConversion(Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // Identity conversion
            if (source == target)
            {
                return true;
            }

            // Nullable conversions
            if (IsNullable(source) && target == GetNonNullableType(source))
            {
                return true;
            }
            if (IsNullable(target) && source == GetNonNullableType(target))
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
                throw new ArgumentNullException(nameof(source));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            // void -> void is  an identity conversion, not a reference conversion
            if (source == typeof(void) || target == typeof(void))
            {
                return false;
            }
            var nonNullableSource = GetNonNullableType(source);
            var nonNullableTarget = GetNonNullableType(target);
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
            if (IsLegalExplicitVariantDelegateConversion(source, target))
            {
                return true;
            }
            // Object conversion
            if (source == typeof(object) || target == typeof(object))
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
                || IsReferenceAssignableFrom(left, right)
                || IsReferenceAssignableFrom(right, left);
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

        public static bool IsConvertible(Type type)
        {
            type = GetNonNullableType(type);
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
            if (IsNullable(target))
            {
                return IsImplicitlyConvertible(GetNonNullableType(source), GetNonNullableType(target));
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
            if (target.GetGenericTypeDefinition() != genericDelegate || genericDelegate == null /* Can this happen? */)
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
                    if (!HasReferenceConversion(sourceArgument, destArgument))
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

        public static bool IsValidInstanceType(MemberInfo member, Type instanceType)
        {
            var targetType = member.DeclaringType;
            if (targetType == null)
            {
                // Can this happen?
                return false;
            }
            if (IsReferenceAssignableFrom(targetType, instanceType))
            {
                return true;
            }
            var instanceInfo = instanceType.GetTypeInfo();
            if (instanceInfo.IsValueType)
            {
                if (IsReferenceAssignableFrom(targetType, typeof(object)))
                {
                    return true;
                }
                if (IsReferenceAssignableFrom(targetType, typeof(ValueType)))
                {
                    return true;
                }
                if (instanceInfo.IsEnum && IsReferenceAssignableFrom(targetType, typeof(Enum)))
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
                        if (IsReferenceAssignableFrom(targetType, interfaceType))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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

        // Expression trees/compiler just use IsByRef, why do we need this?
        // (see LambdaCompiler.EmitArguments for usage in the compiler)
        internal static bool IsByRefParameter(this ParameterInfo pi)
        {
            // not using IsIn/IsOut properties as they are not available in Silverlight:
            if (pi.ParameterType.IsByRef)
            {
                return true;
            }

            return (pi.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out;
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