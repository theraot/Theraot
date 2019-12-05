// Needed for NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Theraot.Reflection
{
    public static class DelegateBuilder
    {
        public static Delegate BuildDelegate(MethodInfo methodInfo, object target)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            if (methodInfo.IsStatic != (target == null))
            {
                if (target == null)
                {
                    throw new ArgumentNullException(nameof(target), "target is null and the method is not static.");
                }

                throw new ArgumentException("target is not null and the method is static", nameof(target));
            }

            var returnType = methodInfo.GetReturnType();
            var parameters = methodInfo.GetParameters();
            if (parameters.Any(parameterInfo => parameterInfo.IsByRefParameterInternal()))
            {
                throw new ArgumentException("By ref parameters are not supported", nameof(methodInfo));
            }

            var parameterTypes = new List<Type>(parameters.Select(parameterInfo => parameterInfo.ParameterType));
            Type? delegateType;
            if (returnType == typeof(void))
            {
                var parameterTypeArray = parameterTypes.ToArray();
                delegateType = GetActionType(parameterTypeArray);
            }
            else
            {
                parameterTypes.Add(returnType);
                var parameterTypeArray = parameterTypes.ToArray();
                delegateType = GetFuncType(parameterTypeArray);
            }

            if (delegateType == null)
            {
                throw new ArgumentException("Could not infer delegate type", nameof(methodInfo));
            }

            return target == null
                ? methodInfo.CreateDelegate(delegateType)
                : methodInfo.CreateDelegate(delegateType, target);
        }

        public static Delegate BuildDelegate(Type delegateType, MethodInfo methodInfo, object target)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            if (methodInfo.IsStatic != (target == null))
            {
                if (target == null)
                {
                    throw new ArgumentNullException(nameof(target), "target is null and the method is not static.");
                }

                throw new ArgumentException("target is not null and the method is static", nameof(target));
            }

            if (delegateType == null)
            {
                throw new ArgumentNullException(nameof(delegateType));
            }

            return target == null
                ? methodInfo.CreateDelegate(delegateType)
                : methodInfo.CreateDelegate(delegateType, target);
        }

        public static Type? GetActionType(Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
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

        public static Type? GetFuncType(Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
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
    }
}