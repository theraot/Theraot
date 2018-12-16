﻿// Needed for NET40

using System;
using System.Reflection;
using System.Runtime.InteropServices;
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
            var type = methodInfo.DeclaringType;
            if (type == null)
            {
                throw new ArgumentException("methodInfo.DeclaringType is null", nameof(methodInfo));
            }
            return methodInfo.CreateDelegate(type, target);
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
                GC.KeepAlive(exception);
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

        public static bool IsAtomic<T>()
        {
#if NETCOREAPP1_0 || NETCOREAPP1_1
            var info = typeof(T).GetTypeInfo();
            return info.IsClass || info.IsPrimitive && Marshal.SizeOf<T>() <= IntPtr.Size;
#else
            var type = typeof(T);
            var info = type.GetTypeInfo();
            return info.IsClass || info.IsPrimitive && Marshal.SizeOf(type) <= IntPtr.Size;
#endif
        }

        public static bool IsContravariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return PrivateIsContravariant(type);
        }

        public static bool IsCovariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return PrivateIsCovariant(type);
        }

        public static bool IsDelegate(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return PrivateIsDelegate(type);
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

        public static bool IsInvariant(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return PrivateIsInvariant(type);
        }

        public static T LazyCreate<T>(ref T target)
                                                                                                                                                                            where T : class
        {
            var found = target;
            if (found == null)
            {
                found = Volatile.Read(ref target);
                if (found == null)
                {
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
                    if (found == null)
                    {
                        return created;
                    }
                }
            }
            return found;
        }

        public static T LazyCreate<T>(ref T target, object syncRoot)
            where T : class
        {
            var found = target;
            if (found == null)
            {
                found = Volatile.Read(ref target);
                if (found == null)
                {
                    lock (syncRoot)
                    {
                        found = Volatile.Read(ref target);
                        if (found == null)
                        {
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
                            if (found == null)
                            {
                                return created;
                            }
                        }
                    }
                }
            }
            return found;
        }

        public static T LazyCreate<T>(ref T target, Func<T> valueFactory)
            where T : class
        {
            var found = target;
            if (found == null)
            {
                found = Volatile.Read(ref target);
                if (found == null)
                {
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
                }
            }
            return found;
        }

        public static T LazyCreate<T>(ref T target, Func<T> valueFactory, object syncRoot)
            where T : class
        {
            var found = target;
            if (found == null)
            {
                found = Volatile.Read(ref target);
                if (found == null)
                {
                    lock (syncRoot)
                    {
                        found = Volatile.Read(ref target);
                        if (found == null)
                        {
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
                        }
                    }
                }
            }
            return found;
        }

        private static bool PrivateIsContravariant(Type type)
        {
            var info = type.GetTypeInfo();
            return 0 != (info.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
        }

        private static bool PrivateIsCovariant(Type type)
        {
            var info = type.GetTypeInfo();
            return 0 != (info.GenericParameterAttributes & GenericParameterAttributes.Covariant);
        }

        private static bool PrivateIsDelegate(Type type)
        {
            var info = type.GetTypeInfo();
            return info.IsSubclassOf(typeof(MulticastDelegate));
        }

        private static bool PrivateIsInvariant(Type type)
        {
            var info = type.GetTypeInfo();
            return 0 == (info.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        }
    }
}