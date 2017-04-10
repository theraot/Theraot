#define FEATURE_CORECLR

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Theraot.Collections;

namespace System.Linq.Expressions.Compiler
{
    internal static partial class DelegateHelpers
    {
#if FEATURE_CORECLR
        private const MethodAttributes _ctorAttributes = MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
        private const MethodImplAttributes _implAttributes = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
        private const MethodAttributes _invokeAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        private static readonly Type[] _delegateCtorSignature = { typeof(object), typeof(IntPtr) };
#endif

        private static Type MakeNewCustomDelegate(Type[] types)
        {
#if FEATURE_CORECLR
            var returnType = types[types.Length - 1];
            var parameters = types.RemoveLast();

            var builder = AssemblyGen.DefineDelegateType("Delegate" + types.Length);
            builder.DefineConstructor(_ctorAttributes, CallingConventions.Standard, _delegateCtorSignature).SetImplementationFlags(_implAttributes);
            builder.DefineMethod("Invoke", _invokeAttributes, returnType, parameters).SetImplementationFlags(_implAttributes);
            return builder.CreateType();
#else
            throw new PlatformNotSupportedException();
#endif
        }

        /// <summary>
        /// Finds a delegate type using the types in the array.
        /// We use the cache to avoid copying the array, and to cache the
        /// created delegate type
        /// </summary>
        internal static Type MakeDelegateType(Type[] types)
        {
            lock (_DelegateCache)
            {
                var curTypeInfo = _DelegateCache;

                // arguments & return type
                foreach (var item in types)
                {
                    curTypeInfo = NextTypeInfo(item, curTypeInfo);
                }
                curTypeInfo.DelegateType = curTypeInfo.DelegateType ?? MakeNewDelegate((Type[])types.Clone());
                return curTypeInfo.DelegateType;
            }
        }
    }
}