// Needed for NET40

using System;
using System.Diagnostics;
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
                throw new ArgumentException("Not a delegate.");
            }

            var methodInfo = delegateType.GetMethod("Invoke");
            if (methodInfo == null)
            {
                throw new ArgumentException("Not a delegate.");
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