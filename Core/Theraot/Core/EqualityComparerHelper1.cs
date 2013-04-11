#if FAT
﻿using System;
using System.Collections.Generic;
using System.Reflection;

using Theraot.Collections;

namespace Theraot.Core
{
    public static class EqualityComparerHelper<T>
    {
        private static IEqualityComparer<T> _default;

        static EqualityComparerHelper()
        {
            var type = typeof(T);
            PropertyInfo property = null;
            if (type.IsImplementationOf(typeof(IEquatable<>).MakeGenericType(type)))
            {
                property = GetPropertyDelegated(type, typeof(EqualityComparer<>));
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
                property = GetProperty(type, typeof(EqualityComparer<>));
            }
            _default = (IEqualityComparer<T>)property.GetValue(null, null);
        }

        public static IEqualityComparer<T> Default
        {
            get
            {
                return _default;
            }
        }

        private static PropertyInfo GetProperty(Type type, Type equalityComparerType)
        {
            Type[] genericTypeArguments = type.GetGenericArguments();
            Type generticType = equalityComparerType.MakeGenericType(genericTypeArguments);
            return generticType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        }

        private static PropertyInfo GetPropertyDelegated(Type type, Type equalityComparerType)
        {
            Type generticType = equalityComparerType.MakeGenericType(type);
            return generticType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        }
    }
}
#endif