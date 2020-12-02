#if TARGETS_NET || LESSTHAN_NETCORE20 || LESSTHAN_NETSTANDARD21

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System.Collections
{
    public static class DictionaryEntryTheraotExtensions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Deconstruct(this DictionaryEntry dictionaryEntry, out object key, out object? value)
        {
            key = dictionaryEntry.Key;
            value = dictionaryEntry.Value;
        }
    }
}

#endif