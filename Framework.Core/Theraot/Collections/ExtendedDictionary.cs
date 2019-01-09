using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class ExtendedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        public ExtendedDictionary()
        {
            // Empty
        }

        public ExtendedDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
            // Empty
        }

        public ExtendedDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            // Empty
        }

        public ExtendedDictionary(int capacity)
            : base(capacity)
        {
            // Empty
        }

        public ExtendedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
            // Empty
        }

        public ExtendedDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
            // Empty
        }

        protected ExtendedDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Empty
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
    }
}