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

        private static readonly Type[] _delegateCtorSignature = { typeof(object), typeof(IntPtr) };

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
                var delegateType = curTypeInfo.DelegateType;
                if (delegateType != null)
                {
                    return delegateType;
                }

                delegateType = MakeDelegateTypeExtracted(returnType, types.Length, types.ConvertAll(exp => exp.Type));
                curTypeInfo.DelegateType = delegateType;
                return delegateType;
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
                curTypeInfo = args.Aggregate(curTypeInfo, Func);

                // return type
                curTypeInfo = NextTypeInfo(returnType, curTypeInfo);

                // see if we have the delegate already
                var delegateType = curTypeInfo.DelegateType;
                if (delegateType != null)
                {
                    return delegateType;
                }

                delegateType = MakeDelegateTypeExtracted(returnType, args.Length, args.ConvertAll(ToType));
                curTypeInfo.DelegateType = delegateType;
                return delegateType;

                static TypeInfo Func(TypeInfo current, DynamicMetaObject arg)
                {
                    var paramType = ToType(arg);

                    return NextTypeInfo(paramType, current);
                }

                static Type ToType(DynamicMetaObject arg)
                {
                    var paramType = arg.Expression.Type;
                    if (arg.Expression is ParameterExpression pe && pe.IsByRef)
                    {
                        paramType = paramType.MakeByRefType();
                    }

                    return paramType;
                }
            }
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