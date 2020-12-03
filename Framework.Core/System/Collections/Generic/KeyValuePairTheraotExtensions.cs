#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public static class KeyValuePairTheraotExtensions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair, out TKey key, out TValue value)
        {
            key = keyValuePair.Key;
            value = keyValuePair.Value;
        }
    }
}

#endif