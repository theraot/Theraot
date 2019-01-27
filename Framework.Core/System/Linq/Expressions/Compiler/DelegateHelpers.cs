#if LESSTHAN_NET35
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Collections;

namespace System.Linq.Expressions.Compiler
{
    internal static partial class DelegateHelpers
    {
        private const MethodAttributes _ctorAttributes = MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;

        private const MethodImplAttributes _implAttributes = (MethodImplAttributes)((int)MethodImplAttributes.Runtime | (int)MethodImplAttributes.Managed);

        private const MethodAttributes _invokeAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

        private static readonly Type[] _delegateCtorSignature = {typeof(object), typeof(IntPtr)};

        internal static Type MakeCallSiteDelegate(Expression[] types, Type returnType)
        {
            lock (_delegateCache)
            {
                var curTypeInfo = _delegateCache;

                // CallSite
                curTypeInfo = NextTypeInfo(typeof(CallSite), curTypeInfo);

                // arguments
                curTypeInfo = types.Aggregate(curTypeInfo, (current, type) => NextTypeInfo(type.Type, current));

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

        internal static Type MakeDeferredSiteDelegate(DynamicMetaObject[] args, Type returnType)
        {
            lock (_delegateCache)
            {
                var curTypeInfo = _delegateCache;

                // CallSite
                curTypeInfo = NextTypeInfo(typeof(CallSite), curTypeInfo);

                // arguments
                foreach (var mo in args)
                {
                    var paramType = mo.Expression.Type;
                    if (IsByRef(mo))
                    {
                        paramType = paramType.MakeByRefType();
                    }

                    curTypeInfo = NextTypeInfo(paramType, curTypeInfo);
                }

                // return type
                curTypeInfo = NextTypeInfo(returnType, curTypeInfo);

                // see if we have the delegate already
                if (curTypeInfo.DelegateType != null)
                {
                    return curTypeInfo.DelegateType;
                }

                {
                    // nope, go ahead and create it and spend the
                    // cost of creating the array.
                    var paramTypes = new Type[args.Length + 2];
                    paramTypes[0] = typeof(CallSite);
                    paramTypes[paramTypes.Length - 1] = returnType;
                    for (var i = 0; i < args.Length; i++)
                    {
                        var mo = args[i];
                        var paramType = mo.Expression.Type;
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
            var returnType = types[types.Length - 1];
            var parameters = types.RemoveLast();

            var builder = AssemblyGen.DefineDelegateType("Delegate" + types.Length);
            builder.DefineConstructor(_ctorAttributes, CallingConventions.Standard, _delegateCtorSignature).SetImplementationFlags(_implAttributes);
            builder.DefineMethod("Invoke", _invokeAttributes, returnType, parameters).SetImplementationFlags(_implAttributes);
            return builder.CreateType();
        }
    }
}

#endif