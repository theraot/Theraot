// Needed for NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Theraot.Core;

namespace Theraot.Reflection
{
    public static partial class TypeHelper
    {
        public static TTarget As<TTarget>(object source)
            where TTarget : class
        {
            return As
            (
                source,
                new Func<TTarget>
                (
                    () => throw new InvalidOperationException("Cannot convert to " + typeof(TTarget).Name))
            );
        }

        public static TTarget As<TTarget>(object source, TTarget def)
            where TTarget : class
        {
            return As(source, () => def);
        }

        public static TTarget As<TTarget>(object source, Func<TTarget> alternative)
            where TTarget : class
        {
            if (alternative == null)
            {
                throw new ArgumentNullException(nameof(alternative));
            }
            if (!(source is TTarget sourceAsTarget))
            {
                return alternative();
            }
            return sourceAsTarget;
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

        public static TTarget Cast<TTarget>(object source)
        {
            return Cast
            (
                source,
                new Func<TTarget>
                (
                    () => throw new InvalidOperationException("Cannot convert to " + typeof(TTarget).Name))
            );
        }

        public static TTarget Cast<TTarget>(object source, TTarget def)
        {
            return Cast(source, () => def);
        }

        public static TTarget Cast<TTarget>(object source, Func<TTarget> alternative)
        {
            if (alternative == null)
            {
                throw new ArgumentNullException(nameof(alternative));
            }
            try
            {
                var sourceAsTarget = (TTarget)source;
                return sourceAsTarget;
            }
            catch (Exception exception)
            {
                No.Op(exception);
                return alternative();
            }
        }

        public static TReturn Default<TReturn>()
        {
            return FuncHelper.GetDefaultFunc<TReturn>().Invoke();
        }

        public static MethodInfo FindConversionOperator(MethodInfo[] methods, Type typeFrom, Type typeTo, bool implicitOnly)
        {
            foreach (var method in methods)
            {
                if (method.Name != "op_Implicit" && (implicitOnly || method.Name != "op_Explicit"))
                {
                    continue;
                }
                if (method.ReturnType != typeTo)
                {
                    continue;
                }
                var parameters = method.GetParameters();
                if (parameters[0].ParameterType != typeFrom)
                {
                    continue;
                }
                return method;
            }
            return null;
        }

        public static Func<TReturn> GetDefault<TReturn>()
        {
            return FuncHelper.GetDefaultFunc<TReturn>();
        }

        public static bool IsImplicitBoxingConversion(Type source, Type target)
        {
            var info = source.GetTypeInfo();
            if (info.IsValueType && (target == typeof(object) || target == typeof(ValueType)))
            {
                return true;
            }
            if (info.IsEnum && target == typeof(Enum))
            {
                return true;
            }
            return false;
        }

        public static bool IsImplicitNumericConversion(Type source, Type target)
        {
            if (source == typeof(sbyte))
            {
                if (
                    target == typeof(short)
                    || target == typeof(int)
                    || target == typeof(long)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(byte))
            {
                if (
                    target == typeof(short)
                    || target == typeof(ushort)
                    || target == typeof(int)
                    || target == typeof(uint)
                    || target == typeof(long)
                    || target == typeof(ulong)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(short))
            {
                if (
                    target == typeof(int)
                    || target == typeof(long)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(ushort))
            {
                if (
                    target == typeof(int)
                    || target == typeof(uint)
                    || target == typeof(long)
                    || target == typeof(ulong)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(int))
            {
                if (
                    target == typeof(long)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(uint))
            {
                if (
                    target == typeof(ulong)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(long) || target == typeof(ulong))
            {
                if (
                    target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(char))
            {
                if (
                    target == typeof(ushort)
                    || target == typeof(int)
                    || target == typeof(uint)
                    || target == typeof(long)
                    || target == typeof(ulong)
                    || target == typeof(float)
                    || target == typeof(double)
                    || target == typeof(decimal)
                    )
                {
                    return true;
                }
            }
            else if (source == typeof(float))
            {
                return target == typeof(double);
            }
            return false;
        }

        public static T LazyCreate<T>(ref T target)
            where T : class
        {
            var found = target;
            if (found != null)
            {
                return found;
            }
            found = Volatile.Read(ref target);
            if (found != null)
            {
                return found;
            }
            T created;
            try
            {
                created = Activator.CreateInstance<T>();
            }
            catch
            {
                throw new MissingMemberException("The type being lazily initialized does not have a public, parameterless constructor.");
            }
            found = Interlocked.CompareExchange(ref target, created, null);
            return found ?? created;
        }

        public static T LazyCreate<T>(ref T target, object syncRoot)
            where T : class
        {
            var found = target;
            if (found != null)
            {
                return found;
            }
            found = Volatile.Read(ref target);
            if (found != null)
            {
                return found;
            }
            lock (syncRoot)
            {
                return LazyCreate(ref target);
            }
        }

        public static T LazyCreate<T>(ref T target, Func<T> valueFactory)
            where T : class
        {
            var found = target;
            if (found != null)
            {
                return found;
            }
            found = Volatile.Read(ref target);
            if (found != null)
            {
                return found;
            }
            var created = valueFactory();
            if (created == null)
            {
                throw new InvalidOperationException("valueFactory returned null");
            }
            found = Interlocked.CompareExchange(ref target, created, null);
            if (found == null)
            {
                return created;
            }
            return found;
        }

        public static T LazyCreate<T>(ref T target, Func<T> valueFactory, object syncRoot)
            where T : class
        {
            var found = target;
            if (found != null)
            {
                return found;
            }
            found = Volatile.Read(ref target);
            if (found != null)
            {
                return found;
            }
            lock (syncRoot)
            {
                return LazyCreate(ref target, valueFactory);
            }
        }
    }
}