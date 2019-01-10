using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#if LESSTHAN_NET45

using System.Runtime.CompilerServices;

#endif

namespace System.Collections.Generic
{
    [Serializable]
    [ComVisible(false)]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>
#if LESSTHAN_NET45
        , IReadOnlyDictionary<TKey, TValue>
#endif
    {
        public DictionaryEx()
        {
            // Empty
        }

        public DictionaryEx(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
            // Empty
        }

        public DictionaryEx(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            // Empty
        }

        public DictionaryEx(int capacity)
            : base(capacity)
        {
            // Empty
        }

        public DictionaryEx(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
            // Empty
        }

        public DictionaryEx(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
            // Empty
        }

        protected DictionaryEx(SerializationInfo info, StreamingContext context)
#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET
            : base(info, context)
#endif
        {
            // TODO ?
            GC.KeepAlive(info);
            GC.KeepAlive(context);
        }

#if LESSTHAN_NET45

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get => Keys;
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get => Values;
        }

#endif
    }
}