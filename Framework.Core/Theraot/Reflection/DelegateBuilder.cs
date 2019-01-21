// Needed for NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Theraot.Reflection
{
    public static class DelegateBuilder
    {
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
            return methodInfo.CreateDelegate(delegateType, target);
        }

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
            Type delegateType;
            if (returnType == typeof(void))
            {
                var parameterTypeArray = parameterTypes.ToArray();
                switch (parameterTypes.Count)
                {
                    case 0:
                        delegateType = typeof(Action);
                        break;

                    case 1:
                        delegateType = typeof(Action<>).MakeGenericType(parameterTypeArray);
                        break;

                    case 2:
                        delegateType = typeof(Action<,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 3:
                        delegateType = typeof(Action<,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 4:
                        delegateType = typeof(Action<,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 5:
                        delegateType = typeof(Action<,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 6:
                        delegateType = typeof(Action<,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 7:
                        delegateType = typeof(Action<,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 8:
                        delegateType = typeof(Action<,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 9:
                        delegateType = typeof(Action<,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 10:
                        delegateType = typeof(Action<,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 11:
                        delegateType = typeof(Action<,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 12:
                        delegateType = typeof(Action<,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 13:
                        delegateType = typeof(Action<,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 14:
                        delegateType = typeof(Action<,,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 15:
                        delegateType = typeof(Action<,,,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 16:
                        delegateType = typeof(Action<,,,,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    default:
                        throw new ArgumentException("No valid Action delegate found", nameof(methodInfo));
                }
            }
            else
            {
                parameterTypes.Add(returnType);
                var parameterTypeArray = parameterTypes.ToArray();
                switch (parameterTypes.Count)
                {
                    case 1:
                        delegateType = typeof(Func<>).MakeGenericType(parameterTypeArray);
                        break;

                    case 2:
                        delegateType = typeof(Func<,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 3:
                        delegateType = typeof(Func<,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 4:
                        delegateType = typeof(Func<,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 5:
                        delegateType = typeof(Func<,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 6:
                        delegateType = typeof(Func<,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 7:
                        delegateType = typeof(Func<,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 8:
                        delegateType = typeof(Func<,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 9:
                        delegateType = typeof(Func<,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 10:
                        delegateType = typeof(Func<,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 11:
                        delegateType = typeof(Func<,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 12:
                        delegateType = typeof(Func<,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 13:
                        delegateType = typeof(Func<,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 14:
                        delegateType = typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 15:
                        delegateType = typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 16:
                        delegateType = typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    case 17:
                        delegateType = typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(parameterTypeArray);
                        break;

                    default:
                        throw new ArgumentException("No valid Func delegate found", nameof(methodInfo));
                }
            }
            return methodInfo.CreateDelegate(delegateType, target);
        }
    }
}