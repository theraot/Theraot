using System.Runtime.CompilerServices;

#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

using Theraot.Collections.ThreadSafe;

#endif

namespace System
{
#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

    public static class ArrayEx
    {
        private static readonly CacheDict<Type, Array> _emptyArrays = new CacheDict<Type, Array>(256);

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T[] Empty<T>()
        {
            var type = typeof(T);
            if (type == typeof(Type))
            {
                return (T[])(object)TypeEx.EmptyTypes;
            }
            if (_emptyArrays.TryGetValue(type, out var array))
            {
                return (T[])array;
            }
            var result = new T[0];
            _emptyArrays[type] = result;
            return result;
        }
    }

#else
    public static class ArrayEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T[] Empty<T>()
        {
            return Array.Empty<T>();
        }
    }

#endif
}