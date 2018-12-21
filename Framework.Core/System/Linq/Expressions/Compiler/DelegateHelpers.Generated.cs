#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Theraot.Collections;

namespace System.Linq.Expressions.Compiler
{
    internal static partial class DelegateHelpers
    {
        private const int MaximumArity = 17;

        private static TypeInfo _delegateCache = new TypeInfo();

        internal static Type GetActionType(Type[] types)
        {
            switch (types.Length)
            {
                case 0:
                    return typeof(Action);

                case 1:
                    return typeof(Action<>).MakeGenericType(types);

                case 2:
                    return typeof(Action<,>).MakeGenericType(types);

                case 3:
                    return typeof(Action<,,>).MakeGenericType(types);

                case 4:
                    return typeof(Action<,,,>).MakeGenericType(types);

                case 5:
                    return typeof(Action<,,,,>).MakeGenericType(types);

                case 6:
                    return typeof(Action<,,,,,>).MakeGenericType(types);

                case 7:
                    return typeof(Action<,,,,,,>).MakeGenericType(types);

                case 8:
                    return typeof(Action<,,,,,,,>).MakeGenericType(types);

                case 9:
                    return typeof(Action<,,,,,,,,>).MakeGenericType(types);

                case 10:
                    return typeof(Action<,,,,,,,,,>).MakeGenericType(types);

                case 11:
                    return typeof(Action<,,,,,,,,,,>).MakeGenericType(types);

                case 12:
                    return typeof(Action<,,,,,,,,,,,>).MakeGenericType(types);

                case 13:
                    return typeof(Action<,,,,,,,,,,,,>).MakeGenericType(types);

                case 14:
                    return typeof(Action<,,,,,,,,,,,,,>).MakeGenericType(types);

                case 15:
                    return typeof(Action<,,,,,,,,,,,,,,>).MakeGenericType(types);

                case 16:
                    return typeof(Action<,,,,,,,,,,,,,,,>).MakeGenericType(types);

                default:
                    return null;
            }
        }

        internal static Type GetFuncType(Type[] types)
        {
            switch (types.Length)
            {
                case 1:
                    return typeof(Func<>).MakeGenericType(types);

                case 2:
                    return typeof(Func<,>).MakeGenericType(types);

                case 3:
                    return typeof(Func<,,>).MakeGenericType(types);

                case 4:
                    return typeof(Func<,,,>).MakeGenericType(types);

                case 5:
                    return typeof(Func<,,,,>).MakeGenericType(types);

                case 6:
                    return typeof(Func<,,,,,>).MakeGenericType(types);

                case 7:
                    return typeof(Func<,,,,,,>).MakeGenericType(types);

                case 8:
                    return typeof(Func<,,,,,,,>).MakeGenericType(types);

                case 9:
                    return typeof(Func<,,,,,,,,>).MakeGenericType(types);

                case 10:
                    return typeof(Func<,,,,,,,,,>).MakeGenericType(types);

                case 11:
                    return typeof(Func<,,,,,,,,,,>).MakeGenericType(types);

                case 12:
                    return typeof(Func<,,,,,,,,,,,>).MakeGenericType(types);

                case 13:
                    return typeof(Func<,,,,,,,,,,,,>).MakeGenericType(types);

                case 14:
                    return typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(types);

                case 15:
                    return typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(types);

                case 16:
                    return typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(types);

                case 17:
                    return typeof(Func<,,,,,,,,,,,,,,,,>).MakeGenericType(types);

                default:
                    return null;
            }
        }

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
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
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
                result = GetActionType(types.RemoveLast());
            }
            else
            {
                result = GetFuncType(types);
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