// Needed for Workaround

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
using System.Runtime.CompilerServices;
#endif

#if FAT

using System.Reflection;
using System.Threading;
using Theraot.Collections;
using Theraot.Threading.Needles;

#endif

namespace Theraot.Core
{
    public static class EqualityComparerHelper
    {
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#endif

        public static IEqualityComparer<T> ToComparer<T>(this Func<T, T, bool> equalityComparison, Func<T, int> getHashCode)
        {
            return new CustomEqualityComparer<T>(equalityComparison, getHashCode);
        }
    }
}

#if FAT

namespace Theraot.Core
{
    public static class EqualityComparerHelper<T>
    {
        static EqualityComparerHelper()
        {
            var type = typeof(T);
            PropertyInfo property;
            if (type.IsImplementationOf(typeof(IEquatable<>).MakeGenericType(type)))
            {
                property = GetPropertyDelegated(type, typeof(EqualityComparer<>));
            }
            else if (type.IsAssignableTo(typeof(Delegate)))
            {
                property = typeof(DelegateEqualityComparer).GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            }
            else if (type.IsAssignableTo(typeof(Thread)))
            {
                property = typeof(ReferenceEqualityComparer).GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            }
            else if (type.IsGenericImplementationOf(out var tmp, typeof(INeedle<>)))
            {
                var types = tmp.GetGenericArguments();
                var conversionType = typeof(NeedleConversionEqualityComparer<,>).MakeGenericType(tmp, types[0]);
                Default = (IEqualityComparer<T>)conversionType.Create
                           (
                               GetPropertyDelegated
                               (
                                   types[0],
                                   typeof(EqualityComparerHelper<>)
                               ).GetValue
                               (
                                   null,
                                   null
                               )
                           );
                return;
            }
            else if (type.IsGenericInstanceOf(typeof(KeyValuePair<,>)))
            {
                property = GetProperty(type, typeof(KeyValuePairEqualityComparer<,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,,,,>));
            }
            else if (type.IsGenericInstanceOf(typeof(Tuple<,,,,,,,>)))
            {
                property = GetProperty(type, typeof(TupleEqualityComparer<,,,,,,,>));
            }
            else
            {
                property = GetPropertyDelegated(type, typeof(EqualityComparer<>));
            }
            // ReSharper disable once PossibleNullReferenceException
            Default = (IEqualityComparer<T>)property.GetValue(null, null);
        }

        public static IEqualityComparer<T> Default { get; }

        private static PropertyInfo GetProperty(Type type, Type equalityComparerType)
        {
            var genericTypeArguments = type.GetGenericArguments();
            var genericType = equalityComparerType.MakeGenericType(genericTypeArguments);
            return genericType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        }

        private static PropertyInfo GetPropertyDelegated(Type type, Type equalityComparerType)
        {
            var genericType = equalityComparerType.MakeGenericType(type);
            return genericType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        }
    }
}

#endif