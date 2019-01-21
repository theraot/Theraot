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
        private const int MaximumArity = 17;

        private static TypeInfo _delegateCache = new TypeInfo();

        internal static TypeInfo GetNextTypeInfo(Type initialArg, TypeInfo curTypeInfo)
        {
            lock (_delegateCache)
            {
                return NextTypeInfo(initialArg, curTypeInfo);
            }
        }

        /// <summary>
        /// Finds a delegate type using the types in the array.
        /// We use the cache to avoid copying the array, and to cache the
        /// created delegate type
        /// </summary>
        internal static Type MakeDelegateType(Type[] types)
        {
            lock (_delegateCache)
            {
                TypeInfo curTypeInfo = _delegateCache;

                // arguments & return type
                for (int i = 0; i < types.Length; i++)
                {
                    curTypeInfo = NextTypeInfo(types[i], curTypeInfo);
                }

                // see if we have the delegate already
                if (curTypeInfo.DelegateType == null)
                {
                    // clone because MakeCustomDelegate can hold onto the array.
                    curTypeInfo.DelegateType = MakeNewDelegate((Type[])types.Clone());
                }

                return curTypeInfo.DelegateType;
            }
        }

        /// <summary>
        /// Creates a new delegate, or uses a func/action
        /// Note: this method does not cache
        /// </summary>
        internal static Type MakeNewDelegate(Type[] types)
        {
            Debug.Assert(types != null && types.Length > 0);

            // Can only used predefined delegates if we have no byref types and
            // the arity is small enough to fit in Func<...> or Action<...>
            bool needCustom;

            if (types.Length > MaximumArity)
            {
                needCustom = true;
            }
            else
            {
                needCustom = false;

                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (type.IsByRef || /*type.IsByRefLike ||*/ type.IsPointer)
                    {
                        needCustom = true;
                        break;
                    }
                }
            }

            if (needCustom)
            {
                return MakeNewCustomDelegate(types);
            }

            Type result;
            if (types[types.Length - 1] == typeof(void))
            {
                result = DelegateBuilder.GetActionType(types.RemoveLast());
            }
            else
            {
                result = DelegateBuilder.GetFuncType(types);
            }

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
            Type lookingUp = initialArg;
            TypeInfo nextTypeInfo;
            if (curTypeInfo.TypeChain == null)
            {
                curTypeInfo.TypeChain = new Dictionary<Type, TypeInfo>();
            }

            if (!curTypeInfo.TypeChain.TryGetValue(lookingUp, out nextTypeInfo))
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