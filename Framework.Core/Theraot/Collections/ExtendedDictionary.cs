using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
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
    }
}