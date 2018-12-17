// Needed for NET40

using System;
using System.Collections.Generic;
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

        public static bool IsArrayTypeAssignableTo(Type type, Type target)
        {
            if (!type.IsArray || !target.IsArray)
            {
                return false;
            }
            if (type.GetArrayRank() != target.GetArrayRank())
            {
                return false;
            }
            return type.GetElementType().IsAssignableToInternal(target.GetElementType());
        }

        public static bool IsArrayTypeAssignableToInterface(Type type, Type target)
        {
            if (!type.IsArray)
            {
                return false;
            }
            return
                (
                    target.IsGenericInstanceOf(typeof(IList<>))
                    || target.IsGenericInstanceOf(typeof(ICollection<>))
                    || target.IsGenericInstanceOf(typeof(IEnumerable<>))
                )
                && type.GetElementType() == target.GetGenericArguments()[0];
        }
    }
}