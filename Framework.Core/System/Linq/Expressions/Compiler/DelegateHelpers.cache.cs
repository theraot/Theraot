#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Collections;
using Theraot.Reflection;

namespace System.Linq.Expressions.Compiler
{
    internal static partial class DelegateHelpers
    {
        private const int _maximumArity = 17;

        private static readonly TypeInfo _delegateCache = new TypeInfo();

        internal static TypeInfo GetNextTypeInfo(Type initialArg, TypeInfo curTypeInfo)
        {
            lock (_delegateCache)
            {
                return NextTypeInfo(initialArg, curTypeInfo);
            }
        }

        internal static Type MakeDelegateType(Type[] types)
        {
            lock (_delegateCache)
            {
                // arguments & return type
                var curTypeInfo = types.Aggregate(_delegateCache, (current, type) => NextTypeInfo(type, current));

                // see if we have the delegate already
                // clone because MakeCustomDelegate can hold onto the array.
                return curTypeInfo.DelegateType ?? (curTypeInfo.DelegateType = MakeNewDelegate((Type[])types.Clone()));
            }
        }

        internal static Type MakeNewDelegate(Type[] types)
        {
            Debug.Assert(types?.Length > 0);

            // Can only used predefined delegates if we have no byref types and
            // the arity is small enough to fit in Func<...> or Action<...>

            var needCustom = types.Length > _maximumArity || types.Any(type => type.IsByRef || /*type.IsByRefLike ||*/ type.IsPointer);

            if (needCustom)
            {
                return MakeNewCustomDelegate(types);
            }

            var result = types[types.Length - 1] == typeof(void) ? DelegateBuilder.GetActionType(types.RemoveLast()) : DelegateBuilder.GetFuncType(types);

            Debug.Assert(result != null);
            return result;
        }

        internal static TypeInfo NextTypeInfo(Type initialArg)
        {
            lock (_delegateCache)
            {
                return NextTypeInfo(initialArg, _delegateCache);
            }
        }

        private static TypeInfo NextTypeInfo(Type initialArg, TypeInfo curTypeInfo)
        {
            var lookingUp = initialArg;
            if (curTypeInfo.TypeChain == null)
            {
                curTypeInfo.TypeChain = new Dictionary<Type, TypeInfo>();
            }

            if (!curTypeInfo.TypeChain.TryGetValue(lookingUp, out var nextTypeInfo))
            {
                nextTypeInfo = new TypeInfo();
                /*if (!lookingUp.IsCollectible)
                {
                    curTypeInfo.TypeChain[lookingUp] = nextTypeInfo;
                }*/
            }

            return nextTypeInfo;
        }

        internal class TypeInfo
        {
            public Type DelegateType;
            public Dictionary<Type, TypeInfo> TypeChain;
        }
    }
}

#endif