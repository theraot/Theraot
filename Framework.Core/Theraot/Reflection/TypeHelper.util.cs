// Needed for NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Theraot.Reflection
{
    public static partial class TypeHelper
    {
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
                    return FindConversionOperator(nonNullableSource.GetStaticMethodsInternal(), source, target, implicitOnly)
                        ?? FindConversionOperator(nonNullableTarget.GetStaticMethodsInternal(), source, target, implicitOnly);
                }
                return FindConversionOperator(sourceStaticMethods = nonNullableSource.GetStaticMethodsInternal(), source, target, implicitOnly)
                    ?? FindConversionOperator(targetStaticMethods = nonNullableTarget.GetStaticMethodsInternal(), source, target, implicitOnly)
                    ?? FindConversionOperator(sourceStaticMethods, source, nonNullableTarget, implicitOnly)
                    ?? FindConversionOperator(targetStaticMethods, source, nonNullableTarget, implicitOnly);
            }
            if (nonNullableTarget == target)
            {
                return FindConversionOperator(sourceStaticMethods = nonNullableSource.GetStaticMethodsInternal(), source, target, implicitOnly)
                    ?? FindConversionOperator(targetStaticMethods = nonNullableTarget.GetStaticMethodsInternal(), source, target, implicitOnly)
                    ?? FindConversionOperator(sourceStaticMethods, nonNullableSource, target, implicitOnly)
                    ?? FindConversionOperator(targetStaticMethods, nonNullableSource, target, implicitOnly);
            }
            return FindConversionOperator(sourceStaticMethods = nonNullableSource.GetStaticMethodsInternal(), source, target, implicitOnly)
                ?? FindConversionOperator(targetStaticMethods = nonNullableTarget.GetStaticMethodsInternal(), source, target, implicitOnly)
                ?? FindConversionOperator(sourceStaticMethods, nonNullableSource, target, implicitOnly)
                ?? FindConversionOperator(targetStaticMethods, nonNullableSource, target, implicitOnly)
                ?? FindConversionOperator(sourceStaticMethods, source, nonNullableTarget, implicitOnly)
                ?? FindConversionOperator(targetStaticMethods, source, nonNullableTarget, implicitOnly)
                ?? FindConversionOperator(sourceStaticMethods, nonNullableSource, nonNullableTarget, implicitOnly)
                ?? FindConversionOperator(targetStaticMethods, nonNullableSource, nonNullableTarget, implicitOnly);
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
        internal static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType)
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
        internal static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target)
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