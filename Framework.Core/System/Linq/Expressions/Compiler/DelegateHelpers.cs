#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Theraot.Collections;

namespace System.Linq.Expressions.Compiler
{
    internal static partial class DelegateHelpers
    {
        private const MethodAttributes _ctorAttributes = MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;

        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        private const MethodImplAttributes _implAttributes = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;

        private const MethodAttributes _invokeAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

        private static readonly Type[] _delegateCtorSignature = { typeof(object), typeof(IntPtr) };

        /// <summary>
        /// Finds a delegate type for a CallSite using the types in the ReadOnlyCollection of Expression.
        ///
        /// We take the read-only collection of Expression explicitly to avoid allocating memory (an array
        /// of types) on lookup of delegate types.
        /// </summary>
        internal static Type MakeCallSiteDelegate(Expression[] types, Type returnType)
        {
            lock (_delegateCache)
            {
                TypeInfo curTypeInfo = _delegateCache;

                // CallSite
                curTypeInfo = NextTypeInfo(typeof(CallSite), curTypeInfo);

                // arguments
                foreach (var type in types)
                {
                    curTypeInfo = NextTypeInfo(type.Type, curTypeInfo);
                }

                // return type
                curTypeInfo = NextTypeInfo(returnType, curTypeInfo);

                // see if we have the delegate already
                if (curTypeInfo.DelegateType == null)
                {
                    curTypeInfo.MakeDelegateType(returnType, types);
                }

                return curTypeInfo.DelegateType;
            }
        }

        /// <summary>
        /// Finds a delegate type for a CallSite using the MetaObject array.
        ///
        /// We take the array of MetaObject explicitly to avoid allocating memory (an array of types) on
        /// lookup of delegate types.
        /// </summary>
        internal static Type MakeDeferredSiteDelegate(DynamicMetaObject[] args, Type returnType)
        {
            lock (_delegateCache)
            {
                TypeInfo curTypeInfo = _delegateCache;

                // CallSite
                curTypeInfo = NextTypeInfo(typeof(CallSite), curTypeInfo);

                // arguments
                foreach (var mo in args)
                {
                    Type paramType = mo.Expression.Type;
                    if (IsByRef(mo))
                    {
                        paramType = paramType.MakeByRefType();
                    }
                    curTypeInfo = NextTypeInfo(paramType, curTypeInfo);
                }

                // return type
                curTypeInfo = NextTypeInfo(returnType, curTypeInfo);

                // see if we have the delegate already
                if (curTypeInfo.DelegateType == null)
                {
                    // nope, go ahead and create it and spend the
                    // cost of creating the array.
                    Type[] paramTypes = new Type[args.Length + 2];
                    paramTypes[0] = typeof(CallSite);
                    paramTypes[paramTypes.Length - 1] = returnType;
                    for (int i = 0; i < args.Length; i++)
                    {
                        DynamicMetaObject mo = args[i];
                        Type paramType = mo.Expression.Type;
                        if (IsByRef(mo))
                        {
                            paramType = paramType.MakeByRefType();
                        }
                        paramTypes[i + 1] = paramType;
                    }

                    curTypeInfo.DelegateType = MakeNewDelegate(paramTypes);
                }

                return curTypeInfo.DelegateType;
            }
        }

        private static bool IsByRef(DynamicMetaObject mo)
        {
            return mo.Expression is ParameterExpression pe && pe.IsByRef;
        }

        private static Type MakeNewCustomDelegate(Type[] types)
        {
            Type returnType = types[types.Length - 1];
            Type[] parameters = types.RemoveLast();

            TypeBuilder builder = AssemblyGen.DefineDelegateType("Delegate" + types.Length);
            builder.DefineConstructor(_ctorAttributes, CallingConventions.Standard, _delegateCtorSignature).SetImplementationFlags(_implAttributes);
            builder.DefineMethod("Invoke", _invokeAttributes, returnType, parameters).SetImplementationFlags(_implAttributes);
            return builder.CreateType();
        }
    }
}

#endif