#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public static class KeyValuePair
    {
        // Creates a new KeyValuePair<TKey, TValue> from the given values.
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}

#endif