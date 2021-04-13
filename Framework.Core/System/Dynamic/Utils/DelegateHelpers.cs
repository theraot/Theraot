#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using System.Reflection;
using Theraot.Collections.ThreadSafe;
using System.Reflection.Emit;

namespace System.Dynamic.Utils
{
    internal static class DelegateHelpers
    {
        private static readonly MethodInfo _arrayEmpty = typeof(ArrayEx).GetMethod(nameof(ArrayEx.Empty)).MakeGenericMethod(typeof(object));

        private static readonly MethodInfo _funcInvoke = typeof(Func<object[], object>).GetMethod("Invoke");

        private static readonly CacheDict<Type, DynamicMethod> _thunks = new(256);

        internal static Delegate CreateObjectArrayDelegate(Type delegateType, Func<object[], object?> handler)
        {
            // We will generate the following code:
#pragma warning disable S125 // Sections of code should not be commented out
            // object ret;
            // object[] args = new object[parameterCount];
            // args[0] = param0;
            // args[1] = param1;
            //  ...
            // try {
            //      ret = handler.Invoke(args);
            // } finally {
            //      param0 = (T0)args[0];   // only generated for each byref argument
            // }
            // return (TRet)ret;
#pragma warning restore S125 // Sections of code should not be commented out
            if (_thunks.TryGetValue(delegateType, out var thunkMethod))
            {
                return thunkMethod.CreateDelegate(delegateType, handler);
            }

            var delegateInvokeMethod = delegateType.GetInvokeMethod();

            var returnType = delegateInvokeMethod.ReturnType;
            var hasReturnValue = returnType != typeof(void);

            var parameters = delegateInvokeMethod.GetParameters();
            var paramTypes = new Type[parameters.Length + 1];
            paramTypes[0] = typeof(Func<object[], object>);
            for (var i = 0; i < parameters.Length; i++)
            {
                paramTypes[i + 1] = parameters[i].ParameterType;
            }

            thunkMethod = new DynamicMethod("Thunk", returnType, paramTypes);
            var ilGenerator = thunkMethod.GetILGenerator();

            var argArray = ilGenerator.DeclareLocal(typeof(object[]));
            var retValue = ilGenerator.DeclareLocal(typeof(object));

            // create the argument array
            if (parameters.Length == 0)
            {
                ilGenerator.Emit(OpCodes.Call, _arrayEmpty);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
                ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            }

            ilGenerator.Emit(OpCodes.Stloc, argArray);

            // populate object array
            var hasRefArgs = false;
            for (var i = 0; i < parameters.Length; i++)
            {
                var paramIsByReference = parameters[i].ParameterType.IsByRef;
                var paramType = parameters[i].ParameterType;
                if (paramIsByReference)
                {
                    paramType = paramType.GetElementType();
                }

                hasRefArgs = hasRefArgs || paramIsByReference;

                ilGenerator.Emit(OpCodes.Ldloc, argArray);
                ilGenerator.Emit(OpCodes.Ldc_I4, i);
                ilGenerator.Emit(OpCodes.Ldarg, i + 1);

                if (paramIsByReference)
                {
                    ilGenerator.Emit(OpCodes.Ldobj, paramType);
                }

                var boxType = ConvertToBoxableType(paramType);
                ilGenerator.Emit(OpCodes.Box, boxType);
                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }

            if (hasRefArgs)
            {
                ilGenerator.BeginExceptionBlock();
            }

            // load delegate
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // load array
            ilGenerator.Emit(OpCodes.Ldloc, argArray);

            // invoke Invoke
            ilGenerator.Emit(OpCodes.Callvirt, _funcInvoke);
            ilGenerator.Emit(OpCodes.Stloc, retValue);

            if (hasRefArgs)
            {
                // copy back ref/out args
                ilGenerator.BeginFinallyBlock();
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (!parameters[i].ParameterType.IsByRef)
                    {
                        continue;
                    }

                    var byrefToType = parameters[i].ParameterType.GetElementType();

                    // update parameter
                    ilGenerator.Emit(OpCodes.Ldarg, i + 1);
                    ilGenerator.Emit(OpCodes.Ldloc, argArray);
                    ilGenerator.Emit(OpCodes.Ldc_I4, i);
                    ilGenerator.Emit(OpCodes.Ldelem_Ref);
                    ilGenerator.Emit(OpCodes.Unbox_Any, byrefToType);
                    ilGenerator.Emit(OpCodes.Stobj, byrefToType);
                }

                ilGenerator.EndExceptionBlock();
            }

            if (hasReturnValue)
            {
                ilGenerator.Emit(OpCodes.Ldloc, retValue);
                ilGenerator.Emit(OpCodes.Unbox_Any, ConvertToBoxableType(returnType));
            }

            ilGenerator.Emit(OpCodes.Ret);

            _thunks[delegateType] = thunkMethod;

            return thunkMethod.CreateDelegate(delegateType, handler);
        }

        private static Type ConvertToBoxableType(Type t)
        {
            return t.IsPointer ? typeof(IntPtr) : t;
        }
    }
}

#endif