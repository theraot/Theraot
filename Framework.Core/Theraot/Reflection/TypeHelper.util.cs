// Needed for NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Theraot.Reflection
{
    public static partial class TypeHelper
    {
        public static MethodInfo? GetUserDefinedConversionMethod(Type source, Type target, bool implicitOnly)
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
}