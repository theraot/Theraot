﻿// Needed for NET40

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Theraot.Reflection
{
    [DebuggerNonUserCode]
    public static partial class TypeHelper
    {
        public static MethodInfo GetDelegateMethodInfo(Type delegateType)
        {
            if (delegateType == null)
            {
                throw new ArgumentNullException(nameof(delegateType));
            }

            var delegateTypeInfo = delegateType.GetTypeInfo();
            if (delegateTypeInfo.BaseType != typeof(MulticastDelegate))
            {
                throw new ArgumentException("Not a delegate.", nameof(delegateType));
            }

            var methodInfo = delegateType.GetRuntimeMethods().FirstOrDefault(info => string.Equals(info.Name, "Invoke", StringComparison.Ordinal));
            if (methodInfo == null)
            {
                throw new ArgumentException("Not a delegate.", nameof(delegateType));
            }

            return methodInfo;
        }

        public static ParameterInfo[] GetDelegateParameters(Type delegateType)
        {
            return GetDelegateMethodInfo(delegateType).GetParameters();
        }

        public static Type GetDelegateReturnType(Type delegateType)
        {
            return GetDelegateMethodInfo(delegateType).ReturnType;
        }
    }
}